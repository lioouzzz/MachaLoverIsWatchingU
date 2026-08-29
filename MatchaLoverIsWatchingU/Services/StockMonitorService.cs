using Services.Interfaces;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
namespace Services;


public class StockMonitorService : IStockMonitorService
{
    private readonly IMatchaScraperService _matchaScraperService;
    private readonly ILogger<StockMonitorService> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _config;

    public StockMonitorService(IMatchaScraperService matchaScraperSerivce, ILogger<StockMonitorService> logger, IConfiguration config, AppDbContext dbContext)
    {
        _matchaScraperService = matchaScraperSerivce;
        _logger = logger;
        _config = config;
        _dbContext = dbContext;
    }

    public async Task<bool> CheckStockAsync()
    {
        var currentStock = await _matchaScraperService.GetProductPageAsync();

        var product = await _dbContext.WatchedProducts
                        .Where(w => w.ProductName != null && w.StockKeepingUnit != null && w.Size != null)
                        .FirstOrDefaultAsync();

        if (product == null)
        {
            var newProduct = new WatchedProducts
            {
                StoreName = currentStock.StoreName,
                ProductName = currentStock.ProductName,
                Size = currentStock.Size,
                StockKeepingUnit = currentStock.Sku,
                ProductUrl = _config["WatchSettings:IsuzuUrl"],
                IsInStock = currentStock.IsInStock,
                LastCheckedAt = DateTime.UtcNow
            };

            await _dbContext.WatchedProducts.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("新增監控商品：{ProductName}, 庫存：{IsInStock}", currentStock.ProductName, currentStock.IsInStock);
            return false;
        }

        var previousProductStatus = product.IsInStock;


        //更新網站現在狀態
        product.IsInStock = currentStock.IsInStock;
        product.LastCheckedAt = DateTime.UtcNow;

        //資料庫存貨資料為false,呼叫官網為true則表示官網已補貨
        if (previousProductStatus == false && currentStock.IsInStock == true)
        {
            product.LastRestockedAt = DateTime.UtcNow;
            _logger.LogInformation("官網已補貨抹茶粉🍵 產品名稱: {ProductName} | 產品尺寸重量: {ProductSize}", currentStock.ProductName, currentStock.Size);

        }
        await _dbContext.SaveChangesAsync();
        return true;
    }
}