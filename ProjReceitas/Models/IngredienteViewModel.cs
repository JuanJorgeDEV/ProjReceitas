using System.ComponentModel.DataAnnotations;

namespace ProjReceitas.Models
{
    public class IngredienteViewModel
    {
        public int? Id_ingrediente { get; set; }

        [Required(ErrorMessage = "O nome do ingrediente é obrigatório.")]
        [StringLength(20, ErrorMessage = "O nome do ingrediente deve ter no máximo 20 caracteres.")]
        public string Nome { get; set; }

        [DataType(DataType.Currency)]
        public decimal? preco { get; set; }
    }
}