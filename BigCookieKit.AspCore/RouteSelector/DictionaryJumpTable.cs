﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BigCookieKit.AspCore.RouteSelector
{
    internal class DictionaryJumpTable : JumpTable
    {
        private readonly int _defaultDestination;
        private readonly int _exitDestination;
        private readonly Dictionary<string, int> _dictionary;

        public DictionaryJumpTable(
            int defaultDestination,
            int exitDestination,
            (string text, int destination)[] entries)
        {
            _defaultDestination = defaultDestination;
            _exitDestination = exitDestination;

            _dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < entries.Length; i++)
            {
                _dictionary.Add(entries[i].text, entries[i].destination);
            }
        }

        public override int GetDestination(string path, PathSegment segment)
        {
            if (segment.Length == 0)
            {
                return _exitDestination;
            }

            var text = path.Substring(segment.Start, segment.Length);
            if (_dictionary.TryGetValue(text, out var destination))
            {
                return destination;
            }

            return _defaultDestination;
        }

        public override string DebuggerToString()
        {
            var builder = new StringBuilder();
            builder.Append("{ ");

            builder.AppendJoin(", ", _dictionary.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

            builder.Append("$+: ");
            builder.Append(_defaultDestination);
            builder.Append(", ");

            builder.Append("$0: ");
            builder.Append(_defaultDestination);

            builder.Append(" }");

            return builder.ToString();
        }
    }
}
