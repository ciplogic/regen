using System.Collections.Generic;

namespace ReGen.Columns
{
    public class StringSequence {
        List<byte> stringBuilder = new List<byte>();
        List<int> Items = new List<int>();
        
        public void Add(byte[] str)
        {
            stringBuilder.AddRange(str);
            var last = Items.Count > 0 ? Items[Items.Count - 1] : 0;
            Items.Add(last + str.Length);
        }

        public void shrink(){
            stringBuilder.TrimExcess();

        }

    }
}