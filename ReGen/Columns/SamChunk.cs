using System.Collections.Generic;
using ReGen.ReadWrite;

namespace ReGen.Columns
{
    class SamChunk
    {
          
        StringSequence QNAME;
        List<char> FLAG;
        DeduplicatedDictionary RNAME;
        List<int> POS;
        internal List<byte> MAPQ;
        internal DeduplicatedDictionary CIGAR;
        internal DeduplicatedDictionary RNEXT;
        internal List<int> PNEXT ;
        internal List<int> TLEN;
        internal DnaEncodingSequences EncodingSequences;
        internal StringSequence QUAL;
        
        public SamChunk(int expectedLength)
        {
            QNAME = new StringSequence();
            FLAG = new List<char>(expectedLength);
            RNAME = new DeduplicatedDictionary();
            POS = new List<int>(expectedLength);
            MAPQ = new List<byte>(expectedLength);

            CIGAR = new DeduplicatedDictionary();
            RNEXT = new DeduplicatedDictionary();
            PNEXT = new List<int>(expectedLength);
            TLEN = new List<int>(expectedLength);
            EncodingSequences = new DnaEncodingSequences(expectedLength);
            QUAL = new StringSequence();
        }

        public void Shrink()
        {
            QNAME.shrink();
            QUAL.shrink();
            EncodingSequences.Shrink();
            CIGAR.shrink();
            RNEXT.shrink();
        }

        public void ReadRow(StringScanner sc)
        {
            QNAME.Add((sc.doSlice()));
            FLAG.Add((char) sc.doInt());
            RNAME.Add(sc.doSliceStr());

            POS.Add(sc.doInt());
            MAPQ.Add((byte) sc.doInt());
            CIGAR.Add(sc.doSliceStr());
            RNEXT.Add(sc.doSliceStr());
            PNEXT.Add(sc.doInt());
            TLEN.Add(sc.doInt());
            var seqText = sc.doSlice();
            EncodingSequences.Add(seqText);
            QUAL.Add(sc.doSlice());
        }
    }
}