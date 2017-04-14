using LSMStorage.Core;

namespace LSMStorage.Tests
{
    internal static class ItemExtensions
    {
        public static IOperation ToOperation(this Item item)
        {
            return item.IsTombStone ? (IOperation) new RemoveOperation(item.Key) : new AddOperation(item.Key, item.Value);
        }
    }
}