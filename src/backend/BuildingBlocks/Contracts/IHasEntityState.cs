using Primitives;

namespace Contracts;

public interface IHasEntityState
{
    EntityState EntityState { get; set; }
}