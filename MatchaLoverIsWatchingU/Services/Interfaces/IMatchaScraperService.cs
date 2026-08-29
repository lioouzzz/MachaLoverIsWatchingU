
using Dtos;
namespace Services.Interfaces;

public interface IMatchaScraperService
{
    Task<ProductStockDto> GetProductPageAsync();
}