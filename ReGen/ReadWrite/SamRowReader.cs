using System.Collections.Generic;
using System.Text;

namespace ReGen.ReadWrite
{
    public class StringScanner
    {
        
        private byte[] _line;
        private int _end;
        Encoding encoder = Encoding.ASCII;

        public StringScanner(byte[]line)
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

        public byte[] doSlice()
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
        public string doSliceStr()
        {

            return encoder.GetString(doSlice());
        }

        public int doInt()
        {
            int result = 0;
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
                int digit = ch - '0';
                result = result * 10 + digit;
            }
        }

        public override string ToString()
        {
            var resultData = new List<byte>();
            for(var i = Pos; i<_end; ++i)
                resultData.Add(_line[i]);
            return encoder.GetString(resultData.ToArray());
        }
    }
}