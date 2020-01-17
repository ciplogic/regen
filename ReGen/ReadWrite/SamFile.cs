using System;
using System.IO;

namespace ReGen.ReadWrite
{
    internal class SamFile : IDisposable
    {
        private readonly FileStream _stream;

        public SamFile(string fileName)
        {
            FileName = fileName;
            _stream = File.OpenRead(fileName);
        }

        public string FileName { get; }

        public void Dispose()
        {
            _stream?.Dispose();
        }

        public bool ReadIntoFrame(FrameReader frameReader)
        {
            var batchSize = frameReader.BatchSize;
            var frameSize = frameReader.FrameSize;
            frameReader.FrameLengths.Clear();
            for (var i = 0; i < batchSize; i++)
            {
                var readLen = _stream.Read(frameReader.Frames[i], 0, frameSize);
                frameReader.FrameLengths.Add(readLen);
                if (readLen != frameSize)
                    return false;
            }

            return true;
        }
    }
}