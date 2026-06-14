using System.ComponentModel.DataAnnotations;

namespace AptekaDiplom2.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int PharmacyId { get; set; }
        public Pharmacy? Pharmacy { get; set; }

        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; } = 0;

        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}