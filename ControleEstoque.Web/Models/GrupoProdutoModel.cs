﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Configuration;

namespace ControleEstoque.Web.Models
{
    public class GrupoProdutoModel
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome!")]
        public string Nome { get; set; }
        public bool Ativo { get; set; }


        public static List<GrupoProdutoModel> RecuperarLista()
        {
            var ret = new List<GrupoProdutoModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from grupo_produto order by nome";
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        ret.Add(new GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["Nome"],
                            Ativo = (bool)reader["Ativo"]

                        });
                    }
                }
            }

            return ret;
        }

        public static GrupoProdutoModel RecuperarPerloId(int id)
        {
            GrupoProdutoModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format("select * from grupo_produto where (id = {0})", id);
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        ret = new GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["Nome"],
                            Ativo = (bool)reader["Ativo"]

                        };
                    }
                }
            }

            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPerloId(id) != null)
            {

                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();
                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = string.Format("Delete from grupo_produto where (id = {0})", id);
                        ret = (comando.ExecuteNonQuery() > 0); //ExecuteNonQuery quantidade de registro afetados pelo comando
                    }
                }
            }
            return ret;
        }

        public int Salvar()
        {
            // INSERE x ALTERA GRUPO DE PRODUTOS.
            var ret = 0;
            var model = RecuperarPerloId(this.Id);  //Busca o cara pelo ID.

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    if (model == null) // se existir
                    {// INSERE
                        comando.CommandText = string.Format("INSERT INTO grupo_produto (NOME, ATIVO) VALUES ('{0}', {1}); select convert(int, scope_identity());", this.Nome, this.Ativo ? 1 : 0);
                        ret = (int)comando.ExecuteScalar(); //ExecuteScalar retorna um object, convertido para inteiro
                    }
                    else
                    {// ALTERA
                        comando.CommandText = string.Format(
                            "UPDATE grupo_produto SET NOME = '{1}', ATIVO = {2} WHERE Id = {0};", this.Id, this.Nome, this.Ativo ? 1 : 0);
                        if (comando.ExecuteNonQuery() > 0) //ExecuteNonQuery e retorna um inteiro, qtde de registros
                        {
                            ret = this.Id;   
                        }

                    }
                }
            }
            return ret;
        }

    }
}