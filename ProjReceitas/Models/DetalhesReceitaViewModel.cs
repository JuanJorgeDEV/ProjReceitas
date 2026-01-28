using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjReceitas.Models
{
    public class DetalhesReceitaViewModel
    {
        public int IdReceita { get; set; }
        public string NomeReceita { get; set; }
        public string Descricao { get; set; }
        public int? TempoPreparo { get; set; }
        public string Tipo { get; set; }
        public decimal? PrecoMedio { get; set; } 
        public string AutorReceita { get; set; } 
        public int? IdAutorDaReceita { get; set; }

        public List<IngredienteNaReceitaItemViewModel> IngredientesDaReceita { get; set; }
        public string? ModoPreparo { get; set; }

        public double MediaAvaliacoes { get; set; }
        public int TotalAvaliacoes { get; set; }
        public List<ComentarioViewModel> Comentarios { get; set; }

        public NovoComentarioViewModel NovoComentario { get; set; } 

        public DetalhesReceitaViewModel()
        {
            IngredientesDaReceita = new List<IngredienteNaReceitaItemViewModel>();
            Comentarios = new List<ComentarioViewModel>();
            NovoComentario = new NovoComentarioViewModel();
        }
    }
}