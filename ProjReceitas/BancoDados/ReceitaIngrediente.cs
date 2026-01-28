// Arquivo: ProjReceitas.BancoDados.ReceitaIngrediente.cs
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using ProjReceitas.Models;

namespace ProjReceitas.BancoDados
{
    public class ReceitaIngrediente
    {
        public int? id_Receita_Ingrediente;
        public int? id_Receita;
        public int? id_ingrediente; 
        public string quantidade;

        private SqlConnection con;

        public ReceitaIngrediente()
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
                throw new Exception("Erro ao inicializar conexão (ReceitaIngrediente): " + ex.Message);
            }
        }

        public bool Inserir()
        {
            try
            {
                string cmdSQL = "INSERT INTO Receita_Ingredientes (Id_Receita, Id_Ingrediente, Quantidade) " +
                                "VALUES (@Id_Receita, @Id_Ingrediente, @Quantidade)";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Id_Receita", id_Receita);
                cmd.Parameters.AddWithValue("@Id_Ingrediente", id_ingrediente);
                cmd.Parameters.AddWithValue("@Quantidade", quantidade);

                con.Open();
                int linhasAfetadas = cmd.ExecuteNonQuery();
                con.Close();
                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao inserir em Receita_Ingredientes: " + ex.Message);
            }
        }

        public DataTable SelecionarPorReceita() 
        {
            try
            {
               
                string cmdSQL = @"SELECT 
                              RI.Id_Receita_Ingrediente, 
                              RI.Id_Receita,
                              RI.Id_Ingrediente, 
                              I.Nome AS NomeIngrediente,  
                              RI.Quantidade
                          FROM 
                              Receita_Ingredientes RI
                          INNER JOIN 
                              Ingrediente I ON RI.Id_ingrediente = I.Id_ingrediente 
                          WHERE 
                              RI.Id_Receita = @IdReceita";

                using (SqlConnection conn = new SqlConnection(this.con.ConnectionString)) 
                {
                    SqlDataAdapter o_DataAdapter = new SqlDataAdapter(cmdSQL, conn);
                    o_DataAdapter.SelectCommand.Parameters.AddWithValue("@IdReceita", this.id_Receita);

                    DataTable dtPesquisa = new DataTable();
                    o_DataAdapter.Fill(dtPesquisa);
                    return dtPesquisa;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao selecionar ingredientes da receita: " + ex.Message);
            }
        }

        public bool Excluir()
        {
            try
            {
                string cmdSQL = "DELETE FROM Receita_Ingredientes WHERE Id_Receita_Ingrediente = @Id_Receita_Ingrediente";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Id_Receita_Ingrediente", id_Receita_Ingrediente);

                con.Open();
                int linhasAfetadas = cmd.ExecuteNonQuery();
                con.Close();
                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao excluir ingrediente da receita: " + ex.Message);
            }
        }

        public bool ExcluirTodosPorReceita()
        {
            try
            {
                string cmdSQL = "DELETE FROM Receita_Ingredientes WHERE Id_Receita = @Id_Receita";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Id_Receita", id_Receita);

                con.Open();
                int linhasAfetadas = cmd.ExecuteNonQuery();
                con.Close();
                return linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                throw new Exception("Erro ao excluir todos os ingredientes da receita: " + ex.Message);
            }
        }
        public bool Existe(int idReceita, int idIngrediente)
        {
            bool existe = false;
            try
            {
                string cmdSQL = "SELECT COUNT(1) FROM Receita_Ingredientes WHERE Id_Receita = @Id_Receita AND Id_Ingrediente = @Id_Ingrediente";
                SqlCommand cmd = new SqlCommand(cmdSQL, con);
                cmd.Parameters.AddWithValue("@Id_Receita", idReceita);
                cmd.Parameters.AddWithValue("@Id_ingrediente", idIngrediente);

                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                existe = count > 0;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open) con.Close();
                Console.WriteLine($"Erro ao verificar existência de Receita_Ingrediente: {ex.Message}");
            }
            return existe;
        }
    }
}