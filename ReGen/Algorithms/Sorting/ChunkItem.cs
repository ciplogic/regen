using ReGen.Columns;

namespace ReGen.Algorithms.Sorting
{
    public struct ChunkItem
    {
        public SamChunk Chunk;
        public int Index;
        
        public ChunkItem(SamChunk chunk, int index)
        {
            Chunk = chunk;
            Index = index;
        }

    }
}