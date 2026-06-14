using System.ComponentModel.DataAnnotations;

namespace AptekaDiplom2.Models
{
    public class Pharmacy
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название аптеки")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите адрес")]
        public string Address { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string? WorkingHours { get; set; }
    }
}