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

    private readonly IEmailService _emailService;

    public StockMonitorService(IMatchaScraperService matchaScraperSerivce, ILogger<StockMonitorService> logger, IConfiguration config, AppDbContext dbContext, IEmailService emailService)
    {
        _matchaScraperService = matchaScraperSerivce;
        _logger = logger;
        _config = config;
        _dbContext = dbContext;
        _emailService = emailService;
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


            var subject = $"小山園補貨通知｜{currentStock.ProductName} 已重新上架🍵";

            var body = $@"
            <!DOCTYPE html>
            <html>
                <body style='font-family: Arial, sans-serif; background-color:#f5f5f5; padding:20px;'>

                    <div style='max-width:600px; margin:auto; background:#ffffff; padding:24px; border-radius:12px;'>

                        <h2 style='margin-top:0;'>
                            🍵 抹茶補貨通知
                        </h2>

                        <p>
                            目前商品重新補貨：
                        </p>

                        <div style='background:#f3f7f3; padding:16px; border-radius:8px; margin:20px 0;'>

                            <p>
                                <strong>產品名稱：</strong>
                                {currentStock.ProductName}
                            </p>

                            <p>
                                <strong>產品尺寸 / 重量：</strong>
                                {currentStock.Size}
                            </p>

                            <p>
                                <strong>目前價格：</strong>
                                {currentStock.Price}
                            </p>

                        </div>

                        <p>
                            🖼️ 商品目前已重新補貨，如果有購買需求，建議盡快前往官網查看最新庫存。
                        </p>

                        <div style='text-align:center; margin:30px 0;'>

                            <a href='{product.ProductUrl}' target='_blank'
                            style='
                                    display:inline-block;
                                    padding:12px 24px;
                                    background:#2f6b3b;
                                    color:white;
                                    text-decoration:none;
                                    border-radius:6px;
                                    font-weight:bold;
                            '>
                                前往官網購買
                            </a>

                        </div>

                        <p style='font-size:13px; color:#777;'>
                            通知時間：{DateTime.Now:yyyy/MM/dd HH:mm}
                        </p>

                        <hr style='border:none; border-top:1px solid #ddd;'>

                        <p style='font-size:12px; color:#999; text-align:center;'>
                            Matcha Lover Is Watching U 👀🍵
                        </p>

                    </div>

                </body>
            </html>";

            await _emailService.SendAsync(subject, body);

        }
        await _dbContext.SaveChangesAsync();
        return true;
    }
}