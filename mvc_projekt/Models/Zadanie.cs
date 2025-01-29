using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvc_projekt.Models
{
    public class Zadanie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Tytul { get; set; }
        [MaxLength(200)]
        public string Opis { get; set; }

        [Required]
        public int KategoriaId { get; set; }

        [ForeignKey("KategoriaId")]
        public Kategoria? Kategoria { get; set; }

        [Required]
        public int StatusId { get; set; }

        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        public int? ZadanieNadrzedneId { get; set; }

        [ForeignKey("ZadanieNadrzedneId")]
        public Zadanie? ZadanieNadrzedne { get; set; }

        
        public int? ZadaniaPodrzedneId { get; set; }
        [ForeignKey("ZadaniaPodrzedneId")]
        public Zadanie? ZadaniaPodrzedne { get; set; }
    }
}
