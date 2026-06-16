using OtcDataService.Models.Entities;

namespace OtcDataService.Repositories;

public interface IMktDepRepository
{
    Task<MktDep?> GetByDepNoAsync(int depNo, CancellationToken cancellationToken = default);
}
