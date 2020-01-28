using System;
using System.Collections.Generic;
using System.Linq;
using ReGen.Algorithms;
using ReGen.Algorithms.Sorting;
using ReGen.Columns;
using ReGen.ReadWrite;

namespace ReGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fileName = @"g:\paper\1_2GB.sam";
            // var fileName = @"c:\paper\input_8G.sam";
            Extensions.TimeIt("Total", () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var headers = new List<string>();
                    Extensions.TimeIt("Total time", () =>
                    {
                        var chunks = new List<SamChunk>();
                        Extensions.TimeIt("Reading", () =>
                        {
                            using var samFile = new SamFile(fileName);
                            FrameReader(samFile, chunks, headers);
                        });
                        GC.Collect(2);
                        Extensions.TimeIt("Sorting", () =>
                        {
                            var sorter = new SamChunkSorter(chunks);
                            sorter.Sort();
                        });
                        Console.WriteLine("Count chunks: " + chunks.Count);
                    });
                }
            });
        }

        private static void FrameReader(SamFile samFile, List<SamChunk> chunks, List<string> headers)
        {
            var frontFrame = new FrameReader(350 * 20000, Environment.ProcessorCount, headers);
            var backFrame = new FrameReader(350 * 20000, Environment.ProcessorCount, headers);
            var frame = frontFrame;
            var canRead = samFile.ReadIntoFrame(frame);
            while (canRead)
            {
                var tasks = new Action[2];
                tasks[0] = () => { canRead = samFile.ReadIntoFrame(backFrame); };
                tasks[1] = () => { frontFrame.MarkSplitFrames(chunks); };
                tasks.
                #if SEQUENTIAL
                    Select(a =>{a();return false;}).ToArray();
                    #else
                    AsParallel().ForAll(action => action());
                #endif
                frontFrame = backFrame;
                backFrame = frame;
                frame = frontFrame;
            }

            frontFrame.MarkSplitFrames(chunks);
        }
    }
}