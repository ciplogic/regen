using System;
using System.Collections.Generic;
using System.Linq;

namespace ReGen
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = @"D:\paper\1_2GB.sam";
            // var fileName = @"c:\paper\input_8G.sam";
            Extensons.TimeIt("Total", () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var headers = new List<string>();
                    Extensons.TimeIt("Reading", () =>
                    {
                        List<SamChunk> chunks = new List<SamChunk>();
                        using var samFile = new SamFile(fileName);
                        FrameReader(samFile, chunks, headers);
                        Console.WriteLine("Count chunks: " + chunks.Count);
                    });
                }
            });

        }

        private static void FrameReader(SamFile samFile, List<SamChunk> chunks, List<string> headers)
        {
            var frontFrame = new FrameReader(350*20000, Environment.ProcessorCount, headers);
            var backFrame = new FrameReader(350*20000, Environment.ProcessorCount, headers);
            var frame = frontFrame;
            bool canRead = samFile.ReadIntoFrame(frame);
            while (canRead)
            {
                var tasks = new Action[2];
                tasks[0] = () => { canRead = samFile.ReadIntoFrame(backFrame); };
                tasks[1] = () =>
                {
                    frontFrame.MarkSplitFrames(chunks);
                };
                tasks.AsParallel().ForAll(action => action());
                frontFrame = backFrame;
                backFrame = frame;
                frame = frontFrame;
            }
            
            frontFrame.MarkSplitFrames(chunks);
        }
    }
}