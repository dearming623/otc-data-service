using OtcDataService.Models.Entities;

namespace OtcDataService.Repositories;

public interface ICleanupTrnRepository
{
    Task<TCleanupTrn?> GetByKeyAsync(TCleanupTrnKey key, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TCleanupTrn>> ListByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TCleanupTrn>> ListByDateRangeAsync(
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> ListDistinctCuItemsByDateRangeAsync(
        DateOnly start,
        DateOnly end,
        CancellationToken cancellationToken = default);

    Task<bool> InsertAsync(TCleanupTrn entity, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(TCleanupTrn entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(TCleanupTrnKey key, CancellationToken cancellationToken = default);
}
