using System;
using System.Collections.Generic;
using System.Linq;
using ReGen.Columns;
using ReGen.ReadWrite;

namespace ReGen
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fileName = @"D:\paper\1_2GB.sam";
            // var fileName = @"c:\paper\input_8G.sam";
            Extensions.TimeIt("Total", () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    var headers = new List<string>();
                    Extensions.TimeIt("Reading", () =>
                    {
                        var chunks = new List<SamChunk>();
                        using var samFile = new SamFile(fileName);
                        FrameReader(samFile, chunks, headers);
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
                tasks
                    // .Select(a =>{a();return false;}).ToArray();
                    .AsParallel().ForAll(action => action());
                frontFrame = backFrame;
                backFrame = frame;
                frame = frontFrame;
            }

            frontFrame.MarkSplitFrames(chunks);
        }
    }
}