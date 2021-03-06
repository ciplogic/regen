﻿using System.Collections.Generic;
using System.Linq;
using ReGen.Columns;

namespace ReGen.ReadWrite
{
    internal class FrameReader
    {
        public SamChunkFileContentSplitter[] Splitters;

        public FrameReader(int frameSize, int batchSize, List<string> headers)
        {
            FrameSize = frameSize;
            BatchSize = batchSize;
            FrameLengths = new List<int>(batchSize);
            Frames = new byte [batchSize][];
            Splitters = new SamChunkFileContentSplitter[batchSize];
            for (var i = 0; i < batchSize; i++)
            {
                Frames[i] = new byte[frameSize];
                Splitters[i] = new SamChunkFileContentSplitter(Frames[i], headers);
            }
        }

        public int FrameSize { get; }
        public int BatchSize { get; set; }
        public byte[][] Frames { get; }
        public List<int> FrameLengths { get; }

        public void MarkSplitFrames(List<SamChunk> chunks)
        {
            var frameDataShared = new (int len, int index, SamChunkFileContentSplitter splitter)[FrameLengths.Count];
            for (var i = 0; i < FrameLengths.Count; i++)
            {
                (int len, int index, SamChunkFileContentSplitter splitter) item = (FrameLengths[i], i, Splitters[i]);
                frameDataShared[i] = item;
            }

            frameDataShared.
            #if SEQUENTIAL
                Select(
            #else
                AsParallel().ForAll(
            #endif
                    it =>{
                    var chunk = new SamChunk(Extensions.PartitionSize);
                    it.splitter.IndexEoln(it.len, scanner => { chunk.ReadRow(scanner); });
                    chunk.Shrink();
                    lock (chunks)
                    {
                        chunks.Add(chunk);
                    }

            #if SEQUENTIAL
                     return false;
            #endif
                })
            #if SEQUENTIAL
                .ToArray();
            #else
                ;
            #endif
        }
    }
}