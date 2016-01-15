namespace Yanyitec.Collections
{
    public interface IDiamond<TX, TY, TValue>
    {
        TValue this[TX x, TY y] { get; set; }

        bool TryGetValue(TX x, TY y, out TValue value);
    }
}