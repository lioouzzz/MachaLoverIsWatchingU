
namespace Dtos
{
    public class ProductStockDto
    {
        public string StoreName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public bool IsInStock { get; set; }
    }

}
