using System.Collections.Generic;
using System.Linq;
using ReGen.Columns;

namespace ReGen.Algorithms
{
    class RemoveUnmappedReads
    {
        private readonly IList<SamChunk> _chunks;

        public RemoveUnmappedReads(IList<SamChunk> chunks)
        {
            _chunks = chunks;
        }

        public SamChunk[] Apply()
        {
            var newChunks = new SamChunk[_chunks.Count];
            Enumerable.Range(0, newChunks.Length)
                .AsParallel().ForAll(i =>
                // .ForEach(i=>
                {
                    var srcChunk = _chunks[i];
                    var destChunk = new SamChunk(srcChunk.Count);
                    newChunks[i] = destChunk;
                    for (var idx = 0; idx < srcChunk.Count; idx++)
                    {
                        short seqLen = srcChunk.EncodingSequences.GetLength(idx);
                        var isMapped = seqLen > 0;
                        if (isMapped)
                        {
                            destChunk.CopyItem(srcChunk, idx);
                        }
                    }
                });
            return newChunks;
        }
    }
}