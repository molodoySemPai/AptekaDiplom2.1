using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AptekaDiplom2.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int PharmacyId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }

        
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Product? Product { get; set; }
        public Pharmacy? Pharmacy { get; set; }

        internal object Field(string v)
        {
            throw new NotImplementedException();
        }
    }
}