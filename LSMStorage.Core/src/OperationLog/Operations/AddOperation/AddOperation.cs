namespace LSMStorage.Core
{
    public class AddOperation: IOperation
    {
        public readonly string Key;
        public readonly string Value;


        public AddOperation(string key, string value)
        {
            Key = key;
            Value = value;
        }
        
        public Item ToItem()
        {
            return Item.CreateItem(Key, Value);
        }

        public override string ToString()
        {
            return $"Add ({Key}, {Value})";
        }
    }
}