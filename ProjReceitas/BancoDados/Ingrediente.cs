// Arquivo: ProjReceitas.BancoDados/Ingrediente.cs
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjReceitas.BancoDados
{
    public class Ingrediente
    {
        public int? id_ingrediente;
        public string nome;


        private SqlConnection con;

        public Ingrediente()
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
                throw new Exception("Erro ao inicializar conexão (Ingrediente): " + ex.Message);
            }
        }

        public int Inserir()
        {
            try
            {
                // Query SQL não inclui mais Preco
                string cmdSQL = "INSERT INTO Ingrediente (Nome) VALUES (@Nome); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Nome", nome);
                // Parâmetro Preco REMOVIDO

                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao inserir ingrediente: " + ex.Message);
            }
        }

        public DataTable SelecionarTodos()
        {
            try
            {
                string cmdSQL = "SELECT Id_ingrediente, Nome FROM Ingrediente ORDER BY Nome";
                SqlDataAdapter adapter = new SqlDataAdapter(cmdSQL, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao selecionar todos os ingredientes: " + ex.Message);
            }
        }

        public DataTable SelecionarPorNome()
        {
            try
            {
                string cmdSQL = "SELECT Id_Ingrediente, Nome FROM Ingrediente WHERE LOWER(Nome) = LOWER(@Nome)";
                SqlDataAdapter adapter = new SqlDataAdapter(cmdSQL, con);
                adapter.SelectCommand.Parameters.AddWithValue("@Nome", nome.Trim());

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao selecionar ingrediente por nome '{nome}': {ex.Message}");
            }
        }

        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = "SELECT Id_Ingrediente, Nome FROM Ingrediente WHERE Id_Ingrediente = @Id_Ingrediente";
                SqlDataAdapter adapter = new SqlDataAdapter(cmdSQL, con);
                adapter.SelectCommand.Parameters.AddWithValue("@Id_ingrediente", id_ingrediente);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao selecionar ingrediente por ID '{id_ingrediente}': {ex.Message}");
            }
        }
        public void Excluir()
        {
            try
            {
                string cmdSQL = "DELETE FROM Ingrediente WHERE Id_Ingrediente = @Id_Ingrediente"; 
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Id_Ingrediente", id_ingrediente);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao excluir ingrediente: " + ex.Message);
            }
        }
  
    }
}