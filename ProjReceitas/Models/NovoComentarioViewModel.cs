using System.ComponentModel.DataAnnotations;

namespace ProjReceitas.Models
{
    public class NovoComentarioViewModel
    {
        public int IdReceita { get; set; } 

        [Required(ErrorMessage = "O comentário não pode estar vazio.")]
        [StringLength(1000, ErrorMessage = "O comentário deve ter no máximo 1000 caracteres.")]
        [Display(Name = "Seu Comentário")]
        public string TextoComentario { get; set; }

        [Required(ErrorMessage = "Por favor, dê uma nota para a receita.")]
        [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
        [Display(Name = "Sua nota (1-5)")]
        public int Nota { get; set; }
    }
}