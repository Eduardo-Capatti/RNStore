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


    public int Verificar(Estoque estoque)
    {
        SqlCommand cmdVerificar = new SqlCommand();
        cmdVerificar.Connection = conn;
        cmdVerificar.CommandText = @"SELECT top 1 p.idProduto, e.idFornecedor FROM Produtos p 
        LEFT JOIN Itens_Entradas ie on ie.idProduto = p.idProduto 
        LEFT JOIN Entradas e on ie.idEntrada = e.idEntrada 
        WHERE p.calcadoId = @calcadoId AND p.corId = @corId AND e.idFornecedor = @idFornecedor AND p.tamanhoId = @tamanhoId
        ";
        cmdVerificar.Parameters.AddWithValue("@calcadoId", estoque.calcadoId);
        cmdVerificar.Parameters.AddWithValue("@corId", estoque.corId);
        cmdVerificar.Parameters.AddWithValue("@idFornecedor", estoque.idFornecedor);
        cmdVerificar.Parameters.AddWithValue("@tamanhoId", estoque.tamanhoId);

        SqlDataReader readerVerificar = cmdVerificar.ExecuteReader();
        if (readerVerificar.Read())
        {

            int idProduto = (int)readerVerificar["idProduto"];
            readerVerificar.Close();

            SqlCommand cmdUpdate = new SqlCommand();
            cmdUpdate.Connection = conn;
            cmdUpdate.CommandText = "UPDATE Produtos set qtd = qtd + @qtd, preco = @preco WHERE idProduto = @idProduto";
            cmdUpdate.Parameters.AddWithValue("@qtd", estoque.qtd);
            cmdUpdate.Parameters.AddWithValue("@preco", estoque.preco);
            cmdUpdate.Parameters.AddWithValue("@idProduto", idProduto);
            cmdUpdate.ExecuteNonQuery();


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

            return 1;
        }

        readerVerificar.Close();

        return 0;
    }


    public Filtro Filtros()
    {
        List<Cor> cores = new List<Cor>();

        SqlCommand cmdCor = new SqlCommand();
        cmdCor.Connection = conn;
        cmdCor.CommandText = @"SELECT * from Cores";
        SqlDataReader readerCor = cmdCor.ExecuteReader();

        while (readerCor.Read())
        {
            cores.Add(
                new Cor
                {
                    idCor = (int)readerCor["idCor"],
                    nomeCor = (string)readerCor["nomeCor"]
                }
            );
        }

        readerCor.Close();


        List<Tamanho> tamanhos = new List<Tamanho>();

        SqlCommand cmdTamanho = new SqlCommand();
        cmdTamanho.Connection = conn;
        cmdTamanho.CommandText = @"SELECT * from Tamanhos";
        SqlDataReader readerTamanho = cmdTamanho.ExecuteReader();

        while (readerTamanho.Read())
        {
            tamanhos.Add(
                new Tamanho
                {
                    idTamanho = (int)readerTamanho["idTamanho"],
                    tamanho = (string)readerTamanho["tamanho"]
                }
            );
        }

        readerTamanho.Close();


        List<Marca> marcas = new List<Marca>();

        SqlCommand cmdMarca = new SqlCommand();
        cmdMarca.Connection = conn;
        cmdMarca.CommandText = @"SELECT * from Marca";
        SqlDataReader readerMarca = cmdMarca.ExecuteReader();

        while (readerMarca.Read())
        {
            marcas.Add(
                new Marca
                {
                    idMarca = (int)readerMarca["idMarca"],
                    nomeMarca = (string)readerMarca["nomeMarca"]
                }
            );
        }

        readerMarca.Close();

        return new Filtro { cores = cores, tamanhos = tamanhos, marcas = marcas };
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
        cmd.CommandText = @"exec sp_InsertImagem @corId, @calcadoId, @nomeImagem;";
        cmd.Parameters.AddWithValue("@corId", estoque.corId);
        cmd.Parameters.AddWithValue("@nomeImagem", estoque.img);
        cmd.Parameters.AddWithValue("@calcadoId", estoque.calcadoId);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int idProduto)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Produtos WHERE idProduto = @idProduto";
        cmd.Parameters.AddWithValue("idProduto", idProduto);

        cmd.ExecuteNonQuery();
    }

    public void DeleteImg(int idImagem)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "exec sp_DeleteImagem @idImagem";
        cmd.Parameters.AddWithValue("@idImagem", idImagem);

        cmd.ExecuteNonQuery();
    }

    public List<Estoque> Read()
    {
        List<Estoque> Estoque = new List<Estoque>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"select * from v_BuscarProdutosEstoque";
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
                    nomeFornecedor = (string)reader["nomeForn"],
                    nomeMarca = (string)reader["nomeMarca"],
                    qtd = (int)reader["qtd"],
                    promocao = reader["promocao"] is DBNull ? 0 : (decimal)reader["promocao"],
                    statusEstoque = (string)reader["statusEstoque"]
                }
            );
        }
        return Estoque;
    }

    public List<Estoque> ReadFiltro(List<int> coresSelecionadas, List<int> tamanhosSelecionados, List<int> marcasSelecionadas, string nomeCalcado, int idProduto, decimal valorMinimo, decimal valorMaximo)
    {
        List<string> condicoes = new List<string>();

        if (coresSelecionadas != null && coresSelecionadas.Any())
        {
            condicoes.Add($"corId IN ({string.Join(",", coresSelecionadas)})");
        }
            
        if (tamanhosSelecionados != null && tamanhosSelecionados.Any())
        {
            condicoes.Add($"p.tamanhoId IN ({string.Join(",", tamanhosSelecionados)})");
        }

        if (marcasSelecionadas != null && marcasSelecionadas.Any())
        {
            condicoes.Add($"c.marcaId IN ({string.Join(",", marcasSelecionadas)})");
        }

        if (nomeCalcado != null)
        {
            condicoes.Add($"c.nomeCalcado LIKE ('%{nomeCalcado}%')");
        }

        if(idProduto != 0)
        {
            condicoes.Add($"p.idProduto = {idProduto}");
        }

        if(valorMaximo != 0)
        {
            condicoes.Add($"p.preco between {valorMinimo} AND {valorMaximo}");
        }

            
        string where = condicoes.Any() ? "WHERE " + string.Join(" AND ", condicoes) : "";
    
        List<Estoque> Estoque = new List<Estoque>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = $@"SELECT distinct p.idProduto, c.nomeCalcado, t.tamanho, co.nomeCor, p.preco, p.corId, p.calcadoId, f.nomeForn, m.nomeMarca, p.qtd, p.promocao 
        FROM Produtos p LEFT JOIN Calcados c on c.idCalcado = p.calcadoId 
        LEFT JOIN Cores co on p.corId = co.idCor 
        LEFT JOIN Tamanhos t on p.tamanhoId = t.idTamanho
        LEFT JOIN Marca m on m.idMarca = c.marcaId
		LEFT JOIN Itens_Entradas ie on ie.idProduto = p.idProduto
		LEFT JOIN Entradas e on e.idEntrada = ie.idEntrada
		LEFT JOIN Fornecedor f on f.idFornecedor = e.idFornecedor
        {where}";
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
                    nomeFornecedor = (string)reader["nomeForn"],
                    nomeMarca = (string)reader["nomeMarca"],
                    qtd = (int)reader["qtd"],
                    promocao = reader["promocao"] is DBNull ? 0 : (decimal)reader["promocao"]
                }
            );
        }

        reader.Close();
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
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"exec sp_UpdateProduto @qtd, @idFornecedor, @idProduto, @valorIE, @preco, @promocao";
        cmd.Parameters.AddWithValue("@qtd", estoque.qtd);
        cmd.Parameters.AddWithValue("@idFornecedor", estoque.idFornecedor);
        cmd.Parameters.AddWithValue("@idProduto", estoque.idProduto);
        cmd.Parameters.AddWithValue("@valorIE", estoque.valorIE);
        cmd.Parameters.AddWithValue("@preco", estoque.preco);
        cmd.Parameters.AddWithValue("@promocao", estoque.promocao);

        cmd.ExecuteNonQuery();

    }


    public void UpdateImg(Estoque estoque)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"exec sp_UpdateImagem @corId, @calcadoId, @idImagem";
        cmd.Parameters.AddWithValue("@corId", estoque.corId);
        cmd.Parameters.AddWithValue("@calcadoId", estoque.calcadoId);
        cmd.Parameters.AddWithValue("@idImagem", estoque.idImagem);

        cmd.ExecuteNonQuery();

    }
}
