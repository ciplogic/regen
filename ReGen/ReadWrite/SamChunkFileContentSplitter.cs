using System;
using System.Collections.Generic;

namespace ReGen.ReadWrite
{
    internal class SamChunkFileContentSplitter
    {
        private readonly byte[] _data;
        private readonly List<string> _headers;
        private readonly List<int> _eolnIndices;
        private readonly StringScanner _scanner;

        public SamChunkFileContentSplitter(byte[] data, List<string> headers)
        {
            _data = data;
            _headers = headers;
            _scanner = new StringScanner(_data);
            _eolnIndices = new List<int>();
        }

        public int IndexEoln(int len, Action<StringScanner> onRowAction)
        {
            if (len > _data.Length)
                throw new InvalidOperationException("Index out of bounds");
            _eolnIndices.Clear();
            var index = 0;
            do
            {
                var start = index;
                index = _data.IndexOf('\n', index);
                if (index == -1 || index > len)
                    break;
                _scanner.SetText(start, index);
                if (_scanner.IsHeader)
                {
                    _headers.Add(_scanner.ToString());
                }
                else
                {
                    if (start != 0) onRowAction(_scanner);
                }

                _eolnIndices.Add(index++);
            } while (true);

            return _eolnIndices.Count;
        }
    }
}