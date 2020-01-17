using System.Collections.Generic;

namespace ReGen.Columns
{
    public class DeduplicatedDictionary
    {
        private readonly Dictionary<string, ushort> _table;

        public DeduplicatedDictionary(int expectedItemsCount = 16)
        {
            _table = new Dictionary<string, ushort>(expectedItemsCount);
            Values = new List<string>(expectedItemsCount);
            Indices = new List<ushort>();
        }

        public List<string> Values { get; }
        public List<ushort> Indices { get; }

        public void Add(string value)
        {
            if (!_table.TryGetValue(value, out var resultValue))
            {
                resultValue = (ushort) Values.Count;
                Values.Add(value);
                _table[value] = resultValue;
                Indices.Add(resultValue);
                return;
            }

            Indices.Add(resultValue);
        }

        public void Shrink()
        {
            Values.TrimExcess();
            Indices.TrimExcess();
            _table.TrimExcess();
        }
    }
}