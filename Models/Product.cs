using System.ComponentModel.DataAnnotations;

namespace AptekaDiplom2.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название товара")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше нуля")]
        public decimal Price { get; set; }

        public string? Manufacturer { get; set; }
        public string? ActiveIngredient { get; set; }
        public bool IsPrescriptionRequired { get; set; }
    }
}