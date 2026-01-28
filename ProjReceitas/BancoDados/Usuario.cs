using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using ProjReceitas.Helpers; 

namespace ProjReceitas.BancoDados
{
    public class Usuario
    {
        public int idUsuario { get; set; }   
        public string Nome { get; set; }        
        public string SenhaHash { get; set; }  

        private SqlConnection con;

        public Usuario()
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
                throw new Exception($"Erro ao inicializar conexão com o banco (Usuario): {ex.Message}", ex);
            }
        }

        public int Inserir(string nomeUsuario, string senhaPlana)
        {
            int novoId = 0;
            try
            {
                if (string.IsNullOrWhiteSpace(nomeUsuario))
                    throw new ArgumentException("O nome do usuário não pode ser vazio.", nameof(nomeUsuario));
                if (string.IsNullOrWhiteSpace(senhaPlana))
                    throw new ArgumentException("A senha não pode ser vazia.", nameof(senhaPlana));

                string senhaHasheada = PasswordHasher.HashPassword(senhaPlana);
                string cmdSQL = "INSERT INTO Usuario (Nome, SenhaHash) VALUES (@Nome, @SenhaHash); SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Nome", nomeUsuario.Trim());
                cmd.Parameters.AddWithValue("@SenhaHash", senhaHasheada);

                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();

                if (result != null && result != DBNull.Value)
                {
                    novoId = Convert.ToInt32(result);
                }
            }
            catch (SqlException sqlEx)
            {
                if (con.State == ConnectionState.Open) con.Close();
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    throw new Exception($"O nome de usuário '{nomeUsuario}' já está em uso.", sqlEx);
                }
                throw new Exception($"Erro SQL ao inserir usuário: {sqlEx.Message} (Número do Erro: {sqlEx.Number})", sqlEx);
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception($"Erro geral ao inserir usuário: {ex.Message}", ex);
            }
            return novoId;
        }

        public DataTable SelecionarPorNome()
        {
            DataTable dt = new DataTable();
            try
            {
                if (string.IsNullOrWhiteSpace(this.Nome))
                {
                    return dt;
                }
                string cmdSQL = "SELECT Id_Usuario, Nome, SenhaHash FROM Usuario WHERE Nome = @Nome";
                SqlDataAdapter adapter = new SqlDataAdapter(cmdSQL, con);
                adapter.SelectCommand.Parameters.AddWithValue("@Nome", this.Nome.Trim());
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter usuário pelo nome '{this.Nome}': {ex.Message}", ex);
            }
            return dt;
        }

        public bool VerificarSeUsuarioExiste(string nomeUsuarioParaVerificar)
        {
            bool existe = false;
            try
            {
                if (string.IsNullOrWhiteSpace(nomeUsuarioParaVerificar))
                    return false;

                string cmdSQL = "SELECT COUNT(1) FROM Usuario WHERE Nome = @Nome";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Nome", nomeUsuarioParaVerificar.Trim());

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                existe = count > 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception($"Erro ao verificar existência do usuário '{nomeUsuarioParaVerificar}': {ex.Message}", ex);
            }
            return existe;
        }

        public DataTable SelecionarPorID()
        {
            DataTable dt = new DataTable();
            try
            {
                if (this.idUsuario <= 0)
                {
                    return dt; 
                }
                string cmdSQL = "SELECT Id_Usuario, Nome, SenhaHash FROM Usuario WHERE Id_Usuario = @Id_Usuario";
                SqlDataAdapter adapter = new SqlDataAdapter(cmdSQL, con);
                adapter.SelectCommand.Parameters.AddWithValue("@Id_Usuario", this.idUsuario);

                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter usuário pelo ID '{this.idUsuario}': {ex.Message}", ex);
            }
            return dt;
        }
    }
}