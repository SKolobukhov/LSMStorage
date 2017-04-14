namespace LSMStorage.Core
{
    public class RemoveOperation: IOperation
    {
        public readonly string Key;


        public RemoveOperation(string key)
        {
            Key = key;
        }

        public Item ToItem()
        {
            return Item.CreateTombStone(Key);
        }

        public override string ToString()
        {
            return $"Remove ({Key})";
        }
    }
}