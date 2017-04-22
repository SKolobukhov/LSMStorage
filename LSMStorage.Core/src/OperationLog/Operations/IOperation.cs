namespace LSMStorage.Core
{
    public interface IOperation
    {
        long Timestamp { get; }

        void Apply(IMemStorage storage);
    }
}