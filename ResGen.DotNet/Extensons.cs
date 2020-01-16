using System;
using System.Diagnostics;

namespace ReGen
{
    static class Extensons
    {
        public static int IndexOf(this byte[] line, char ch, int startPos)
        {
            for (var i = startPos; i < line.Length; i++)
            {
                if (line[i] == ch)
                    return i;
            }

            return -1;
        }
        public static byte[] Substring(this byte[] line, int startPos, int length = -1)
        {
            if (length == -1)
            {
                length = line.Length - startPos;
            }

            var result = new byte[length];
            Array.Copy(line, startPos, result, 0, length);
            return result;
        }

        public static void TimeIt(string message, Action action)
        {
            var start = new Stopwatch();
            start.Start();
            action();
            start.Stop();
            Console.WriteLine($"{message}: {(start.ElapsedMilliseconds/1000.0)} s");
        }
    }
}