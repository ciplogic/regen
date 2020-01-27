using System;
using System.Collections.Generic;
using ReGen.Columns;

namespace ReGen.Algorithms
{
    public class FilterChunks
    {
        private readonly SamChunk[] _chunks;
        private readonly Func<SamChunk, int, bool> _filter;

        public FilterChunks(SamChunk[] chunks, Func<SamChunk, int, bool> filter)
        {
            _chunks = chunks;
            _filter = filter;
        }

        public SamChunk[] Apply()
        {
            var results = new List<SamChunk>();

            for (var i = 0; i < _chunks.Length; i++)
            {
                var currChunk = _chunks[i];
                var item = new SamChunk(currChunk.Count);
                ApplyFilterOnChunk(currChunk, item);


            }

            return results.ToArray();
        }

        private void ApplyFilterOnChunk(SamChunk currChunk, SamChunk item)
        {
            var count = currChunk.Count;
            for (var i = 0; i < count; i++)
            {
                if (_filter(currChunk, i))
                {
                    item.CopyItem(currChunk, i);
                }
            }
            item.Shrink();
        }
    }
}