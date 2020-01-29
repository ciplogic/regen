using System.Collections.Concurrent;
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

        public static ChunkItem[] ExtractChunkItems(SamChunk samChunk)
        {
            var size = samChunk.Count;
            var indices = new ChunkItem[size];
            for (var i = 0; i < size; i++)
            {
                indices[i] = new ChunkItem(samChunk, i);
            }

            return indices;
        }
    
        public void Sort()
        {
            var sortChunks = _allChunks
                .Select(ExtractChunkItems)
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