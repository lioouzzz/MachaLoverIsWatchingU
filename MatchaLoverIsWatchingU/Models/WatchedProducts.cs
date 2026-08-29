using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class WatchedProducts
    {
        [Key]
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public string Size { get; set; } = string.Empty;
        public string StockKeepingUnit { get; set; } = string.Empty;

        public string ProductUrl { get; set; } = string.Empty;

        public bool IsInStock { get; set; }

        public DateTime? LastCheckedAt { get; set; }
        public DateTime? LastRestockedAt { get; set; }


    }
}