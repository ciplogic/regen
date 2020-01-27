using System.Collections.Generic;
using System.Linq;
using ReGen.Columns;

namespace ReGen.Algorithms.Sorting
{
    public class SamChunkSorter
    {
        private readonly List<SamChunk> _allChunks;

        public SamChunkSorter(List<SamChunk> allChunks)
        {
            _allChunks = allChunks;
        }

        public void Sort()
        {
            var sortChunks = _allChunks.AsParallel().Select(ChunkSorter.ExtractChunkItems)
                .ToArray();
            var listChunks = new List<ChunkItem>();
            foreach (var sortChunk in sortChunks)
            {
                listChunks.AddRange(sortChunk);
            }
            listChunks.Sort(new ChunkComparer());
        }
    }
}