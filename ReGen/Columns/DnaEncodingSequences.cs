﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReGen.Columns
{
    internal class DnaEncodingSequences
    {
        public  List<long> FullSequences { get; }
        private readonly List<short> _lengths;
        public List<int> Starts { get; }
        private byte[] _table = new byte[20];

        public DnaEncodingSequences(int expectedLength)
        {
            FullSequences = new List<long>(expectedLength);
            Starts = new List<int>(expectedLength);
            _lengths = new List<short>(expectedLength);
            _table['C' - 'A'] = 1;
            _table['G' - 'A'] = 2;
            _table['T' - 'A'] = 3;
            _table['N' - 'A'] = 4;
        }

        public void Shrink()
        {
            FullSequences.TrimExcess();
            Starts.TrimExcess();
            _lengths.TrimExcess();
            _table = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private long CharLetterEncode(byte ch)
        {
            return _table[ch - 'A'];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static char CharLetterDecode(int ch)
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

        private static int RoundUpValue(int value, int divBase)
        {
            var rem = value % divBase;
            var div = value / divBase;
            if (rem == 0)
                return value;
            return (div + 1) * divBase;
        }

        public long[] EncodeSequence(byte[] sequence)
        {
            var sequenceIsClean = sequence.IndexOf('N', 0) == -1;
            _lengths.Add((short) (sequenceIsClean ? sequence.Length : -sequence.Length));

            if (sequenceIsClean) return EncodeCleanSequence(sequence);

            return ExtractNotCleanSequence(sequence);
        }

        static int LengthOfCleanSequence(int sequenceLength)
        {
            if (sequenceLength == 0)
                return 0;
            return ((sequenceLength-1) / 32) + 1;
        }

        static int LengthOfNotCleanSequence(int sequenceLength)
        {
            if (sequenceLength == 0)
                return 0;
            return ((sequenceLength-1) / 21) + 1;
        }

        private long[] ExtractNotCleanSequence(byte[] sequence)
        {
            var sequenceLength = sequence.Length;
            var len = LengthOfNotCleanSequence(sequenceLength);
            var result = new long[len];
            var pos = 0;
            long combinedCode = 0;
            var bigShift = (long) 1 << 60;

            var shifter = bigShift;
            for (var index = 0; index < sequenceLength; index++)
            {
                var dnaLetter = sequence[index];
                if (shifter == 0)
                {
                    result[pos++] = combinedCode;
                    combinedCode = 0;
                    shifter = bigShift;
                }

                var encodedLetter = CharLetterEncode(dnaLetter);
                combinedCode += encodedLetter * shifter;
                shifter >>= 3;
            }

            result[pos++] = combinedCode;
            // Debug.Assert(Encoding.ASCII.GetString(sequence) == DecodeSequence(result, sequenceLength));
            return result;
        }

        private long[] EncodeCleanSequence(byte[] sequence)
        {
            var sequenceLength = sequence.Length;
            var len = LengthOfCleanSequence(sequenceLength);
            var result = new long[len];
            var pos = 0;
            long combinedCode = 0;
            var bigShift = (long) 1 << 62;

            var shifter = bigShift;
            for (var index = 0; index < sequenceLength; index++)
            {
                var dnaLetter = sequence[index];
                if (shifter == 0)
                {
                    result[pos++] = combinedCode;
                    combinedCode = 0;
                    shifter = bigShift;
                }

                var encodedLetter = CharLetterEncode(dnaLetter);
                combinedCode += encodedLetter * shifter;
                shifter >>= 2;
            }

            result[pos++] = combinedCode;
            //Debug.Assert(Encoding.ASCII.GetString(sequence) == DecodeSequence(result, sequenceLength));
            return result;
        }

        public static string DecodeSequence(long[] input, int length)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var divide = i / 21;
                var remainderIndex = i % 21;
                var charSeq = input[divide];
                var charShifted = (int) (charSeq >> (remainderIndex * 3)) & 7;
                var charDecoded = CharLetterDecode(charShifted);
                sb.Append(charDecoded);
            }

            return sb.ToString();
        }

        public void Add(byte[] seqText)
        {
            var encoded = EncodeSequence(seqText);
            Starts.Add(Starts.Count == 0 ? 0 : FullSequences.Count);
            FullSequences.AddRange(encoded);
        }

        public void AddFromChunk(DnaEncodingSequences src, in int i)
        {
            short srcLen = src._lengths[i];
            int len = LengthOfCleanSequence(srcLen);
            int start = src.Starts[i];
            List<long> fullSeq = src.FullSequences;
            Starts.Add(Starts.Count == 0 ? 0 : FullSequences.Count);
            for (var idx = 0; idx < len; idx++)
            {
                FullSequences.Add(fullSeq[start+idx]);
            }
            _lengths.Add(srcLen);
        }

        public short GetLength(int index)
        {
            return _lengths[index];
        }
    }
}