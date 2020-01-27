using System.Collections.Generic;
using ReGen.Columns;

namespace ReGen.Algorithms.Sorting
{
    internal class ChunkComparer : IComparer<ChunkItem>
    {
        public int Compare(ChunkItem x, ChunkItem y)
        {
            return CompareSequences(x.Chunk, x.Index, y.Chunk, y.Index);
        }
        public static int CompareSequences(SamChunk chunkLeft, int indexLeft, SamChunk chunkRight, int indexRight)
        {

            return 0;
        }
    }
}