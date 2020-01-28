using System.Collections.Generic;
using ReGen.ReadWrite;

namespace ReGen.Columns
{
    public class SamChunk
    {
        internal DeduplicatedDictionary Cigar;
        internal DnaEncodingSequences EncodingSequences;
        internal readonly List<char> Flag;
        internal List<byte> Mapq;
        internal List<int> Pnext;
        internal readonly List<int> Pos;

        internal readonly StringSequence Qname;
        internal StringSequence Qual;
        internal readonly DeduplicatedDictionary Rname;
        internal DeduplicatedDictionary Rnext;
        internal List<int> Tlen;

        public SamChunk(int expectedLength)
        {
            Qname = new StringSequence();
            Flag = new List<char>(expectedLength);
            Rname = new DeduplicatedDictionary();
            Pos = new List<int>(expectedLength);
            Mapq = new List<byte>(expectedLength);

            Cigar = new DeduplicatedDictionary();
            Rnext = new DeduplicatedDictionary();
            Pnext = new List<int>(expectedLength);
            Tlen = new List<int>(expectedLength);
            EncodingSequences = new DnaEncodingSequences(expectedLength);
            Qual = new StringSequence();
        }

        public int Count => Flag.Count;

        public override string ToString()
        {
            return "" + Count;
        }

        public void Shrink()
        {
            Qname.Shrink();
            Qual.Shrink();
            EncodingSequences.Shrink();
            Cigar.Shrink();
            Rnext.Shrink();
        }

        public void ReadRow(StringScanner sc)
        {
            Qname.Add(sc.DoSliceStruct());
            Flag.Add((char) sc.DoInt());
            Rname.Add(sc.DoSliceStr());

            Pos.Add(sc.DoInt());
            Mapq.Add((byte) sc.DoInt());
            Cigar.Add(sc.DoSliceStr());
            Rnext.Add(sc.DoSliceStr());
            Pnext.Add(sc.DoInt());
            Tlen.Add(sc.DoInt());
            var seqText = sc.DoSlice();
            EncodingSequences.Add(seqText);
            Qual.Add(sc.DoSliceStruct());
        }

        public void CopyItem(SamChunk chunk, int i)
        {
            Qname.AddFromChunk(chunk.Qname, i);
            Flag.Add(chunk.Flag[i]);
            
            Rname.Add(chunk.Rname.GetValueByIndex(i));

            Pos.Add(chunk.Pos[i]);
            Mapq.Add(chunk.Mapq[i]);
            Cigar.Add(chunk.Cigar.GetValueByIndex(i));
            Rnext.Add(chunk.Rnext.GetValueByIndex(i));
            Pnext.Add(chunk.Pnext[i]);
            Tlen.Add(chunk.Tlen[i]);
            EncodingSequences.AddFromChunk(chunk.EncodingSequences, i);
            Qual.AddFromChunk(chunk.Qname, i);
        }
    }
}