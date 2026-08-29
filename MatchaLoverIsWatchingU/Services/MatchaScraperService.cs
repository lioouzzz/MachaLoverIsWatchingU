using Services.Interfaces;
using HtmlAgilityPack;
using Dtos;
namespace Services;

public class MatchaScraperService : IMatchaScraperService
{
    private readonly HttpClient _client;
    private readonly IConfiguration _config;

    public MatchaScraperService(HttpClient client, IConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task<ProductStockDto> GetProductPageAsync()
    {
        var url = _config["WatchSettings:IsuzuUrl"];

        var response = await _client.GetAsync(url);

        response.EnsureSuccessStatusCode();


        var html = await response.Content.ReadAsStringAsync();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);


        var nameNode = doc.DocumentNode
            .SelectSingleNode("//h1[contains(@class, 'product_title')]/span");

        var skuNode = doc.DocumentNode
            .SelectSingleNode("//dl[contains(@class, 'pa-sku')]/dd");

        var sizeNode = doc.DocumentNode
            .SelectSingleNode("//dl[contains(@class, 'pa-size')]/dd");

        var priceNode = doc.DocumentNode
            .SelectSingleNode("//span[contains(@class, 'woocommerce-Price-amount')]/bdi");

        var stockNode = doc.DocumentNode
            .SelectSingleNode("//p[contains(@class, 'stock') and contains(@class, 'in-stock')]");


        return new ProductStockDto
        {
            StoreName = "丸久小山園",
            ProductName = nameNode?.InnerText.Trim() ?? "",
            Sku = skuNode?.InnerText.Trim() ?? "",
            Size = sizeNode?.InnerText.Trim() ?? "",
            Price = priceNode?.InnerText.Trim() ?? "",
            IsInStock = stockNode != null && stockNode.InnerText.Contains("在庫あり") ? true : false
        };

    }
}