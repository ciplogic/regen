using System.Collections.Generic;
using System.Linq;
using ReGen.Columns;

namespace ReGen.Algorithms.Sorting
{
    public class SamChunkPartitionSorter
    {
        private readonly IList<SamChunk> _allChunks;
        public List<ChunkItem>[] Partitions { get; } = new List<ChunkItem>[256];

        public SamChunkPartitionSorter(IList<SamChunk> allChunks)
        {
            _allChunks = allChunks;
            var expectedSize = allChunks.Count * Extensions.PartitionSize / 256;
            for (var index = 0; index < Partitions.Length; index++)
            {
                Partitions[index] = new List<ChunkItem>(expectedSize*110/100);
            }
        }

        public void Apply()
        {
            Partition();
        }

        private void Partition()
        {
            Enumerable.Range(0, Partitions.Length)
                
#if SEQUENTIAL
                .ForEach(
#else
                .AsParallel().ForAll(
#endif
                        i => {
                    DoPartition(i, Partitions[i], _allChunks);
                });
        }

        private void DoPartition(in int index, List<ChunkItem> partition, IList<SamChunk> allChunks)
        {
            int expectedLen = Extensions.PartitionSize;
            foreach (var srcChunk in allChunks)
            {
                var fullSequences = srcChunk.EncodingSequences.FullSequences;
                var starts = srcChunk.EncodingSequences.Starts;
                for (var idxStart = 0; idxStart < starts.Count; idxStart++)
                {
                    var start = starts[idxStart];
                    var firstLong = fullSequences[start];
                    var firstByte = (int) (firstLong >> 56);
                    if (firstByte == index)
                    {
                        var chunkItem = new ChunkItem(srcChunk, idxStart);
                        partition.Add(chunkItem);
                    }
                }
            }
            partition.TrimExcess();
            partition.Sort(new ChunkComparer());
        }
    }
}