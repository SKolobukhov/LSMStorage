namespace LSMStorage.Core
{
    public class Item
    {
        public static Item CreateItem(string key, string value, long timestamp)
        {
            return new Item(key, value, false, timestamp);
        }

        public static Item CreateItem(Item item, long timestamp)
        {
            return new Item(item.Key, item.Value, false, timestamp);
        }
        
        public static Item CreateTombStone(Item item, long timestamp)
        {
            return new Item(item.Key, item.Value, true, timestamp);
        }

        public readonly string Key;
        public readonly string Value;
        public readonly bool IsTombStone;
        public readonly long Timestamp;

        private Item(string key, string value, bool isTombstone, long timestamp)
        {
            Key = key;
            Value = value;
            IsTombStone = isTombstone;
            Timestamp = timestamp;
        }

        #region EqualityMembers
        private bool Equals(Item other)
        {
            return string.Equals(Key, other.Key)
                && string.Equals(Value, other.Value)
                && IsTombStone == other.IsTombStone
                && Timestamp == other.Timestamp;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Key?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Value?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ IsTombStone.GetHashCode();
                return (hashCode * 397) ^ Timestamp.GetHashCode();
            }
        }
        #endregion
    }
}
