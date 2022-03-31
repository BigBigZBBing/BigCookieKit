using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.Buffers;
using System.Numerics;

namespace UnitConsole
{
    public static class RSATest
    {
        public static AsnWriter WritePkcs1PublicKey(RSAParameters rsaParameters)
        {
            AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);
            writer.PushSequence();
            writer.WriteKeyParameterInteger(rsaParameters.Modulus);
            writer.WriteKeyParameterInteger(rsaParameters.Exponent);
            writer.PopSequence();
            return writer;
        }

        public static AsnWriter WritePkcs1PrivateKey(RSAParameters rsaParameters)
        {
            AsnWriter writer = new AsnWriter(AsnEncodingRules.DER);
            writer.PushSequence();
            // Format version 0
            writer.WriteKeyParameterInteger(new byte[] { 0 }); // 5
            writer.WriteKeyParameterInteger(rsaParameters.Modulus); // 137
            writer.WriteKeyParameterInteger(rsaParameters.Exponent); // 142
            writer.WriteKeyParameterInteger(rsaParameters.D); // 273
            writer.WriteKeyParameterInteger(rsaParameters.P); // 340
            writer.WriteKeyParameterInteger(rsaParameters.Q); // 407
            writer.WriteKeyParameterInteger(rsaParameters.DP); // 473
            writer.WriteKeyParameterInteger(rsaParameters.DQ); // 539
            writer.WriteKeyParameterInteger(rsaParameters.InverseQ); // 605
            writer.PopSequence();
            return writer;
        }

        public static AsnWriter WritePkcs8PrivateKey(ReadOnlySpan<byte> pkcs1PrivateKey)
        {
            AsnWriter writer = new AsnWriter(AsnEncodingRules.BER);

            using (writer.PushSequence())
            {
                // Version 0 format (no attributes)
                writer.WriteInteger(0);
                WriteAlgorithmIdentifier(writer);

                writer.WriteOctetStringCore(Asn1Tag.PrimitiveOctetString, pkcs1PrivateKey);
            }

            return writer;
        }

        private static void WriteAlgorithmIdentifier(AsnWriter writer)
        {
            writer.PushSequence(); // 7

            writer.WriteObjectIdentifierCore(Asn1Tag.ObjectIdentifier, "1.2.840.113549.1.1.1".AsSpan());
            writer.WriteNullCore(Asn1Tag.Null);

            writer.PopSequence();
        }
    }

    internal static class CryptoPool
    {
        internal const int ClearAll = -1;

        internal static byte[] Rent(int minimumLength) => ArrayPool<byte>.Shared.Rent(minimumLength);

        internal static void Return(ArraySegment<byte> arraySegment)
        {
            Debug.Assert(arraySegment.Array != null);
            Debug.Assert(arraySegment.Offset == 0);

            Return(arraySegment.Array, arraySegment.Count);
        }

        internal static void Return(byte[] array, int clearSize = ClearAll)
        {
            Debug.Assert(clearSize <= array.Length);
            bool clearWholeArray = clearSize < 0;

            if (!clearWholeArray && clearSize != 0)
            {
#if (NETCOREAPP || NETSTANDARD2_1) && !CP_NO_ZEROMEMORY
                CryptographicOperations.ZeroMemory(array.AsSpan(0, clearSize));
#else
                Array.Clear(array, 0, clearSize);
#endif
            }

            ArrayPool<byte>.Shared.Return(array, clearWholeArray);
        }
    }

    internal sealed class SetOfValueComparer : IComparer<ReadOnlyMemory<byte>>
    {
        internal static SetOfValueComparer Instance { get; } = new SetOfValueComparer();

        public int Compare(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y) =>
            Compare(x.Span, y.Span);

        internal static int Compare(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y)
        {
            int min = Math.Min(x.Length, y.Length);
            int diff;

            for (int i = 0; i < min; i++)
            {
                int xVal = x[i];
                byte yVal = y[i];
                diff = xVal - yVal;

                if (diff != 0)
                {
                    return diff;
                }
            }

            // The sorting rules (T-REC-X.690-201508 sec 11.6) say that the shorter one
            // counts as if it are padded with as many 0x00s on the right as required for
            // comparison.
            //
            // But, since a shorter definite value will have already had the length bytes
            // compared, it was already different.  And a shorter indefinite value will
            // have hit end-of-contents, making it already different.
            //
            // This is here because the spec says it should be, but no values are known
            // which will make diff != 0.
            diff = x.Length - y.Length;

            return diff;
        }
    }

    public sealed partial class AsnWriter
    {
        private byte[] _buffer = null!;
        private int _offset;
        private Stack<StackFrame> _nestingStack;

        public AsnEncodingRules RuleSet { get; }

        public AsnWriter(AsnEncodingRules ruleSet)
        {
            if (ruleSet != AsnEncodingRules.BER &&
                ruleSet != AsnEncodingRules.CER &&
                ruleSet != AsnEncodingRules.DER)
            {
                throw new ArgumentOutOfRangeException(nameof(ruleSet));
            }

            RuleSet = ruleSet;
        }

        public void WriteOctetStringCore(Asn1Tag tag, ReadOnlySpan<byte> octetString)
        {
            if (RuleSet == AsnEncodingRules.CER)
            {
                // If it's bigger than a primitive segment, use the constructed encoding
                // T-REC-X.690-201508 sec 9.2
                if (octetString.Length > 1000)
                {
                    WriteConstructedCerOctetString(tag, octetString);
                    return;
                }
            }

            // Clear the constructed flag, if present.
            WriteTag(tag.AsPrimitive());
            WriteLength(octetString.Length);
            octetString.CopyTo(_buffer.AsSpan(_offset));
            _offset += octetString.Length;
        }

        public void WriteNullCore(Asn1Tag tag)
        {
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(0);
        }

        public void WriteObjectIdentifierCore(Asn1Tag tag, ReadOnlySpan<char> oidValue)
        {
            // The worst case is "1.1.1.1.1", which takes 4 bytes (5 components, with the first two condensed)
            // Longer numbers get smaller: "2.1.127" is only 2 bytes. (81d (0x51) and 127 (0x7F))
            // So length / 2 should prevent any reallocations.
            byte[] tmp = CryptoPool.Rent(oidValue.Length / 2);
            int tmpOffset = 0;

            try
            {
                int firstComponent = oidValue[0] switch
                {
                    '0' => 0,
                    '1' => 1,
                    '2' => 2,
                    _ => throw new NotImplementedException(),
                };

                // The first two components are special:
                // ITU X.690 8.19.4:
                //   The numerical value of the first subidentifier is derived from the values of the first two
                //   object identifier components in the object identifier value being encoded, using the formula:
                //       (X*40) + Y
                //   where X is the value of the first object identifier component and Y is the value of the
                //   second object identifier component.
                //       NOTE - This packing of the first two object identifier components recognizes that only
                //          three values are allocated from the root node, and at most 39 subsequent values from
                //          nodes reached by X = 0 and X = 1.

                // skip firstComponent and the trailing .
                ReadOnlySpan<char> remaining = oidValue.Slice(2);

                BigInteger subIdentifier = ParseSubIdentifier(ref remaining);
                subIdentifier += 40 * firstComponent;

                int localLen = EncodeSubIdentifier(tmp.AsSpan(tmpOffset), ref subIdentifier);
                tmpOffset += localLen;

                while (!remaining.IsEmpty)
                {
                    subIdentifier = ParseSubIdentifier(ref remaining);
                    localLen = EncodeSubIdentifier(tmp.AsSpan(tmpOffset), ref subIdentifier);
                    tmpOffset += localLen;
                }

                Debug.Assert(!tag.IsConstructed);
                WriteTag(tag);
                WriteLength(tmpOffset);
                Buffer.BlockCopy(tmp, 0, _buffer, _offset, tmpOffset);
                _offset += tmpOffset;
            }
            finally
            {
                CryptoPool.Return(tmp, tmpOffset);
            }
        }

        // ITU-T-X.690-201508 sec 8.19.5
        private static int EncodeSubIdentifier(Span<byte> dest, ref BigInteger subIdentifier)
        {
            Debug.Assert(dest.Length > 0);

            if (subIdentifier.IsZero)
            {
                dest[0] = 0;
                return 1;
            }
            subIdentifier = int.MaxValue;
            BigInteger unencoded = subIdentifier;
            int idx = 0;

            // 除去第一位 其余 -128
            // 例：840=72 (134-128)=6
            // 840-6-72=762
            // 762/127=6
            // ((6)<<7)+72=840

            // 例：113549=13 (247-128)=119 (134-128)=6
            // 113549-6-119-13=113411
            // 113411/127=893
            // ((893-6)<<7)+13=113549

            // 例：2147483647=127 (255-128)=127 (255-128)=127 (255-128)=127 (135-128)=7
            // 2147483647-127-127-127-127-7=2147483132
            // 2147483647/127=16909316
            do
            {
                BigInteger cur = unencoded & 0x7F;
                byte curByte = (byte)cur;

                if (subIdentifier != unencoded)
                {
                    curByte |= 0x80;
                }

                unencoded >>= 7;
                dest[idx] = curByte;
                idx++;
            }
            while (unencoded != BigInteger.Zero);

            Reverse(dest.Slice(0, idx));
            return idx;
        }

        internal static void Reverse(Span<byte> span)
        {
            int i = 0;
            int j = span.Length - 1;

            while (i < j)
            {
                byte tmp = span[i];
                span[i] = span[j];
                span[j] = tmp;

                i++;
                j--;
            }
        }

        private static BigInteger ParseSubIdentifier(ref ReadOnlySpan<char> oidValue)
        {
            int endIndex = oidValue.IndexOf('.');

            if (endIndex == -1)
            {
                endIndex = oidValue.Length;
            }

            // The following code is equivalent to
            // BigInteger.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out value)
            // TODO: Split this for netstandard vs netcoreapp for span-perf?.
            BigInteger value = BigInteger.Zero;

            for (int position = 0; position < endIndex; position++)
            {
                value *= 10;
                value += AtoI(oidValue[position]);
            }

            oidValue = oidValue.Slice(Math.Min(oidValue.Length, endIndex + 1));
            return value;
        }

        private static int AtoI(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }
            return 0;
        }

        public void WriteInteger(long value, Asn1Tag? tag = null)
        {
            WriteIntegerCore(tag?.AsPrimitive() ?? Asn1Tag.Integer, value);
        }

        private void WriteNonNegativeIntegerCore(Asn1Tag tag, ulong value)
        {
            int valueLength;

            // 0x80 needs two bytes: 0x00 0x80
            if (value < 0x80)
                valueLength = 1;
            else if (value < 0x8000)
                valueLength = 2;
            else if (value < 0x800000)
                valueLength = 3;
            else if (value < 0x80000000)
                valueLength = 4;
            else if (value < 0x80_00000000)
                valueLength = 5;
            else if (value < 0x8000_00000000)
                valueLength = 6;
            else if (value < 0x800000_00000000)
                valueLength = 7;
            else if (value < 0x80000000_00000000)
                valueLength = 8;
            else
                valueLength = 9;

            // Clear the constructed bit, if it was set.
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(valueLength);

            ulong remaining = value;
            int idx = _offset + valueLength - 1;

            do
            {
                _buffer[idx] = (byte)remaining;
                remaining >>= 8;
                idx--;
            } while (idx >= _offset);

#if DEBUG
            if (valueLength > 1)
            {
                // T-REC-X.690-201508 sec 8.3.2
                // Cannot start with 9 bits of 0 (or 9 bits of 1, but that's not this method).
                Debug.Assert(_buffer[_offset] != 0 || _buffer[_offset + 1] > 0x7F);
            }
#endif

            _offset += valueLength;
        }

        private void WriteIntegerCore(Asn1Tag tag, long value)
        {
            if (value >= 0)
            {
                WriteNonNegativeIntegerCore(tag, (ulong)value);
                return;
            }

            int valueLength;

            if (value >= sbyte.MinValue)
                valueLength = 1;
            else if (value >= short.MinValue)
                valueLength = 2;
            else if (value >= unchecked((long)0xFFFFFFFF_FF800000))
                valueLength = 3;
            else if (value >= int.MinValue)
                valueLength = 4;
            else if (value >= unchecked((long)0xFFFFFF80_00000000))
                valueLength = 5;
            else if (value >= unchecked((long)0xFFFF8000_00000000))
                valueLength = 6;
            else if (value >= unchecked((long)0xFF800000_00000000))
                valueLength = 7;
            else
                valueLength = 8;

            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);
            WriteLength(valueLength);

            long remaining = value;
            int idx = _offset + valueLength - 1;

            do
            {
                _buffer[idx] = (byte)remaining;
                remaining >>= 8;
                idx--;
            } while (idx >= _offset);

#if DEBUG
            if (valueLength > 1)
            {
                // T-REC-X.690-201508 sec 8.3.2
                // Cannot start with 9 bits of 1 (or 9 bits of 0, but that's not this method).
                Debug.Assert(_buffer[_offset] != 0xFF || _buffer[_offset + 1] < 0x80);
            }
#endif

            _offset += valueLength;
        }

        public byte[] Encode()
        {
            if (_offset == 0)
            {
                return Array.Empty<byte>();
            }

            return _buffer.AsSpan(0, _offset).ToArray();
        }

        public void WriteKeyParameterInteger(ReadOnlySpan<byte> integer)
        {
            Debug.Assert(!integer.IsEmpty);

            if (integer[0] == 0)
            {
                int newStart = 1;

                while (newStart < integer.Length)
                {
                    if (integer[newStart] >= 0x80)
                    {
                        newStart--;
                        break;
                    }

                    if (integer[newStart] != 0)
                    {
                        break;
                    }

                    newStart++;
                }

                if (newStart == integer.Length)
                {
                    newStart--;
                }

                integer = integer.Slice(newStart);
            }

            this.WriteIntegerUnsigned(integer);
        }

        public void WriteIntegerUnsigned(ReadOnlySpan<byte> value, Asn1Tag? tag = null)
        {
            WriteIntegerUnsignedCore(tag?.AsPrimitive() ?? Asn1Tag.Integer, value);
        }

        private void WriteIntegerUnsignedCore(Asn1Tag tag, ReadOnlySpan<byte> value)
        {
            Debug.Assert(!tag.IsConstructed);
            WriteTag(tag);

            if (value[0] >= 0x80)
            {
                WriteLength(checked(value.Length + 1));
                _buffer[_offset] = 0;
                _offset++;
            }
            else
            {
                WriteLength(value.Length);
            }

            value.CopyTo(_buffer.AsSpan(_offset));
            _offset += value.Length;
        }

        public Scope PushSequence(Asn1Tag? tag = null)
        {
            // Assert the constructed flag, in case it wasn't.
            return PushSequenceCore(tag?.AsConstructed() ?? Asn1Tag.Sequence);
        }

        public void PopSequence(Asn1Tag? tag = null)
        {
            // Assert the constructed flag, in case it wasn't.
            PopSequenceCore(tag?.AsConstructed() ?? Asn1Tag.Sequence);
        }

        private void PopSequenceCore(Asn1Tag tag)
        {
            Debug.Assert(tag.IsConstructed);
            PopTag(tag, UniversalTagNumber.Sequence);
        }

        private void PopTag(Asn1Tag tag, UniversalTagNumber tagType, bool sortContents = false)
        {
            (Asn1Tag stackTag, int lenOffset, UniversalTagNumber stackTagType) = _nestingStack.Peek();

            Debug.Assert(tag.IsConstructed);

            _nestingStack.Pop();

            if (sortContents)
            {
                Debug.Assert(tagType == UniversalTagNumber.SetOf);
                SortContents(_buffer, lenOffset + 1, _offset);
            }

            // BER could use the indefinite encoding that CER does.
            // But since the definite encoding form is easier to read (doesn't require a contextual
            // parser to find the end-of-contents marker) some ASN.1 readers (including the previous
            // incarnation of AsnReader) may choose not to support it.
            //
            // So, BER will use the DER rules here, in the interest of broader compatibility.

            // T-REC-X.690-201508 sec 9.1 (constructed CER => indefinite length)
            // T-REC-X.690-201508 sec 8.1.3.6
            if (RuleSet == AsnEncodingRules.CER && tagType != UniversalTagNumber.OctetString)
            {
                WriteEndOfContents();
                return;
            }

            int containedLength = _offset - 1 - lenOffset;
            Debug.Assert(containedLength >= 0);

            int start = lenOffset + 1;

            // T-REC-X.690-201508 sec 9.2
            // T-REC-X.690-201508 sec 10.2
            if (tagType == UniversalTagNumber.OctetString)
            {
                if (RuleSet != AsnEncodingRules.CER || containedLength <= 1000)
                {
                    // Need to replace the tag with the primitive tag.
                    // Since the P/C bit doesn't affect the length, overwrite the tag.
                    int tagLen = tag.CalculateEncodedSize();
                    tag.AsPrimitive().Encode(_buffer.AsSpan(lenOffset - tagLen, tagLen));
                    // Continue with the regular flow.
                }
                else
                {
                    int fullSegments = Math.DivRem(
                        containedLength,
                        1000,
                        out int lastSegmentSize);

                    int requiredPadding =
                        // Each full segment has a header of 048203E8
                        4 * fullSegments +
                        // The last one is 04 plus the encoded length.
                        2 + GetEncodedLengthSubsequentByteCount(lastSegmentSize);

                    // Shift the data forward so we can use right-source-overlapped
                    // copy in the existing method.
                    // Also, ensure the space for the end-of-contents marker.
                    EnsureWriteCapacity(requiredPadding + 2);
                    ReadOnlySpan<byte> src = _buffer.AsSpan(start, containedLength);
                    Span<byte> dest = _buffer.AsSpan(start + requiredPadding, containedLength);
                    src.CopyTo(dest);

                    int expectedEnd = start + containedLength + requiredPadding + 2;
                    _offset = lenOffset - tag.CalculateEncodedSize();
                    WriteConstructedCerOctetString(tag, dest);
                    Debug.Assert(_offset == expectedEnd);
                    return;
                }
            }

            int shiftSize = GetEncodedLengthSubsequentByteCount(containedLength);

            // Best case, length fits in the compact byte
            if (shiftSize == 0)
            {
                _buffer[lenOffset] = (byte)containedLength;
                return;
            }

            // We're currently at the end, so ensure we have room for N more bytes.
            EnsureWriteCapacity(shiftSize);

            // Buffer.BlockCopy correctly does forward-overlapped, so use it.
            Buffer.BlockCopy(_buffer, start, _buffer, start + shiftSize, containedLength);

            int tmp = _offset;
            _offset = lenOffset;
            WriteLength(containedLength);
            Debug.Assert(_offset - lenOffset - 1 == shiftSize);
            _offset = tmp + shiftSize;
        }

        private void WriteConstructedCerOctetString(Asn1Tag tag, ReadOnlySpan<byte> payload)
        {
            const int MaxCERSegmentSize = 1000;
            Debug.Assert(payload.Length > MaxCERSegmentSize);

            WriteTag(tag.AsConstructed());
            WriteLength(-1);

            int fullSegments = Math.DivRem(payload.Length, MaxCERSegmentSize, out int lastSegmentSize);

            // The tag size is 1 byte.
            // The length will always be encoded as 82 03 E8 (3 bytes)
            // And 1000 content octets (by T-REC-X.690-201508 sec 9.2)
            const int FullSegmentEncodedSize = 1004;
            Debug.Assert(
                FullSegmentEncodedSize == 1 + 1 + MaxCERSegmentSize + GetEncodedLengthSubsequentByteCount(MaxCERSegmentSize));

            int remainingEncodedSize;

            if (lastSegmentSize == 0)
            {
                remainingEncodedSize = 0;
            }
            else
            {
                // One byte of tag, and minimum one byte of length.
                remainingEncodedSize = 2 + lastSegmentSize + GetEncodedLengthSubsequentByteCount(lastSegmentSize);
            }

            // Reduce the number of copies by pre-calculating the size.
            // +2 for End-Of-Contents
            int expectedSize = fullSegments * FullSegmentEncodedSize + remainingEncodedSize + 2;
            EnsureWriteCapacity(expectedSize);

            byte[] ensureNoExtraCopy = _buffer;
            int savedOffset = _offset;

            ReadOnlySpan<byte> remainingData = payload;
            Span<byte> dest;
            Asn1Tag primitiveOctetString = Asn1Tag.PrimitiveOctetString;

            while (remainingData.Length > MaxCERSegmentSize)
            {
                // T-REC-X.690-201508 sec 8.7.3.2-note2
                WriteTag(primitiveOctetString);
                WriteLength(MaxCERSegmentSize);

                dest = _buffer.AsSpan(_offset);
                remainingData.Slice(0, MaxCERSegmentSize).CopyTo(dest);

                _offset += MaxCERSegmentSize;
                remainingData = remainingData.Slice(MaxCERSegmentSize);
            }

            WriteTag(primitiveOctetString);
            WriteLength(remainingData.Length);
            dest = _buffer.AsSpan(_offset);
            remainingData.CopyTo(dest);
            _offset += remainingData.Length;

            WriteEndOfContents();

            Debug.Assert(_offset - savedOffset == expectedSize, $"expected size was {expectedSize}, actual was {_offset - savedOffset}");
            Debug.Assert(_buffer == ensureNoExtraCopy, $"_buffer was replaced during {nameof(WriteConstructedCerOctetString)}");
        }

        private void WriteEndOfContents()
        {
            EnsureWriteCapacity(2);
            _buffer[_offset++] = 0;
            _buffer[_offset++] = 0;
        }


        private static void SortContents(byte[] buffer, int start, int end)
        {
            Debug.Assert(buffer != null);
            Debug.Assert(end >= start);

            int len = end - start;

            if (len == 0)
            {
                return;
            }

            // Since BER can read everything and the reader does not mutate data
            // just use a BER reader for identifying the positions of the values
            // within this memory segment.
            //
            // Since it's not mutating, any restrictions imposed by CER or DER will
            // still be maintained.
            var reader = new AsnReader(new ReadOnlyMemory<byte>(buffer, start, len), AsnEncodingRules.BER);

            List<(int, int)> positions = new List<(int, int)>();

            int pos = start;

            while (reader.HasData)
            {
                ReadOnlyMemory<byte> encoded = reader.ReadEncodedValue();
                positions.Add((pos, encoded.Length));
                pos += encoded.Length;
            }

            Debug.Assert(pos == end);

            var comparer = new ArrayIndexSetOfValueComparer(buffer);
            positions.Sort(comparer);

            byte[] tmp = CryptoPool.Rent(len);

            pos = 0;

            foreach ((int offset, int length) in positions)
            {
                Buffer.BlockCopy(buffer, offset, tmp, pos, length);
                pos += length;
            }

            Debug.Assert(pos == len);

            Buffer.BlockCopy(tmp, 0, buffer, start, len);
            CryptoPool.Return(tmp, len);
        }

        private void EnsureWriteCapacity(int pendingCount)
        {
            if (pendingCount < 0)
            {
                throw new OverflowException();
            }

            if (_buffer == null || _buffer.Length - _offset < pendingCount)
            {
#if CHECK_ACCURATE_ENSURE
                // A debug paradigm to make sure that throughout the execution nothing ever writes
                // past where the buffer was "allocated".  This causes quite a number of reallocs
                // and copies, so it's a #define opt-in.
                byte[] newBytes = new byte[_offset + pendingCount];

                if (_buffer != null)
                {
                    Span<byte> bufferSpan = _buffer.AsSpan(0, _offset);
                    bufferSpan.CopyTo(newBytes);
                    bufferSpan.Clear();
                }

                _buffer = newBytes;
#else
                const int BlockSize = 1024;
                // Make sure we don't run into a lot of "grow a little" by asking in 1k steps.
                int blocks = checked(_offset + pendingCount + (BlockSize - 1)) / BlockSize;
                byte[]? oldBytes = _buffer;
                Array.Resize(ref _buffer, BlockSize * blocks);

                if (oldBytes != null)
                {
                    oldBytes.AsSpan(0, _offset).Clear();
                }
#endif

#if DEBUG
                // Ensure no "implicit 0" is happening, in case we move to pooling.
                _buffer.AsSpan(_offset).Fill(0xCA); //202
#endif
            }
        }

        private Scope PushSequenceCore(Asn1Tag tag)
        {
            Debug.Assert(tag.IsConstructed);
            return PushTag(tag, UniversalTagNumber.Sequence);
        }

        private Scope PushTag(Asn1Tag tag, UniversalTagNumber tagType)
        {
            if (_nestingStack == null)
            {
                _nestingStack = new Stack<StackFrame>();
            }

            Debug.Assert(tag.IsConstructed);
            WriteTag(tag);
            _nestingStack.Push(new StackFrame(tag, _offset, tagType));
            // Indicate that the length is indefinite.
            // We'll come back and clean this up (as appropriate) in PopTag.
            WriteLength(-1);
            return new Scope(this);
        }

        private void WriteTag(Asn1Tag tag)
        {
            int spaceRequired = tag.CalculateEncodedSize();
            EnsureWriteCapacity(spaceRequired);

            if (!tag.TryEncode(_buffer.AsSpan(_offset, spaceRequired), out int written) ||
                written != spaceRequired)
            {
                Debug.Fail($"TryWrite failed or written was wrong value ({written} vs {spaceRequired})");
                throw new InvalidOperationException();
            }

            _offset += spaceRequired;
        }

        private void WriteLength(int length)
        {
            const byte MultiByteMarker = 0x80;
            Debug.Assert(length >= -1);

            // If the indefinite form has been requested.
            // T-REC-X.690-201508 sec 8.1.3.6
            if (length == -1)
            {
                EnsureWriteCapacity(1);
                _buffer[_offset] = MultiByteMarker;
                _offset++;
                return;
            }

            Debug.Assert(length >= 0);

            // T-REC-X.690-201508 sec 8.1.3.3, 8.1.3.4
            if (length < MultiByteMarker)
            {
                EnsureWriteCapacity(1 + length);
                // Pre-allocate the pending data since we know how much.
                _buffer[_offset] = (byte)length;
                _offset++;
                return;
            }

            // The rest of the method implements T-REC-X.680-201508 sec 8.1.3.5
            int lengthLength = GetEncodedLengthSubsequentByteCount(length);

            // Pre-allocate the pending data since we know how much.
            EnsureWriteCapacity(lengthLength + 1 + length);
            _buffer[_offset] = (byte)(MultiByteMarker | lengthLength);

            // No minus one because offset didn't get incremented yet.
            int idx = _offset + lengthLength;

            int remaining = length;

            do
            {
                _buffer[idx] = (byte)remaining;
                remaining >>= 8;
                idx--;
            } while (remaining > 0);

            Debug.Assert(idx == _offset);
            _offset += lengthLength + 1;
        }

        private static int GetEncodedLengthSubsequentByteCount(int length)
        {
            if (length < 0)
                throw new OverflowException();
            if (length <= 0x7F)
                return 0;
            if (length <= byte.MaxValue)
                return 1;
            if (length <= ushort.MaxValue)
                return 2;
            if (length <= 0x00FFFFFF)
                return 3;

            return 4;
        }

        private sealed class ArrayIndexSetOfValueComparer : IComparer<(int, int)>
        {
            private readonly byte[] _data;

            public ArrayIndexSetOfValueComparer(byte[] data)
            {
                _data = data;
            }

            public int Compare((int, int) x, (int, int) y)
            {
                (int xOffset, int xLength) = x;
                (int yOffset, int yLength) = y;

                int value =
                    SetOfValueComparer.Instance.Compare(
                        new ReadOnlyMemory<byte>(_data, xOffset, xLength),
                        new ReadOnlyMemory<byte>(_data, yOffset, yLength));

                if (value == 0)
                {
                    // Whichever had the lowest index wins (once sorted, stay sorted)
                    return xOffset - yOffset;
                }

                return value;
            }
        }

        private readonly struct StackFrame : IEquatable<StackFrame>
        {
            public Asn1Tag Tag { get; }
            public int Offset { get; }
            public UniversalTagNumber ItemType { get; }

            internal StackFrame(Asn1Tag tag, int offset, UniversalTagNumber itemType)
            {
                Tag = tag;
                Offset = offset;
                ItemType = itemType;
            }

            public void Deconstruct(out Asn1Tag tag, out int offset, out UniversalTagNumber itemType)
            {
                tag = Tag;
                offset = Offset;
                itemType = ItemType;
            }

            public bool Equals(StackFrame other)
            {
                return Tag.Equals(other.Tag) && Offset == other.Offset && ItemType == other.ItemType;
            }

            public override bool Equals([NotNullWhen(true)] object? obj) => obj is StackFrame other && Equals(other);

            public override int GetHashCode()
            {
                return (Tag, Offset, ItemType).GetHashCode();
            }

            public static bool operator ==(StackFrame left, StackFrame right) => left.Equals(right);

            public static bool operator !=(StackFrame left, StackFrame right) => !left.Equals(right);
        }

        public readonly struct Scope : IDisposable
        {
            private readonly AsnWriter _writer;
            private readonly StackFrame _frame;
            private readonly int _depth;

            internal Scope(AsnWriter writer)
            {
                Debug.Assert(writer._nestingStack != null);

                _writer = writer;
                _frame = _writer._nestingStack.Peek();
                _depth = _writer._nestingStack.Count;
            }

            public void Dispose()
            {
                //Debug.Assert(_writer == null || _writer._nestingStack != null);

                //if (_writer == null || _writer._nestingStack!.Count == 0)
                //{
                //    return;
                //}

                //if (_writer._nestingStack.Peek() == _frame)
                //{
                //    switch (_frame.ItemType)
                //    {
                //        case UniversalTagNumber.SetOf:
                //            _writer.PopSetOf(_frame.Tag);
                //            break;
                //        case UniversalTagNumber.Sequence:
                //            _writer.PopSequence(_frame.Tag);
                //            break;
                //        case UniversalTagNumber.OctetString:
                //            _writer.PopOctetString(_frame.Tag);
                //            break;
                //        default:
                //            Debug.Fail($"No handler for {_frame.ItemType}");
                //            throw new InvalidOperationException();
                //    }
                //}
                //else if (_writer._nestingStack.Count > _depth &&
                //    _writer._nestingStack.Contains(_frame))
                //{
                //    // Another frame was pushed when we got disposed.
                //    // Report the imbalance.
                //    throw new InvalidOperationException(SR.AsnWriter_PopWrongTag);
                //}
            }
        }
    }

    public readonly partial struct Asn1Tag : IEquatable<Asn1Tag>
    {
        private const byte ClassMask = 0b1100_0000; // 192
        private const byte ConstructedMask = 0b0010_0000; // 32
        private const byte ControlMask = ClassMask | ConstructedMask; // 224
        private const byte TagNumberMask = 0b0001_1111; // 31

        private readonly byte _controlFlags;

        /// <summary>
        ///   Represents the End-of-Contents meta-tag.
        /// </summary>
        internal static readonly Asn1Tag EndOfContents = new Asn1Tag(0, (int)UniversalTagNumber.EndOfContents);

        /// <summary>
        ///   Represents the universal class tag for a Boolean value.
        /// </summary>
        public static readonly Asn1Tag Boolean = new Asn1Tag(0, (int)UniversalTagNumber.Boolean);

        /// <summary>
        ///   Represents the universal class tag for an Integer value.
        /// </summary>
        public static readonly Asn1Tag Integer = new Asn1Tag(0, (int)UniversalTagNumber.Integer);

        /// <summary>
        ///   Represents the universal class tag for a Bit String value under a primitive encoding.
        /// </summary>
        public static readonly Asn1Tag PrimitiveBitString = new Asn1Tag(0, (int)UniversalTagNumber.BitString);

        /// <summary>
        ///   表示构造的编码下的位字符串值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag ConstructedBitString = new Asn1Tag(ConstructedMask, (int)UniversalTagNumber.BitString);

        /// <summary>
        ///   表示基元编码下的Octet String值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag PrimitiveOctetString = new Asn1Tag(0, (int)UniversalTagNumber.OctetString);

        /// <summary>
        ///   表示构造的编码下的Octet String值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag ConstructedOctetString =
            new Asn1Tag(ConstructedMask, (int)UniversalTagNumber.OctetString);

        /// <summary>
        ///   表示Null值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag Null = new Asn1Tag(0, (int)UniversalTagNumber.Null);

        /// <summary>
        ///   表示对象标识符值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag ObjectIdentifier = new Asn1Tag(0, (int)UniversalTagNumber.ObjectIdentifier);

        /// <summary>
        ///   表示枚举值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag Enumerated = new Asn1Tag(0, (int)UniversalTagNumber.Enumerated);

        /// <summary>
        ///   表示Sequence值的通用类标记(总是构造的编码)。
        /// </summary>
        public static readonly Asn1Tag Sequence = new Asn1Tag(ConstructedMask, (int)UniversalTagNumber.Sequence);

        /// <summary>
        ///   表示SetOf值的通用类标记(总是构造的编码)。
        /// </summary>
        public static readonly Asn1Tag SetOf = new Asn1Tag(ConstructedMask, (int)UniversalTagNumber.SetOf);

        /// <summary>
        ///   表示UtcTime值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag UtcTime = new Asn1Tag(0, (int)UniversalTagNumber.UtcTime);

        /// <summary>
        ///   表示GeneralizedTime值的通用类标记。
        /// </summary>
        public static readonly Asn1Tag GeneralizedTime = new Asn1Tag(0, (int)UniversalTagNumber.GeneralizedTime);

        /// <summary>
        ///   这个标记所属的标记类。
        /// </summary>
        public TagClass TagClass => (TagClass)(_controlFlags & ClassMask);

        /// <summary>
        ///   指示标记是否表示构造的编码 (<see langword="true"/>), 或者是原始编码 (<see langword="false"/>).
        /// </summary>
        public bool IsConstructed => (_controlFlags & ConstructedMask) != 0;

        /// <summary>
        ///   The numeric value for this tag.
        /// </summary>
        /// <remarks>
        ///   If <see cref="TagClass"/> is <see cref="Asn1.TagClass.Universal"/>, this value can
        ///   be interpreted as a <see cref="UniversalTagNumber"/>.
        /// </remarks>
        public int TagValue { get; }

        private Asn1Tag(byte controlFlags, int tagValue)
        {
            _controlFlags = (byte)(controlFlags & ControlMask);
            TagValue = tagValue;
        }

        /// <summary>
        ///   Create an <see cref="Asn1Tag"/> for a tag from the UNIVERSAL class.
        /// </summary>
        /// <param name="universalTagNumber">
        ///   One of the enumeration values that specifies the semantic type for this tag.
        /// </param>
        /// <param name="isConstructed">
        ///   <see langword="true"/> for a constructed tag, <see langword="false"/> for a primitive tag.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="universalTagNumber"/> is not a known value.
        /// </exception>
        public Asn1Tag(UniversalTagNumber universalTagNumber, bool isConstructed = false)
            : this(isConstructed ? ConstructedMask : (byte)0, (int)universalTagNumber)
        {
            // T-REC-X.680-201508 sec 8.6 (Table 1)
            const UniversalTagNumber ReservedIndex = (UniversalTagNumber)15;

            if (universalTagNumber < UniversalTagNumber.EndOfContents ||
                universalTagNumber > UniversalTagNumber.RelativeObjectIdentifierIRI ||
                universalTagNumber == ReservedIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(universalTagNumber));
            }
        }

        /// <summary>
        ///   Create an <see cref="Asn1Tag"/> for a specified value within a specified tag class.
        /// </summary>
        /// <param name="tagClass">
        ///   The tag class for this tag.
        /// </param>
        /// <param name="tagValue">
        ///   The numeric value for this tag.
        /// </param>
        /// <param name="isConstructed">
        ///   <see langword="true"/> for a constructed tag, <see langword="false"/> for a primitive tag.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="tagClass"/> is not a known value.
        ///
        ///   -or-
        ///
        ///   <paramref name="tagValue" /> is negative.
        /// </exception>
        /// <remarks>
        ///   This constructor allows for the creation undefined UNIVERSAL class tags.
        /// </remarks>
        public Asn1Tag(TagClass tagClass, int tagValue, bool isConstructed = false)
            : this((byte)((byte)tagClass | (isConstructed ? ConstructedMask : 0)), tagValue)
        {
            switch (tagClass)
            {
                case TagClass.Universal:
                case TagClass.ContextSpecific:
                case TagClass.Application:
                case TagClass.Private:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tagClass));
            }

            if (tagValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tagValue));
            }
        }

        /// <summary>
        ///   Produces a tag with the same <see cref="TagClass"/> and
        ///   <see cref="TagValue"/> values, but whose <see cref="IsConstructed"/> is <see langword="true"/>.
        /// </summary>
        /// <returns>
        ///   A tag with the same <see cref="TagClass"/> and <see cref="TagValue"/>
        ///   values, but whose <see cref="IsConstructed"/> is <see langword="true"/>.
        /// </returns>
        public Asn1Tag AsConstructed()
        {
            return new Asn1Tag((byte)(_controlFlags | ConstructedMask), TagValue);
        }

        /// <summary>
        ///   Produces a tag with the same <see cref="TagClass"/> and
        ///   <see cref="TagValue"/> values, but whose <see cref="IsConstructed"/> is <see langword="false"/>.
        /// </summary>
        /// <returns>
        ///   A tag with the same <see cref="TagClass"/> and <see cref="TagValue"/>
        ///   values, but whose <see cref="IsConstructed"/> is <see langword="false"/>.
        /// </returns>
        public Asn1Tag AsPrimitive()
        {
            return new Asn1Tag((byte)(_controlFlags & ~ConstructedMask), TagValue);
        }

        /// <summary>
        ///   Attempts to read a BER-encoded tag which starts at <paramref name="source"/>.
        /// </summary>
        /// <param name="source">
        ///   The read only byte sequence whose beginning is a BER-encoded tag.
        /// </param>
        /// <param name="tag">
        ///   The decoded tag.
        /// </param>
        /// <param name="bytesConsumed">
        ///   When this method returns, contains the number of bytes that contributed
        ///   to the encoded tag, 0 on failure. This parameter is treated as uninitialized.
        /// </param>
        /// <returns>
        ///   <see langword="true" /> if a tag was correctly decoded; otherwise, <see langword="false" />.
        /// </returns>
        public static bool TryDecode(ReadOnlySpan<byte> source, out Asn1Tag tag, out int bytesConsumed)
        {
            tag = default(Asn1Tag);
            bytesConsumed = 0;

            if (source.IsEmpty)
            {
                return false;
            }

            byte first = source[bytesConsumed];
            bytesConsumed++;
            uint tagValue = (uint)(first & TagNumberMask);

            if (tagValue == TagNumberMask)
            {
                // Multi-byte encoding
                // T-REC-X.690-201508 sec 8.1.2.4
                const byte ContinuationFlag = 0x80;
                const byte ValueMask = ContinuationFlag - 1;

                tagValue = 0;
                byte current;

                do
                {
                    if (source.Length <= bytesConsumed)
                    {
                        bytesConsumed = 0;
                        return false;
                    }

                    current = source[bytesConsumed];
                    byte currentValue = (byte)(current & ValueMask);
                    bytesConsumed++;

                    // If TooBigToShift is shifted left 7, the content bit shifts out.
                    // So any value greater than or equal to this cannot be shifted without loss.
                    const int TooBigToShift = 0b00000010_00000000_00000000_00000000;

                    if (tagValue >= TooBigToShift)
                    {
                        bytesConsumed = 0;
                        return false;
                    }

                    tagValue <<= 7;
                    tagValue |= currentValue;

                    // The first byte cannot have the value 0 (T-REC-X.690-201508 sec 8.1.2.4.2.c)
                    if (tagValue == 0)
                    {
                        bytesConsumed = 0;
                        return false;
                    }
                }
                while ((current & ContinuationFlag) == ContinuationFlag);

                // This encoding is only valid for tag values greater than 30.
                // (T-REC-X.690-201508 sec 8.1.2.3, 8.1.2.4)
                if (tagValue <= 30)
                {
                    bytesConsumed = 0;
                    return false;
                }

                // There's not really any ambiguity, but prevent negative numbers from showing up.
                if (tagValue > int.MaxValue)
                {
                    bytesConsumed = 0;
                    return false;
                }
            }

            Debug.Assert(bytesConsumed > 0);
            tag = new Asn1Tag(first, (int)tagValue);
            return true;
        }

        /// <summary>
        ///   Reads a BER-encoded tag which starts at <paramref name="source"/>.
        /// </summary>
        /// <param name="source">
        ///   The read only byte sequence whose beginning is a BER-encoded tag.
        /// </param>
        /// <param name="bytesConsumed">
        ///   When this method returns, contains the number of bytes that contributed
        ///   to the encoded tag. This parameter is treated as uninitialized.
        /// </param>
        /// <returns>
        ///   The decoded tag.
        /// </returns>
        /// <exception cref="AsnContentException">
        ///   The provided data does not decode to a tag.
        /// </exception>
        public static Asn1Tag Decode(ReadOnlySpan<byte> source, out int bytesConsumed)
        {
            if (TryDecode(source, out Asn1Tag tag, out bytesConsumed))
            {
                return tag;
            }

            throw new Exception();
        }

        /// <summary>
        ///   报告此标记的ber编码所需的字节数。
        /// </summary>
        /// <returns>
        ///   The number of bytes required for the BER-encoding of this tag.
        /// </returns>
        /// <seealso cref="TryEncode(Span{byte},out int)"/>
        public int CalculateEncodedSize()
        {
            const int SevenBits = 0b0111_1111; // 127
            const int FourteenBits = 0b0011_1111_1111_1111; // 16383
            const int TwentyOneBits = 0b0001_1111_1111_1111_1111_1111; // 2097151
            const int TwentyEightBits = 0b0000_1111_1111_1111_1111_1111_1111_1111; // 268435455

            if (TagValue < TagNumberMask)
                return 1;
            if (TagValue <= SevenBits)
                return 2;
            if (TagValue <= FourteenBits)
                return 3;
            if (TagValue <= TwentyOneBits)
                return 4;
            if (TagValue <= TwentyEightBits)
                return 5;

            return 6;
        }

        /// <summary>
        ///   尝试将此标记的ber编码形式写入<paramref name="destination"/>.
        /// </summary>
        /// <param name="destination">
        ///   The start of where the encoded tag should be written.
        /// </param>
        /// <param name="bytesWritten">
        ///   Receives the value from <see cref="CalculateEncodedSize"/> on success, 0 on failure.
        /// </param>
        /// <returns>
        ///   <see langword="false"/> if <paramref name="destination"/>.<see cref="Span{T}.Length"/> &lt;
        ///   <see cref="CalculateEncodedSize"/>(), <see langword="true"/> otherwise.
        /// </returns>
        public bool TryEncode(Span<byte> destination, out int bytesWritten)
        {
            int spaceRequired = CalculateEncodedSize();

            if (destination.Length < spaceRequired)
            {
                bytesWritten = 0;
                return false;
            }

            if (spaceRequired == 1)
            {
                byte value = (byte)(_controlFlags | TagValue);
                destination[0] = value;
                bytesWritten = 1;
                return true;
            }

            byte firstByte = (byte)(_controlFlags | TagNumberMask);
            destination[0] = firstByte;

            int remaining = TagValue;
            int idx = spaceRequired - 1;

            while (remaining > 0)
            {
                int segment = remaining & 0x7F; // 检测TagValue是否超过127

                // 最后一个字节没有得到标记，这是我们先写的。
                if (remaining != TagValue)
                {
                    segment |= 0x80;
                }

                Debug.Assert(segment <= byte.MaxValue);
                destination[idx] = (byte)segment;
                remaining >>= 7;
                idx--;
            }

            Debug.Assert(idx == 0);
            bytesWritten = spaceRequired;
            return true;
        }

        /// <summary>
        ///   Writes the BER-encoded form of this tag to <paramref name="destination"/>.
        /// </summary>
        /// <param name="destination">
        ///   The start of where the encoded tag should be written.
        /// </param>
        /// <returns>
        ///   The number of bytes written to <paramref name="destination"/>.
        /// </returns>
        /// <seealso cref="CalculateEncodedSize"/>
        /// <exception cref="ArgumentException">
        ///   <paramref name="destination"/>.<see cref="Span{T}.Length"/> &lt; <see cref="CalculateEncodedSize"/>.
        /// </exception>
        public int Encode(Span<byte> destination)
        {
            if (TryEncode(destination, out int bytesWritten))
            {
                return bytesWritten;
            }

            throw new Exception();
        }

        /// <summary>
        ///   Tests if <paramref name="other"/> has the same encoding as this tag.
        /// </summary>
        /// <param name="other">
        ///   Tag to test for equality.
        /// </param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="other"/> has the same values for
        ///   <see cref="TagClass"/>, <see cref="TagValue"/>, and <see cref="IsConstructed"/>;
        ///   <see langword="false"/> otherwise.
        /// </returns>
        public bool Equals(Asn1Tag other)
        {
            return _controlFlags == other._controlFlags && TagValue == other.TagValue;
        }

        /// <summary>
        ///   Tests if <paramref name="obj"/> is an <see cref="Asn1Tag"/> with the same
        ///   encoding as this tag.
        /// </summary>
        /// <param name="obj">Object to test for value equality</param>
        /// <returns>
        ///   <see langword="false"/> if <paramref name="obj"/> is not an <see cref="Asn1Tag"/>,
        ///   <see cref="Equals(Asn1Tag)"/> otherwise.
        /// </returns>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Asn1Tag tag && Equals(tag);
        }

        /// <summary>
        ///   Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            // Most TagValue values will be in the 0-30 range,
            // the GetHashCode value only has collisions when TagValue is
            // between 2^29 and uint.MaxValue
            return (_controlFlags << 24) ^ TagValue;
        }

        /// <summary>
        ///   Tests if two <see cref="Asn1Tag"/> values have the same BER encoding.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have the same
        ///   BER encoding, <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator ==(Asn1Tag left, Asn1Tag right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///   Tests if two <see cref="Asn1Tag"/> values have a different BER encoding.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have a different
        ///   BER encoding, <see langword="false"/> otherwise.
        /// </returns>
        public static bool operator !=(Asn1Tag left, Asn1Tag right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        ///   Tests if <paramref name="other"/> has the same <see cref="TagClass"/> and <see cref="TagValue"/>
        ///   values as this tag, and does not compare <see cref="IsConstructed"/>.
        /// </summary>
        /// <param name="other">Tag to test for concept equality.</param>
        /// <returns>
        ///   <see langword="true"/> if <paramref name="other"/> has the same <see cref="TagClass"/> and <see cref="TagValue"/>
        ///   as this tag, <see langword="false"/> otherwise.
        /// </returns>
        public bool HasSameClassAndValue(Asn1Tag other)
        {
            return TagValue == other.TagValue && TagClass == other.TagClass;
        }

        /// <summary>
        ///   Provides a text representation of this tag suitable for debugging.
        /// </summary>
        /// <returns>
        ///   A text representation of this tag suitable for debugging.
        /// </returns>
        public override string ToString()
        {
            const string ConstructedPrefix = "Constructed ";
            string classAndValue;

            if (TagClass == TagClass.Universal)
            {
                classAndValue = ((UniversalTagNumber)TagValue).ToString();
            }
            else
            {
                classAndValue = TagClass + "-" + TagValue;
            }

            if (IsConstructed)
            {
                return ConstructedPrefix + classAndValue;
            }

            return classAndValue;
        }
    }
}
