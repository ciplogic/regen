using ReGen.Columns;

namespace ReGen.Algorithms.Sorting
{
    internal class ChunkSorter
    {
        private readonly SamChunk _samChunk;

        public ChunkSorter(SamChunk samChunk)
        {
            _samChunk = samChunk;
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
    }
}