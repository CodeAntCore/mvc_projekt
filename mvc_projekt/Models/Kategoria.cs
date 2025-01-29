using System.ComponentModel.DataAnnotations;

namespace mvc_projekt.Models
{
    public class Kategoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nazwa { get; set; }

    }
}
