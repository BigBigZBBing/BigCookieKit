using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BigCookieKit.Algorithm
{
    public partial class RSAProvider<T>
    {
        #region 解析T-REC-X.690-201508

        private byte[] ExportPublicKeyTest()
        {
            List<byte> bytes = new List<byte>();
            WriteTag(bytes, 16); //Sequence
            WriteLength(bytes, -1);
            WriteTagAndIntegerUnsigned(bytes, publickKey.Modulus);
            WriteTagAndIntegerUnsigned(bytes, publickKey.Exponent);
            PopSequence(bytes);
            return bytes.ToArray();
        }

        private byte[] ExportPrivateKeyTest(bool isPKCS8 = false, string password = null)
        {
            List<byte> bytes = new List<byte>();
            WriteTag(bytes, 16); //Sequence
            WriteLength(bytes, -1);
            WriteTagAndIntegerUnsigned(bytes, 0);
            WriteTagAndIntegerUnsigned(bytes, privateKey.Modulus);
            WriteTagAndIntegerUnsigned(bytes, privateKey.Exponent);
            WriteTagAndIntegerUnsigned(bytes, privateKey.D);
            WriteTagAndIntegerUnsigned(bytes, privateKey.P);
            WriteTagAndIntegerUnsigned(bytes, privateKey.Q);
            WriteTagAndIntegerUnsigned(bytes, privateKey.DP);
            WriteTagAndIntegerUnsigned(bytes, privateKey.DQ);
            WriteTagAndIntegerUnsigned(bytes, privateKey.InverseQ);
            PopSequence(bytes);
            return bytes.ToArray();
        }

        private void PopSequence(List<byte> bytes)
        {
            int containedLength = bytes.Count - 2;
            var shiftSize = GetEncodedLengthSubsequentByteCount(containedLength);
            for (int i = 0; i < shiftSize; i++) bytes.Add(0);
            byte[] dest = bytes.ToArray();
            Buffer.BlockCopy(dest, 2, dest, 2 + shiftSize, containedLength);
            bytes.Clear();
            bytes.AddRange(dest);
            WriteOffsetLength(bytes, containedLength, 1);
        }

        private void WriteTagAndIntegerUnsigned(List<byte> bytes, params byte[] value)
        {
            // Integer标识
            bytes.Add(2);
            // 写入长度
            if (value[0] >= 0x80)
            {
                WriteLength(bytes, value.Length + 1);
                bytes.Add(0);
            }
            else
            {
                WriteLength(bytes, value.Length);
            }
            // 写入内容
            bytes.AddRange(value);
        }

        private void WriteLength(List<byte> bytes, int length)
        {
            if (length == -1)
            {
                bytes.Add(0X80);
                return;
            }

            if (length >= 0x80)
            {
                var lengthLength = GetEncodedLengthSubsequentByteCount(length);
                bytes.Add((byte)(0x80 | lengthLength));
                var remaining = length;
                do
                {
                    bytes.Add((byte)remaining);
                    remaining >>= 8;
                } while (remaining > 0);
            }
            else bytes.Add((byte)length);
        }

        private void WriteOffsetLength(List<byte> bytes, int length, int offset)
        {
            if (length >= 0x80)
            {
                var lengthLength = GetEncodedLengthSubsequentByteCount(length);
                int idx = offset + lengthLength;
                bytes[offset] = (byte)(0x80 | lengthLength);
                var remaining = length;
                do
                {
                    bytes[idx] = (byte)remaining;
                    remaining >>= 8;
                    idx--;
                } while (remaining > 0);
            }
            else bytes.Add((byte)length);
        }

        private void WriteTag(List<byte> bytes, int tagValue)
        {
            var spaceRequired = BarEncodeSize(tagValue);

            if (spaceRequired == 1)
            {
                bytes.Add((byte)(32 | tagValue));
                return;
            }
            else
            {
                bytes.Add((byte)(32 | tagValue));

                int count = bytes.Count;
                int remaining = tagValue;

                while (remaining > 0)
                {
                    int segment = remaining & 0x7F; // 超过127则是0

                    if (count != bytes.Count)
                    {
                        bytes[--count] = (byte)segment;
                    }
                    else
                    {
                        bytes.Add((byte)segment);
                    }
                    remaining >>= 7; // 推7位:大于127才会大于0
                }
            }
        }

        private int BarEncodeSize(int tagValue)
        {
            if (tagValue < 31)
            {
                return 1;
            }
            else return 2;
        }

        private int GetEncodedLengthSubsequentByteCount(int value_len)
        {
            if (value_len < 0)
                throw new OverflowException();
            if (value_len <= 0x7F)
                return 0;
            if (value_len <= byte.MaxValue)
                return 1;
            if (value_len <= ushort.MaxValue)
                return 2;
            if (value_len <= 0x00FFFFFF)
                return 3;

            return 4;
        }

        #endregion

        private RSAParameters getKeyPara(string hashKey, int type)
        {
            RSAParameters rsaP = new RSAParameters();
            byte[] tmpKeyNoB64 = Convert.FromBase64String(hashKey);
            int pemModulus = 128;
            int pemPublicExponent = 3;
            int pemPrivateExponent = 128;
            int pemPrime1 = 64;
            int pemPrime2 = 64;
            int pemExponent1 = 64;
            int pemExponent2 = 64;
            int pemCoefficient = 64;
            byte[] arrPemModulus = new byte[128];
            byte[] arrPemPublicExponent = new byte[3];
            byte[] arrPemPrivateExponent = new byte[128];
            byte[] arrPemPrime1 = new byte[64];
            byte[] arrPemPrime2 = new byte[64];
            byte[] arrPemExponent1 = new byte[64];
            byte[] arrPemExponent2 = new byte[64];
            byte[] arrPemCoefficient = new byte[64];

            if (type == 0)//私钥
            {
                //Modulus
                for (int i = 0; i < pemModulus; i++)
                {
                    arrPemModulus[i] = tmpKeyNoB64[11 + i];
                }
                rsaP.Modulus = arrPemModulus;

                //PublicExponent
                for (int i = 0; i < pemPublicExponent; i++)
                {
                    arrPemPublicExponent[i] = tmpKeyNoB64[141 + i];
                }
                rsaP.Exponent = arrPemPublicExponent;

                //PrivateExponent
                for (int i = 0; i < pemPrivateExponent; i++)
                {
                    arrPemPrivateExponent[i] = tmpKeyNoB64[147 + i];
                }
                rsaP.D = arrPemPrivateExponent;

                //Prime1
                for (int i = 0; i < pemPrime1; i++)
                {
                    arrPemPrime1[i] = tmpKeyNoB64[278 + i];
                }
                rsaP.P = arrPemPrime1;

                //Prime2
                for (int i = 0; i < pemPrime2; i++)
                {
                    arrPemPrime2[i] = tmpKeyNoB64[345 + i];
                }
                rsaP.Q = arrPemPrime2;

                //Exponent1
                for (int i = 0; i < pemExponent1; i++)
                {
                    arrPemExponent1[i] = tmpKeyNoB64[412 + i];
                }
                rsaP.DP = arrPemExponent1;

                //Exponent2
                for (int i = 0; i < pemExponent2; i++)
                {
                    arrPemExponent2[i] = tmpKeyNoB64[478 + i];
                }
                rsaP.DQ = arrPemExponent2;

                //Coefficient
                for (int i = 0; i < pemCoefficient; i++)
                {
                    arrPemCoefficient[i] = tmpKeyNoB64[545 + i];
                }
                rsaP.InverseQ = arrPemCoefficient;
            }
            else//公钥
            {
                for (int i = 0; i < pemModulus; i++)
                {
                    arrPemModulus[i] = tmpKeyNoB64[29 + i];
                }
                rsaP.Modulus = arrPemModulus;

                for (int i = 0; i < pemPublicExponent; i++)
                {
                    arrPemPublicExponent[i] = tmpKeyNoB64[159 + i];
                }
                rsaP.Exponent = arrPemPublicExponent;
            }

            return rsaP;
        }
    }
}
