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
                    CASE 
                        WHEN p.promocao IS NOT NULL THEN p.promocao 
                        ELSE p.preco 
                    END ASC
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
        JOIN ProdEscolhido pe 
        ON c.idCalcado = pe.calcadoId 
        AND pe.rn = 1
        JOIN Marca m 
        ON m.idMarca = c.marcaId
        LEFT JOIN ImagensProdutos ip 
        ON ip.calcadoId = c.idCalcado 
        AND ip.corId = pe.corId
        LEFT JOIN Imagens i 
        ON i.idImagem = ip.imagemId 
        AND i.statusImagem = 1;";

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
                    ApenasImagemPrincipal = img,
                }

            );
        }
        return catalogos;
    }

    public List<Slider> ReadSlides()
    {
        List<Slider> sliders = new List<Slider>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT img FROM Slider WHERE status = 1";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            sliders.Add(new Slider
            {
                img = (string)reader["img"]
            });
        }

        reader.Close();
        return sliders;
    }

    public Catalogo Read(int idProduto, int idCalcado)
    {
        int corPrincipal = 0;
        string corPrincipalNome;

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

        List<Cor> cores = new List<Cor>();
        List<int> listaCores = new List<int>();

        using (SqlCommand cmd = new SqlCommand(
            "SELECT co.idCor, co.nomeCor FROM Cores co LEFT JOIN CoresCalcados cc on co.idCor = cc.corId WHERE cc.calcadoId = @id", conn))
        {
            cmd.Parameters.AddWithValue("@id", idCalcado);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int cor = (int)reader["idCor"];

                    listaCores.Add(cor);
                    cores.Add(new Cor
                    {
                        idCor = cor,
                        nomeCor = (string)reader["nomeCor"]
                    });
                }
            }
        }

        List<Imagem> imagens = new List<Imagem>();
        using (SqlCommand cmd = new SqlCommand(
            @"SELECT i.idImagem, i.nomeImagem FROM Imagens i LEFT JOIN ImagensProdutos ip on ip.corId = i.corId
             WHERE ip.corId = @cor AND ip.calcadoId = @calcadoId ORDER BY i.statusImagem DESC", conn))
        {
            cmd.Parameters.AddWithValue("@cor", corPrincipal);
            cmd.Parameters.AddWithValue("@calcadoId", idCalcado);

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

        List<Imagem> imagensOutrasCores = new List<Imagem>();

        string coresIn = string.Join(",", listaCores);
        using (SqlCommand cmd = new SqlCommand(
            @"SELECT idImagem, nomeImagem FROM Imagens i LEFT JOIN ImagensProdutos ip on ip.corId = i.corId 
            WHERE ip.corId IN (@cores) AND ip.calcadoId = @calcadoId AND i.statusImagem = 1", conn))
        {
            cmd.Parameters.AddWithValue("cores", coresIn);
            cmd.Parameters.AddWithValue("@calcadoId", idCalcado);

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

        using (SqlCommand cmd = new SqlCommand(@"
            SELECT p.idProduto, c.nomeCalcado, m.nomeMarca, p.preco, p.promocao
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
