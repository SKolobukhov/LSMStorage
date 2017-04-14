namespace LSMStorage.Core
{
    public interface IMemTable
    {
        void Apply(IOperation item);
        
        Item Get(string key);
    }
}