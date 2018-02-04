namespace Core.Providers
{
    public interface IProvider<TKey, TValue>
    {
        TValue this[TKey key] { get; }
    }
}
