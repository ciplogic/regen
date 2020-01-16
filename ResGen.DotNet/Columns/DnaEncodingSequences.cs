using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ReGen
{
    class DnaEncodingSequences
    {
        List<long> fullSequences;
        List<int> _lengths;

        public DnaEncodingSequences(int expectedLength)
        {
            fullSequences = new List<long>(expectedLength);
            _lengths = new List<int>(expectedLength);
        }

        public void shrink()
        {
            fullSequences.TrimExcess();
        }

        static long CharLetterEncode(byte ch)
        {
            switch ((char)ch)
            {
                case 'C': return 1;
                case 'G': return 2;
                case 'T': return 3;
                case 'N': return 4;
                default:
                    return 0;
            }
        }
        static char CharLetterDecode(int ch)
        {
            switch (ch)
            {
                case 0: return 'A';
                case 1: return 'C';
                case 2: return 'G';
                case 3: return 'T';
                case 4: return 'N';
                default:
                    return 'A';
            }
        }

        static int roundUpValue(int value, int divBase){
            var rem = value% divBase;
            var div = value / divBase;
            if (rem==0)
                return value;
            return (div+1)*divBase;
        }
        public static long[] EncodeSequence(byte[] sequence)
        {
            var sequenceLength = sequence.Length;
            var result = new List<long>(sequenceLength/21+1);
            long combinedCode = 0;
            for (var index = 0; index < sequenceLength; index++)
            {
                var dnaLetter = sequence[index];
                var remainder = index % 21; //16 letters fit in a 32 bit uint
                if (remainder == 0)
                {
                    if (result.Count != 0)
                    {
                        result[result.Count-1] = combinedCode;
                    }
                    result.Add(0);
                    combinedCode = 0;
                }

                var encodedLetter = CharLetterEncode(dnaLetter);
                combinedCode += encodedLetter << (remainder * 3);
            }

            result[result.Count-1] = combinedCode;
            var encodeSequence = result.ToArray();
            Debug.Assert(ASCIIEncoding.ASCII.GetString(sequence) == DecodeSequence(encodeSequence, sequenceLength));
            return encodeSequence;
        }

        public static string DecodeSequence(long[] input, int length)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var divide = i / 21;
                var remainderIndex = i % 21;
                var charSeq = input[divide];
                var charShifted = (int)(charSeq >> (remainderIndex * 3)) & 7;
                var charDecoded = CharLetterDecode(charShifted);
                sb.Append(charDecoded);
            }
            return sb.ToString();
        }

        public void Add(byte[] seqText)
        {
            var encoded = EncodeSequence(seqText);
            fullSequences.AddRange(encoded);
            var startIndex = 0;
            if (_lengths.Count!=0){
                startIndex = roundUpValue(_lengths[_lengths.Count-1], 21);
            }
            _lengths.Add(startIndex+seqText.Length);
        }

        public IEnumerable<string> Sequences
        {
            get
            {
                var seqList = new List<long>();
                for (var index = 0; index < _lengths.Count; index++)
                {
                    seqList.Clear();
                    var start = _lengths[index];
                    var length = _lengths[index];
                    var fullSequencesCount = fullSequences.Count;

                    for (var i = 0; i < length/21+1; i++)
                    {
                        if (start + i < fullSequencesCount)
                        {
                            seqList.Add(fullSequences[start+i]);
                        }
                    }
                    yield return DecodeSequence(seqList.ToArray(), length);
                }
            }
        }
    }
}