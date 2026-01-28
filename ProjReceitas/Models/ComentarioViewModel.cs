using System;

namespace ProjReceitas.Models
{
    public class ComentarioViewModel
    {
        public int IdComentario { get; set; }
        public string NomeUsuario { get; set; }
        public string TextoComentario { get; set; }
        public int Nota { get; set; }
        public DateTime? DataPostagem { get; set; } 

        public string DataPostagemFormatada
        {
            get
            {
                return DataPostagem.HasValue ? DataPostagem.Value.ToString("dd/MM/yyyy 'às' HH:mm") : "Data não disponível";
            }
        }
    }
}