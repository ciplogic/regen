using System.Collections.Generic;
using System.Text;

namespace ReGen.ReadWrite
{
    public class StringScanner
    {
        private int _end;

        private readonly byte[] _line;
        private readonly Encoding _encoder = Encoding.ASCII;

        public StringScanner(byte[] line)
        {
            _line = line;
        }

        public int Pos { get; set; }

        public bool IsHeader => _line[Pos] == '@';


        public void SetText(int start, int end)
        {
            Pos = start;
            _end = end;
        }

        public byte[] DoSlice()
        {
            var endIndex = _line.IndexOf('\t', Pos);
            if (endIndex == -1)
            {
                Pos = _line.Length;
                return _line.Substring(Pos);
            }

            var result = _line.Substring(Pos, endIndex - Pos);
            Pos = endIndex + 1;
            return result;
        }

        public (byte[]line, int start, int len) DoSliceStruct()
        {
            var endIndex = _line.IndexOf('\t', Pos);
            if (endIndex == -1)
            {
                Pos = _line.Length;
                return (_line, Pos, _line.Length - Pos);
            }

            var oldPos = Pos;
            var oldLen = endIndex - Pos;
            Pos = endIndex + 1;
            return (_line, oldPos, oldLen);
        }

        public string DoSliceStr()
        {
            return _encoder.GetString(DoSlice());
        }

        public int DoInt()
        {
            var result = 0;
            var isNegative = false;
            if (_line[Pos] == '-')
            {
                isNegative = true;
                Pos++;
            }

            while (true)
            {
                var ch = _line[Pos++];
                if (ch == '\t')
                    return isNegative ? -result : result;
                var digit = ch - '0';
                result = result * 10 + digit;
            }
        }

        public override string ToString()
        {
            var resultData = new List<byte>();
            for (var i = Pos; i < _end; ++i)
                resultData.Add(_line[i]);
            return _encoder.GetString(resultData.ToArray());
        }
    }
}