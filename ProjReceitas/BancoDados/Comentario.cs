using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjReceitas.BancoDados
{
    public class Comentario
    {

        public int? id_Comentario { get; set; } 
        public int? id_Usuario { get; set; }    
        public int id_Receita { get; set; }   
        public string texto_Comentario { get; set; } 
        public int nota { get; set; }
        public DateTime? data_Postagem { get; set; }

        private SqlConnection con;

        public Comentario()
        {
            try
            {
                IConfigurationRoot o_Config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@".\Configuration\SistemaReceitas.json")
                    .Build();
                string strConexao = o_Config.GetConnectionString(@"StringConexaoSQLServer");
                con = new SqlConnection(strConexao);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao inicializar conexão (Comentario): " + ex.Message, ex);
            }
        }

        public bool Inserir()
        {
            int linhasAfetadas = 0;
            try
            {
                string cmdSQL = "INSERT INTO Comentario (Id_Usuario, Id_Receita, Texto_Comentario, Nota) " +
                                "VALUES (@Id_Usuario, @Id_Receita, @Texto_Comentario, @Nota)";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Id_Usuario", id_Usuario ?? Convert.DBNull); 
                cmd.Parameters.AddWithValue("@Id_Receita", id_Receita);
                cmd.Parameters.AddWithValue("@Texto_Comentario", texto_Comentario);
                cmd.Parameters.AddWithValue("@Nota", nota);

                con.Open();
                linhasAfetadas = cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao inserir comentário: " + ex.Message, ex);
            }
            return linhasAfetadas > 0;
        }

     
        public DataTable SelecionarPorReceita(int idReceitaParaBuscar)
        {
            DataTable dt = new DataTable();
            try
            {
              
                string cmdSQL = @"SELECT 
                                    c.id_Comentario, 
                                    c.Id_Receita, 
                                    c.Texto_Comentario, 
                                    c.Nota, 
                                    c.Data_Postagem, 
                                    c.Id_Usuario,
                                    COALESCE(u.Nome, 'Anônimo') AS NomeUsuario
                                  FROM Comentario c
                                  LEFT JOIN Usuario u ON c.Id_Usuario = u.Id_Usuario
                                  WHERE c.Id_Receita = @Id_Receita
                                  ORDER BY c.Data_Postagem DESC"; 

                SqlDataAdapter adapter = new SqlDataAdapter(cmdSQL, con);
                adapter.SelectCommand.Parameters.AddWithValue("@Id_Receita", idReceitaParaBuscar);

                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao selecionar comentários para a receita ID {idReceitaParaBuscar}: {ex.Message}", ex);
            }
            return dt;
        }
        public void ExcluirPorReceita(int idReceita)
        {
            try
            {
                string cmdSQL = "DELETE FROM Comentario WHERE Id_Receita = @Id_Receita";
                
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Id_Receita", idReceita);

                if (con.State != ConnectionState.Open) con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir comentários da receita ID {idReceita}: " + ex.Message);
            }
            finally
            {
               
                if (con.State == ConnectionState.Open) con.Close();
            }
        }
    }
}