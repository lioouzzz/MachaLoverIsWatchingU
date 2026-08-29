
namespace Services.Interfaces;

public interface IStockMonitorService
{
    Task<bool> CheckStockAsync();
}