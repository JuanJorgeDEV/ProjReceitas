    // Arquivo: ProjReceitas/BancoDados/Receita.cs
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ProjReceitas.BancoDados
{
    public class Receita
    {
        public int? id_Receita;
        public string nome;
        public string descricao;
        public int? tempPreparo;
        public string tipo;
        public decimal? preco;
        public byte[]? fotoReceita { get; set; }
        public string? fotoReceitaTipoMIME { get; set; }
        public int? idUsuario { get; set; } 
        
        public string? ModoPreparo { get; set; } 
        
        private SqlConnection con;

        public Receita()
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
                throw new Exception("Erro ao inicializar conexão (Receita): " + ex.Message);
            }
        }

        public int Inserir()
        {
            try
            {
                // ***** MODIFICADO PARA INCLUIR ModoPreparo *****
                string cmdSQL = @"INSERT INTO Receita 
                                    (Nome, Descricao, Temp_Preparo, Tipo, Preco, 
                                     FotoReceita, FotoReceitaTipoMIME, Id_Usuario, Modo_Preparo) 
                                  VALUES 
                                    (@Nome, @Descricao, @Temp_Preparo, @Tipo, @Preco, 
                                     @FotoReceita, @FotoReceitaTipoMIME, @Id_Usuario, @Modo_Preparo);
                                  SELECT CAST(SCOPE_IDENTITY() AS INT);";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Descricao", descricao);
                cmd.Parameters.AddWithValue("@Temp_Preparo", tempPreparo ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Tipo", string.IsNullOrEmpty(tipo) ? Convert.DBNull : tipo);
                cmd.Parameters.AddWithValue("@Preco", preco ?? Convert.DBNull);
                cmd.Parameters.AddWithValue("@Id_Usuario", idUsuario ?? Convert.DBNull); 

                cmd.Parameters.AddWithValue("@Modo_Preparo", string.IsNullOrEmpty(ModoPreparo) ? Convert.DBNull : ModoPreparo);

                SqlParameter fotoParam = new SqlParameter("@fotoReceita", SqlDbType.VarBinary, -1);
                fotoParam.Value = (fotoReceita != null && fotoReceita.Length > 0) ? (object)fotoReceita : DBNull.Value;
                cmd.Parameters.Add(fotoParam);
                
                cmd.Parameters.AddWithValue("@fotoReceitaTipoMIME", string.IsNullOrEmpty(fotoReceitaTipoMIME) ? Convert.DBNull : fotoReceitaTipoMIME);

                con.Open();
                object result = cmd.ExecuteScalar();
                con.Close();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao inserir receita: " + ex.Message);
            }
        }

        public DataTable SelecionarReceitasDestaque(int quantidade)
        {
            try
            {
                string cmdSQL = $"SELECT TOP (@Quantidade) Id_Receita, Nome, Descricao, FotoReceita, FotoReceitaTipoMIME, Tipo FROM Receita ORDER BY Id_Receita DESC";

                using (SqlConnection conn = new SqlConnection(this.con.ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(cmdSQL, conn);
                    cmd.Parameters.AddWithValue("@Quantidade", quantidade);

                    SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmd);
                    DataTable dtPesquisa = new DataTable();
                    o_DataAdapter.Fill(dtPesquisa);
                    return dtPesquisa;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao selecionar {quantidade} receitas em destaque: {ex.Message}");
            }
        }
        public DataTable SelecionarFiltro(string? tipoFiltro = null)
        {
            try
            {
                string cmdSQL = @"SELECT 
                                  R.Id_Receita, R.Nome, R.Temp_Preparo, R.Tipo, R.Preco, 
                                  U.Nome AS NomeUsuario 
                              FROM Receita R
                              LEFT JOIN Usuario U ON R.Id_Usuario = U.Id_Usuario";

                if (!string.IsNullOrEmpty(tipoFiltro) && tipoFiltro.ToLower() != "todos")
                {
                    cmdSQL += " WHERE LOWER(R.Tipo) = LOWER(@Tipo)";
                }
                cmdSQL += " ORDER BY R.Nome";

                SqlCommand cmd = new SqlCommand(cmdSQL, con);

                if (!string.IsNullOrEmpty(tipoFiltro) && tipoFiltro.ToLower() != "todos")
                {
                    cmd.Parameters.AddWithValue("@Tipo", tipoFiltro);
                }

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmd);
                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                return dtPesquisa;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao selecionar receitas com filtro: " + ex.Message);
            }
        }
        public DataTable SelecionarPorID()
        {
            try
            {
                string cmdSQL = @"SELECT 
                                  R.*, 
                                  U.Nome AS NomeUsuario
                              FROM Receita R
                              LEFT JOIN Usuario U ON R.Id_Usuario = U.Id_Usuario
                              WHERE R.Id_Receita = @Id_Receita";

                SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, con);
                o_DataAdapter.SelectCommand.Parameters.AddWithValue("@Id_Receita", id_Receita);

                DataTable dtPesquisa = new DataTable();
                o_DataAdapter.Fill(dtPesquisa);
                return dtPesquisa;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao selecionar receita por ID: " + ex.Message);
            }
        }
        public void Update()
        {
            try
            {
                string cmdSQL = @"UPDATE Receita SET 
                            Nome = @Nome, Descricao = @Descricao, Temp_Preparo = @Temp_Preparo, 
                            Tipo = @Tipo, Preco = @Preco, Modo_Preparo = @Modo_Preparo";

                bool fotoEnviada = (fotoReceita != null && fotoReceita.Length > 0 && !string.IsNullOrEmpty(fotoReceitaTipoMIME));
                if (fotoEnviada)
                {
                    cmdSQL += ", FotoReceita = @FotoReceita, FotoReceitaTipoMIME = @fotoReceitaTipoMIME";
                }
                cmdSQL += " WHERE Id_Receita = @Id_Receita";

                SqlCommand cmd = new SqlCommand(cmdSQL, con); 

                cmd.Parameters.AddWithValue("@Id_Receita", id_Receita);
                cmd.Parameters.AddWithValue("@Nome", nome);
                cmd.Parameters.AddWithValue("@Descricao", string.IsNullOrEmpty(descricao) ? (object)DBNull.Value : descricao);
                cmd.Parameters.AddWithValue("@Temp_Preparo", tempPreparo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Tipo", string.IsNullOrEmpty(tipo) ? (object)DBNull.Value : tipo);
                cmd.Parameters.AddWithValue("@Preco", preco ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Modo_Preparo", string.IsNullOrEmpty(ModoPreparo) ? (object)DBNull.Value : ModoPreparo);

                if (fotoEnviada)
                {
                    SqlParameter fotoParam = new SqlParameter("@FotoReceita", SqlDbType.VarBinary, -1);
                    fotoParam.Value = fotoReceita;
                    cmd.Parameters.Add(fotoParam);
                    cmd.Parameters.AddWithValue("@fotoReceitaTipoMIME", fotoReceitaTipoMIME);
                }

                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar receita ID {this.id_Receita}: " + ex.Message, ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public void Excluir()
        {
            if (!this.id_Receita.HasValue || this.id_Receita.Value <= 0)
            {
                throw new InvalidOperationException("ID da Receita não definido para exclusão.");
            }

            try
            {
           
                ReceitaIngrediente riDAL = new ReceitaIngrediente(); 
                riDAL.id_Receita = this.id_Receita;
                riDAL.ExcluirTodosPorReceita(); 

                Comentario comentarioDAL = new Comentario(); 
                comentarioDAL.ExcluirPorReceita(id_Receita.Value); 

                string cmdSQL = "DELETE FROM Receita WHERE Id_Receita = @Id_Receita";
                SqlCommand cmd = new SqlCommand(cmdSQL, con); 
                cmd.Parameters.AddWithValue("@Id_Receita", id_Receita);

                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir a receita ID {id_Receita.Value}: {ex.Message}", ex);
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
    }
}