namespace myon.Data
{
    public class UniqueNumberService
    {
        private int n;

        private Dictionary<object, int> _uniqueNumbers = new();

        public bool TryGetNumber(object key, out int value)
        {
            if (!_uniqueNumbers.TryGetValue(key, out value))
            {
                value = n;
                return TryAddNumber(key);
            }
            return true;
        }

        public bool TryAddNumber(object key) => _uniqueNumbers.TryAdd(key, n++);
    }
}
