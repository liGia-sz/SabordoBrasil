using System.ComponentModel.DataAnnotations;

namespace SeuProjeto.Models
{
    public class Prato
    {
        [Key]
        public int IdPrato { get; set; }
        [Required]
        public string? NomePrato { get; set; }
        public string? Foto { get; set; }
        public string? Localidade { get; set; }
        public string? Cidade { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}