using System.ComponentModel.DataAnnotations;

namespace ProjReceitas.Models
{
    public class UsuarioViewModel
    {
        public int? Id_Usuario { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(11, MinimumLength = 4, ErrorMessage = "A senha deve ter entre 4 e 11 caracteres.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

     
    }
}