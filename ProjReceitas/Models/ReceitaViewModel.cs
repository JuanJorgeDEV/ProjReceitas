// Arquivo: ProjReceitas/Models/ReceitaViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProjReceitas.Models
{
    public class ReceitaViewModel
    {
        [Display(Name = "Modo de Preparo")]
        [DataType(DataType.MultilineText)]
        public string? ModoPreparo { get; set; }
    
         public int Id_Receita { get; set; }

        [Required(ErrorMessage = "O nome da receita é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome da receita deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        [DataType(DataType.MultilineText)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O tempo de preparo é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O tempo de preparo deve ser um valor positivo.")]
        [Display(Name = "Tempo de Preparo (minutos)")]
        public int? Temp_Preparo { get; set; }

        [Required(ErrorMessage = "O tipo da receita é obrigatório.")]
        [Display(Name = "Tipo da Receita")]
        public string Tipo { get; set; }

        [Display(Name = "Preço Médio da Receita")]
        [DataType(DataType.Currency)]
        [Range(0, 100000, ErrorMessage = "O preço deve ser um valor válido.")]
        public decimal? Preco { get; set; }

        [Display(Name = "Foto da Receita")]
        public IFormFile? ImagemArquivo { get; set; }

        public ReceitaViewModel()
        {
        }
    }
}