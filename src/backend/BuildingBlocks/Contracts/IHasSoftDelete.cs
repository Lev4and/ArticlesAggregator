namespace Contracts;

public interface IHasSoftDelete
{
    bool IsDeleted { get; set; }

    DateTime? DeletedAt { get; set; }
}
