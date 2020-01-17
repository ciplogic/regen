using System.Collections.Generic;
using ReGen.ReadWrite;

namespace ReGen.Columns
{
    internal class SamChunk
    {
        internal DeduplicatedDictionary Cigar;
        internal DnaEncodingSequences EncodingSequences;
        private readonly List<char> _flag;
        internal List<byte> Mapq;
        internal List<int> Pnext;
        private readonly List<int> _pos;

        private readonly StringSequence _qname;
        internal StringSequence Qual;
        private readonly DeduplicatedDictionary _rname;
        internal DeduplicatedDictionary Rnext;
        internal List<int> Tlen;

        public SamChunk(int expectedLength)
        {
            _qname = new StringSequence();
            _flag = new List<char>(expectedLength);
            _rname = new DeduplicatedDictionary();
            _pos = new List<int>(expectedLength);
            Mapq = new List<byte>(expectedLength);

            Cigar = new DeduplicatedDictionary();
            Rnext = new DeduplicatedDictionary();
            Pnext = new List<int>(expectedLength);
            Tlen = new List<int>(expectedLength);
            EncodingSequences = new DnaEncodingSequences(expectedLength);
            Qual = new StringSequence();
        }

        public void Shrink()
        {
            _qname.Shrink();
            Qual.Shrink();
            EncodingSequences.Shrink();
            Cigar.Shrink();
            Rnext.Shrink();
        }

        public void ReadRow(StringScanner sc)
        {
            _qname.Add(sc.DoSliceStruct());
            _flag.Add((char) sc.DoInt());
            _rname.Add(sc.DoSliceStr());

            _pos.Add(sc.DoInt());
            Mapq.Add((byte) sc.DoInt());
            Cigar.Add(sc.DoSliceStr());
            Rnext.Add(sc.DoSliceStr());
            Pnext.Add(sc.DoInt());
            Tlen.Add(sc.DoInt());
            var seqText = sc.DoSlice();
            EncodingSequences.Add(seqText);
            Qual.Add(sc.DoSliceStruct());
        }
    }
}