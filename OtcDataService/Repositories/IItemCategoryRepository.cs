using OtcDataService.Models.Entities;

namespace OtcDataService.Repositories;

public interface IItemCategoryRepository
{
    Task<ItemCategory?> GetByCatNoAsync(int catNo, CancellationToken cancellationToken = default);
}
