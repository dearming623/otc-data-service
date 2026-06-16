using OtcDataService.Models.Entities;

namespace OtcDataService.Repositories;

public interface IProdtableRepository
{
    Task<Prodtable?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Prodtable>> ListByPCodeAsync(string pCode, CancellationToken cancellationToken = default);

    Task<bool> InsertAsync(Prodtable entity, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Prodtable entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string barcode, CancellationToken cancellationToken = default);
}
