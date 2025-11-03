namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;



public class CatalogoDatabaseRepository: Connection, ICatalogoRepository{
    public CatalogoDatabaseRepository(string conn) : base(conn)
    {

    }


    public List<Catalogo> Read()
    {
        List<Catalogo> catalogos = new List<Catalogo>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Produtos p LEFT JOIN Calcados c on p.calcadoId = c.idCalcado LEFT JOIN Marca m on c.marcaId = m.idMarca LEFT JOIN Imagens i on p.idProduto = i.produtoId WHERE statusImagem = 1";

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
                    nomeCalcado = (string)reader["nomeCalcado"],
                    marcaCalcado = (string)reader["marcaCalcado"],
                    promocao = (float)reader["promocao"],
                    img = img
                }

            );

            img.Clear();


        }
        return catalogos;
    }

    public Catalogo Read(int id)
    {

        List<string> tamanhos = new List<string>();

        SqlCommand cmd1 = new SqlCommand();
        cmd1.Connection = conn;
        cmd1.CommandText = "SELECT * FROM Tamanhos WHERE produtoId = @id";
        cmd1.Parameters.AddWithValue("id", id);

        SqlDataReader reader1 = cmd1.ExecuteReader();

        while (reader1.Read())
        {
            tamanhos.Add((string)reader1["tamanho"]);
        }

        List<string> cores = new List<string>();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = "SELECT * FROM Cores WHERE produtoId = @id";
        cmd2.Parameters.AddWithValue("id", id);

        SqlDataReader reader2 = cmd2.ExecuteReader();

        while (reader2.Read())
        {
            cores.Add((string)reader2["nomeCor"]);
        }

        List<string> imagens = new List<string>();

        SqlCommand cmd3 = new SqlCommand();
        cmd3.Connection = conn;
        cmd3.CommandText = "SELECT * FROM Imagens WHERE produtoId = @id";
        cmd3.Parameters.AddWithValue("id", id);

        SqlDataReader reader3 = cmd3.ExecuteReader();

        while (reader3.Read())
        {
            imagens.Add((string)reader3["nomeImagem"]);
        }

        SqlCommand cmd4 = new SqlCommand();
        cmd4.Connection = conn;
        cmd4.CommandText = "SELECT * FROM Produtos p LEFT JOIN Calcados c on p.calcadoId = c.idCalcado LEFT JOIN Marca m on c.marcaId = m.idMarca LEFT JOIN Tamanhos t p.tamanhoId = t.idTamanho LEFT JOIN Cores co on p.idProduto = co.produtoId LEFT JOIN Imagens i on p.idProduto = i.produtoId WHERE p.idProduto = @id";
        cmd4.Parameters.AddWithValue("id", id);

        SqlDataReader reader4 = cmd4.ExecuteReader();

        if (reader4.Read())
        {
            return new Catalogo
            {
                idProduto = (int)reader4["idProduto"],
                nomeCalcado = (string)reader4["nomeCalcado"],
                cor = cores,
                tamanho = tamanhos,
                marcaCalcado = (string)reader4["marcaCalcado"],
                promocao = (float)reader4["promocao"],
                img = imagens
            };
        }
        
        return null;
    }
    
}