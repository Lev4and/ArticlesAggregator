namespace Primitives;

public abstract class EntityBase<TKey>
    where TKey : notnull
{
    public TKey Id { get; set; }
}
