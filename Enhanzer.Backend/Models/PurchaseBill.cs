using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Enhanzer.Backend.Models
{
    public class PurchaseBill
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalSelling { get; set; }
        public ICollection<PurchaseBillItem> Items { get; set; } = new List<PurchaseBillItem>();
    }

    public class PurchaseBillItem
    {
        [Key]
        public int Id { get; set; }
        public int PurchaseBillId { get; set; }
        
        [JsonIgnore]
        public PurchaseBill? PurchaseBill { get; set; }

        public string ItemName { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public decimal StandardCost { get; set; }
        public decimal StandardPrice { get; set; }
        public decimal Margin { get; set; }
        public int Qty { get; set; }
        public int FreeQty { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalSelling { get; set; }
    }
}
