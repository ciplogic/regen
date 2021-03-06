﻿using System.Collections.Generic;

namespace ReGen.Columns
{
    public class StringSequence
    {
        private readonly List<int> _items = new List<int>();
        private readonly List<byte> _stringBuilder = new List<byte>();

        public void Add(byte[] str)
        {
            _stringBuilder.AddRange(str);
            var last = _items.Count > 0 ? _items[_items.Count - 1] : 0;
            _items.Add(last + str.Length);
        }
        public void AddFromChunk(StringSequence srcSeq, int i)
        {
            var start = i == 0 ? 0 : srcSeq._items[i-1];
            var len = srcSeq._items[i] - start;
            Add((srcSeq._stringBuilder, start, len));
        }

        public void Shrink()
        {
            _stringBuilder.TrimExcess();
        }

        public void Add((IList<byte> line, int start, int len) sliceStruct)
        {
            var line = sliceStruct.line;
            var start = sliceStruct.start;
            var len = sliceStruct.len;
            for (var i = 0; i < len; i++)
            {
                _stringBuilder.Add(line[start + i]);
            }
            var last = _items.Count > 0 ? _items[_items.Count - 1] : 0;
            _items.Add(last + len);
        }

    }
}