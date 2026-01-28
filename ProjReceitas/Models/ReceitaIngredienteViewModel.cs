using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace ProjReceitas.Models
{
    public class ReceitaIngredienteViewModel
    {

        [Required]
        public int? Id_Receita { get; set; }

        [Required(ErrorMessage = "É obrigatório selecionar um ingrediente.")]
        public int? Id_ingrediente { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        [StringLength(10, ErrorMessage = "A quantidade deve ter no máximo 10 caracteres (ex: 2 xícaras, 100g).")]
        public string Quantidade { get; set; }

        public SelectList IngredientesDisponiveis { get; set; }

        public string NomeReceita { get; set; }
        public string NomeIngrediente { get; set; }
    }
}