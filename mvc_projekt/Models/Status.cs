using System.ComponentModel.DataAnnotations;

namespace mvc_projekt.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nazwa { get; set; }
    }
}
