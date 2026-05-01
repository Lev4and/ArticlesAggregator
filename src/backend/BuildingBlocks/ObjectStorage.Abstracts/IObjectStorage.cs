using ObjectStorage.Abstracts.Models;
using Result;

namespace ObjectStorage.Abstracts;

public interface IObjectStorage : IDisposable
{
    Task<AppResult> CreateObjectAsync(CreateObjectRequest request, CancellationToken ct = default);

    Task<AppResult<GetObjectResponse>> GetObjectAsync(GetObjectRequest request, CancellationToken ct = default);
    
    Task<AppResult> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken ct = default);
}