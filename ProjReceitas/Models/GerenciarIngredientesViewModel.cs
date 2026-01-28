// Arquivo: Models/GerenciarIngredientesViewModel.cs
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjReceitas.Models
{
    public class IngredienteNaReceitaItemViewModel
    {
        public int IdReceitaIngrediente { get; set; }
        public int IdIngrediente { get; set; }
        public string NomeIngrediente { get; set; }
        public string Quantidade { get; set; }
    }

    public class GerenciarIngredientesViewModel
    {
        public int IdReceita { get; set; }
        public string NomeReceita { get; set; }
        public List<IngredienteNaReceitaItemViewModel> IngredientesAtuais { get; set; }

        [Display(Name = "Nome do Ingrediente")]
        [Required(ErrorMessage = "O nome do ingrediente é obrigatório.")]
        public string NomeNovoIngrediente { get; set; }

        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        public string QuantidadeNovoIngrediente { get; set; }

 
        
        public GerenciarIngredientesViewModel()
        {
            IngredientesAtuais = new List<IngredienteNaReceitaItemViewModel>();
        }
    }
}
