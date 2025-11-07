namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class EstoqueDatabaseRepository : Connection, IEstoqueRepository
{
    public EstoqueDatabaseRepository(string conn) : base(conn)
    {

    }

    public void Create(Estoque estoque)
    {
        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;

        cmd.CommandText = @"INSERT INTO Produtos(calcadoId, CorId, tamanhoId, preco, qtd) VALUES (@calcadoId, @corId, @tamanhoId, @preco, @qtd);
        SELECT CAST(SCOPE_IDENTITY() AS int)";

        cmd.Parameters.AddWithValue("@calcadoId", estoque.calcadoId);
        cmd.Parameters.AddWithValue("@corId", estoque.corId);
        cmd.Parameters.AddWithValue("@tamanhoId", estoque.tamanhoId);
        cmd.Parameters.AddWithValue("@preco", estoque.preco);
        cmd.Parameters.AddWithValue("@qtd", estoque.qtd);

        int idProduto = (int)cmd.ExecuteScalar();

        SqlCommand cmd2 = new SqlCommand();

        cmd2.Connection = conn;

        cmd2.CommandText = @"INSERT INTO Entradas(idFornecedor) VALUES(@idFornecedor);
        SELECT CAST(SCOPE_IDENTITY() AS int)";

        cmd2.Parameters.AddWithValue("@idFornecedor", estoque.idFornecedor);

        int idEntrada = (int)cmd2.ExecuteScalar();


        SqlCommand cmd3 = new SqlCommand();

        cmd3.Connection = conn;

        cmd3.CommandText = @"INSERT INTO Itens_Entradas(idProduto, idEntrada, valorIE, qtdIE) VALUES(@idProduto, @idEntrada, @valorIE, @qtdIE)";

        cmd3.Parameters.AddWithValue("@idProduto", idProduto);
        cmd3.Parameters.AddWithValue("@idEntrada", idEntrada);
        cmd3.Parameters.AddWithValue("@valorIE", estoque.valorIE);
        cmd3.Parameters.AddWithValue("@qtdIE", estoque.qtd);
      
        cmd3.ExecuteNonQuery();

    }

    public Estoque Create()
    {
        List<Calcado> calcados = new List<Calcado>();

        SqlCommand cmd1 = new SqlCommand();
        cmd1.Connection = conn;
        cmd1.CommandText = "SELECT * FROM Calcados";
        SqlDataReader reader1 = cmd1.ExecuteReader();

        while (reader1.Read())
        {
            calcados.Add
            (
                new Calcado
                {
                    idCalcado = (int)reader1["idCalcado"],
                    nomeCalcado = (string)reader1["nomeCalcado"]
                }
            );
        }

        reader1.Close();

        List<Tamanho> tamanhos = new List<Tamanho>();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = "SELECT * FROM Tamanhos";
        SqlDataReader reader2 = cmd2.ExecuteReader();

        while (reader2.Read())
        {
            tamanhos.Add
            (
                new Tamanho
                {
                    idTamanho = (int)reader2["idTamanho"],
                    tamanho = (string)reader2["tamanho"]
                }
            );
        }
        reader2.Close();

        List<Cor> cores = new List<Cor>();

        SqlCommand cmd3 = new SqlCommand();
        cmd3.Connection = conn;
        cmd3.CommandText = "SELECT * FROM Cores";
        SqlDataReader reader3 = cmd3.ExecuteReader();

        while (reader3.Read())
        {
            cores.Add
            (
                new Cor
                {
                    idCor = (int)reader3["idCor"],
                    nomeCor = (string)reader3["nomeCor"]
                }
            );
        }

        reader3.Close();

        List<Fornecedor> fornecedores = new List<Fornecedor>();

        SqlCommand cmd4 = new SqlCommand();
        cmd4.Connection = conn;
        cmd4.CommandText = "SELECT * FROM Fornecedor";
        SqlDataReader reader4 = cmd4.ExecuteReader();

        while (reader4.Read())
        {
            fornecedores.Add
            (
                new Fornecedor
                {
                    idFornecedor = (int)reader4["idFornecedor"],
                    nomeFornecedor = (string)reader4["nomeForn"]
                }
            );
        }

        reader4.Close();

        return new Estoque
        {
            selectCalcado = calcados,
            selectCor = cores,
            selectTamanho = tamanhos,
            selectFornecedor = fornecedores
        };

    }

    public void CreateImg(Estoque estoque)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"INSERT INTO Imagens (corId, nomeImagem, statusImagem) VALUES(@corId, @nomeImagem, 0);
        SELECT CAST(SCOPE_IDENTITY() AS int);";
        cmd.Parameters.AddWithValue("@corId", estoque.corId);
        cmd.Parameters.AddWithValue("@nomeImagem", estoque.img);
        int idImagem = (int)cmd.ExecuteScalar();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = @"INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) VALUES(@corId, @calcadoId, @imagemId)";
        cmd2.Parameters.AddWithValue("@corId", estoque.corId);
        cmd2.Parameters.AddWithValue("@calcadoId", estoque.calcadoId);
        cmd2.Parameters.AddWithValue("@imagemId", idImagem);

        cmd2.ExecuteNonQuery();
    }

    public void Delete(int idProduto)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Estoque WHERE idProduto = @idProduto";
        cmd.Parameters.AddWithValue("idProduto", idProduto);

        cmd.ExecuteNonQuery();
    }

    public void DeleteImg(int idImagem)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM ImagensProdutos WHERE imagemId = @idImagem";
        cmd.Parameters.AddWithValue("idImagem", idImagem);

        cmd.ExecuteNonQuery();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = "DELETE FROM Imagens WHERE idImagem = @idImagem";
        cmd2.Parameters.AddWithValue("idImagem", idImagem);

        cmd2.ExecuteNonQuery();
    }

    public List<Estoque> Read()
    {
        List<Estoque> Estoque = new List<Estoque>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"SELECT distinct p.idProduto, c.nomeCalcado, t.tamanho, co.nomeCor, p.preco, p.corId, p.calcadoId, f.nomeForn, m.nomeMarca, p.qtd, p.promocao 
        FROM Produtos p LEFT JOIN Calcados c on c.idCalcado = p.calcadoId 
        LEFT JOIN Cores co on p.corId = co.idCor 
        LEFT JOIN Tamanhos t on p.tamanhoId = t.idTamanho
        LEFT JOIN Marca m on m.idMarca = c.marcaId
		LEFT JOIN Itens_Entradas ie on ie.idProduto = p.idProduto
		LEFT JOIN Entradas e on e.idEntrada = ie.idEntrada
		LEFT JOIN Fornecedor f on f.idFornecedor = e.idFornecedor";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Estoque.Add(
                new Estoque
                {
                    idProduto = (int)reader["idProduto"],
                    nomeCalcado = (string)reader["nomeCalcado"],
                    tamanho = (string)reader["tamanho"],
                    nomeCor = (string)reader["nomeCor"],
                    preco = (decimal)reader["preco"],
                    corId = (int)reader["corId"],
                    calcadoId = (int)reader["calcadoId"],
                    nomeFornecedor = (string)reader["nomeForn"] ,
                    nomeMarca = (string)reader["nomeMarca"],
                    qtd = (int)reader["qtd"],
                    promocao = reader["promocao"] is DBNull ? 0 : (decimal)reader["promocao"]
                }
            );
        }
        return Estoque;
    }

    public Estoque Read(int idEstoque)
    {
        Estoque estoque = new Estoque();

        List<Imagem> imagens = new List<Imagem>();


        SqlCommand cmd1 = new SqlCommand();
        cmd1.Connection = conn;
        cmd1.CommandText = @"SELECT * FROM Produtos p LEFT JOIN Itens_Entradas ie on p.idProduto = ie.idProduto LEFT JOIN Entradas e on ie.idEntrada = e.idEntrada WHERE p.idProduto = @idProduto";
        cmd1.Parameters.AddWithValue("@idProduto", idEstoque);
        SqlDataReader reader1 = cmd1.ExecuteReader();

        if (reader1.Read())
        {
            estoque.calcadoId = (int)reader1["calcadoId"];
            estoque.corId = (int)reader1["corId"];
            estoque.qtd = (int)reader1["qtd"];
            estoque.promocao = reader1["promocao"] is DBNull ? 0 : (decimal)reader1["promocao"];
            estoque.valorIE = (decimal)reader1["valorIE"];
            estoque.preco = (decimal)reader1["preco"];
            estoque.idFornecedor = (int)reader1["idFornecedor"];
            estoque.idProduto = (int)reader1["idProduto"];
        }

        reader1.Close();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = @"SELECT * FROM Imagens i LEFT JOIN ImagensProdutos ip on i.idImagem = ip.imagemId
        WHERE i.corId = @corId AND ip.calcadoId = @calcadoId";
        cmd2.Parameters.AddWithValue("@calcadoId", estoque.calcadoId);
        cmd2.Parameters.AddWithValue("@corId", estoque.corId);

        SqlDataReader reader2 = cmd2.ExecuteReader();


        while (reader2.Read())
        {
            imagens.Add
            (
                new Imagem
                {
                    nomeImagem = (string)reader2["nomeImagem"],
                    statusImagem = (int)reader2["statusImagem"],
                    idImagem = (int)reader2["idImagem"]
                }
            );
        }

        reader2.Close();
        estoque.imagens = imagens;
        return estoque; 
    }


    public void Update(Estoque estoque)
    {
        int qtd = 0;

        SqlCommand cmd1 = new SqlCommand();
        cmd1.Connection = conn;
        cmd1.CommandText = @"
        SELECT qtd FROM Produtos
        WHERE idProduto = @idProduto";
        cmd1.Parameters.AddWithValue("@idProduto", estoque.idProduto);

        SqlDataReader reader1 = cmd1.ExecuteReader();

        if (reader1.Read())
        {
            qtd = (int)reader1["qtd"];
        }

        reader1.Close();

        if (qtd <= estoque.qtd)
        {
            SqlCommand cmd2 = new SqlCommand();
            cmd2.Connection = conn;
            cmd2.CommandText = @"
            INSERT INTO Entradas (idFornecedor) values(@idFornecedor);
            SELECT CAST(SCOPE_IDENTITY() AS int); ";
            cmd2.Parameters.AddWithValue("@idFornecedor", estoque.idFornecedor);

            int idEntrada = (int)cmd2.ExecuteScalar();

            SqlCommand cmd3 = new SqlCommand();
            cmd3.Connection = conn;
            cmd3.CommandText = @"
            INSERT INTO Itens_Entradas (idProduto, idEntrada, valorIE, qtdIE) values(@idProduto, @idEntrada, @valorIE, @qtd)";
            cmd3.Parameters.AddWithValue("@idProduto", estoque.idProduto);
            cmd3.Parameters.AddWithValue("@idEntrada", idEntrada);
            cmd3.Parameters.AddWithValue("@valorIE", estoque.valorIE);
            cmd3.Parameters.AddWithValue("@qtd", qtd);

            Console.WriteLine(estoque.idProduto);

            cmd3.ExecuteNonQuery();

        }

        SqlCommand cmd4 = new SqlCommand();
        cmd4.Connection = conn;
        cmd4.CommandText = @"
        UPDATE Produtos SET qtd = @qtd, preco = @preco, promocao = @promocao
        WHERE idProduto = @idProduto";
        cmd4.Parameters.AddWithValue("@qtd", estoque.qtd);
        cmd4.Parameters.AddWithValue("@preco", estoque.preco);
        cmd4.Parameters.AddWithValue("@promocao", estoque.promocao);
        cmd4.Parameters.AddWithValue("@idProduto", estoque.idProduto);
        
        cmd4.ExecuteNonQuery();

    }


    public void UpdateImg(Estoque estoque)
    {
        int imagemPrincipalAntigo = 0;

        SqlCommand cmd1 = new SqlCommand();
        cmd1.Connection = conn;
        cmd1.CommandText = @"SELECT idImagem FROM ImagensProdutos ip LEFT JOIN Imagens i on i.corId = ip.corId 
        WHERE i.corId = @corId AND i.statusImagem = 1 AND ip.calcadoId = @calcadoId";
        cmd1.Parameters.AddWithValue("@corId", estoque.corId);
        cmd1.Parameters.AddWithValue("@calcadoId", estoque.calcadoId);

        SqlDataReader reader1 = cmd1.ExecuteReader();

        if (reader1.Read())
        {
            imagemPrincipalAntigo = (int)reader1["idImagem"];
        }

        reader1.Close();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = @"
        UPDATE Imagens SET statusImagem = 0
        WHERE idImagem = @idImagem";
        cmd2.Parameters.AddWithValue("@idImagem", imagemPrincipalAntigo);

        cmd2.ExecuteNonQuery();



        SqlCommand cmd3 = new SqlCommand();
        cmd3.Connection = conn;
        cmd3.CommandText = @"
        UPDATE Imagens SET statusImagem = 1
        WHERE idImagem = @idImagem ";
        cmd3.Parameters.AddWithValue("@idImagem", estoque.idImagem);

        cmd3.ExecuteNonQuery();

    }
}
