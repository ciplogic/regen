using System;
using System.Collections.Generic;
using System.Linq;

namespace ReGen
{
    class FrameReader
    {
        public int FrameSize { get; }
        public int BatchSize { get; set; }
        public byte[][] Frames { get; }
        public SamChunkFileContentSplitter[] Splitters;
        public List<int> FrameLengths { get; }
        public FrameReader(int frameSize, int batchSize, List<string> headers)
        {
            FrameSize = frameSize;
            BatchSize  = batchSize;
            FrameLengths =new List<int>(batchSize);
            Frames = new byte [batchSize][];
            Splitters = new SamChunkFileContentSplitter[batchSize];
            for (var i = 0; i < batchSize; i++)
            {
                Frames[i] = new byte[frameSize];
                Splitters[i] = new SamChunkFileContentSplitter(Frames[i], headers);
            }
        }

        public void MarkSplitFrames(List<SamChunk> chunks)
        {
            var frameDataShared = new (int len, int index, SamChunkFileContentSplitter splitter)[FrameLengths.Count];
            for (var i = 0; i < FrameLengths.Count; i++)
            {
                (int len, int index, SamChunkFileContentSplitter splitter) item = (FrameLengths[i], i, Splitters[i]);
                frameDataShared[i] = item;
            }

            frameDataShared
                .AsParallel().ForAll(it =>
                    // .Select(it=>
                {
                    var chunk = new SamChunk(20000);
                    it.splitter.IndexEoln(it.len, scanner => { chunk.ReadRow(scanner); });
                    chunk.Shrink();
                    lock (chunks)
                    {
                        chunks.Add(chunk);
                    }
                    // return false;
                })
                ;
                // .ToArray();
        }
        

    }
}