namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class CatalogoDatabaseRepository : Connection, ICatalogoRepository
{
    public CatalogoDatabaseRepository(string conn) : base(conn)
    {

    }


    public List<Catalogo> Read()
    {
        List<Catalogo> catalogos = new List<Catalogo>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
            WITH ProdEscolhido AS (
                SELECT 
                    p.idProduto,
                    p.calcadoId,
                    p.corId,
                    p.preco,
                    p.promocao,
                    ROW_NUMBER() OVER (
                        PARTITION BY p.calcadoId 
                        ORDER BY 
                            CASE WHEN p.promocao IS NOT NULL THEN p.promocao ELSE p.preco END ASC
                    ) AS rn
                FROM Produtos p
            )
            SELECT 
                c.idCalcado,
                pe.idProduto,
                c.nomeCalcado,
                m.nomeMarca,
                pe.preco,
                pe.promocao,
                i.nomeImagem
            FROM Calcados c
            JOIN ProdEscolhido pe ON c.idCalcado = pe.calcadoId AND pe.rn = 1
            JOIN Marca m ON m.idMarca = c.marcaId
            LEFT JOIN Cores co ON co.idCor = pe.corId
            LEFT JOIN Imagens i ON i.corId = co.idCor AND i.statusImagem = 1;";

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            List<string> img = new List<string>();
            img.Add((string)reader["nomeImagem"]);

            catalogos.Add
            (
                new Catalogo
                {
                    idProduto = (int)reader["idProduto"],
                    calcadoId = (int)reader["idCalcado"],
                    nomeCalcado = (string)reader["nomeCalcado"],
                    marcaCalcado = (string)reader["nomeMarca"],
                    preco = (decimal)reader["preco"],
                    promocao = reader["promocao"] != DBNull.Value ? (decimal?)reader["promocao"] : null,
                    ApenasImagemPrincipal = img
                }

            );
        }
        return catalogos;
    }

    public Catalogo Read(int idProduto, int idCalcado)
    {
        int corPrincipal = 0;
        string corPrincipalNome;

        // 1) Pega cor do produto selecionado
        using (SqlCommand cmd = new SqlCommand(
            "SELECT corId, nomeCor FROM Produtos LEFT JOIN Cores on corId = idCor WHERE idProduto = @id", conn))
        {
            cmd.Parameters.AddWithValue("@id", idProduto);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                    corPrincipal = (int)reader["corId"];
                    corPrincipalNome = (string)reader["nomeCor"];
            }
        }

        // 2) Pega tamanhos disponíveis para essa cor
        List<Tamanho> tamanhos = new List<Tamanho>();
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT DISTINCT t.idTamanho, t.tamanho
            FROM Produtos p
            JOIN Tamanhos t ON p.tamanhoId = t.idTamanho
            WHERE p.calcadoId = @calcado AND p.corId = @cor", conn))
        {
            cmd.Parameters.AddWithValue("@calcado", idCalcado);
            cmd.Parameters.AddWithValue("@cor", corPrincipal);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tamanhos.Add(new Tamanho
                    {
                        idTamanho = (int)reader["idTamanho"],
                        tamanho = (string)reader["tamanho"]
                    });
                }
            }
        }

        // 3) Pega todas cores do calçado
        List<Cor> cores = new List<Cor>();
        List<int> listaCores = new List<int>();

        using (SqlCommand cmd = new SqlCommand(
            "SELECT idCor, nomeCor FROM Cores WHERE calcadoId = @id", conn))
        {
            cmd.Parameters.AddWithValue("@id", idCalcado);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int c = (int)reader["idCor"];

                    listaCores.Add(c);
                    cores.Add(new Cor
                    {
                        idCor = c,
                        nomeCor = (string)reader["nomeCor"]
                    });
                }
            }
        }

        // 4) Pega imagens da cor principal
        List<Imagem> imagens = new List<Imagem>();
        using (SqlCommand cmd = new SqlCommand(
            "SELECT idImagem, nomeImagem FROM Imagens WHERE corId = @cor ORDER BY statusImagem DESC", conn))
        {
            cmd.Parameters.AddWithValue("@cor", corPrincipal);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    imagens.Add(new Imagem
                    {
                        idImagem = (int)reader["idImagem"],
                        nomeImagem = (string)reader["nomeImagem"]
                    });
                }
            }
        }

        // 5) Pega imagem principal das outras cores
        List<Imagem> imagensOutrasCores = new List<Imagem>();

        string coresIn = string.Join(",", listaCores);
        using (SqlCommand cmd = new SqlCommand(
            "SELECT idImagem, nomeImagem FROM Imagens WHERE corId IN (1,2) AND statusImagem = 1", conn))
        {
            //cmd.Parameters.AddWithValue("cores", coresIn);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    imagensOutrasCores.Add(new Imagem
                    {
                        idImagem = (int)reader["idImagem"],
                        nomeImagem = (string)reader["nomeImagem"]
                    });
                }
            }
        }

        // 6) Pega dados gerais do produto
        using (SqlCommand cmd = new SqlCommand(@"
            SELECT TOP 1 p.idProduto, c.nomeCalcado, m.nomeMarca, p.preco, p.promocao
            FROM Produtos p
            JOIN Calcados c ON p.calcadoId = c.idCalcado
            JOIN Marca m ON c.marcaId = m.idMarca
            WHERE p.idProduto = @id", conn))
        {
            cmd.Parameters.AddWithValue("@id", idProduto);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Catalogo
                    {
                        idProduto = (int)reader["idProduto"],
                        nomeCalcado = (string)reader["nomeCalcado"],
                        calcadoId = idCalcado,
                        cor = cores,
                        corPrincipal = corPrincipalNome,
                        tamanho = tamanhos,
                        marcaCalcado = (string)reader["nomeMarca"],
                        preco = (decimal)reader["preco"],
                        promocao = reader["promocao"] is DBNull ? 0 : (decimal)reader["promocao"],
                        img = imagens,
                        imgOutrasCores = imagensOutrasCores
                    };
                }
            }
        }

            return null;
        }
    }
