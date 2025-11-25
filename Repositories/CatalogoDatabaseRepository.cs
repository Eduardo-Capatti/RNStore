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
        cmd.CommandText = @"select * from v_BuscarPorProdutosCatalogo";

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            List<string> img = new List<string>();
            if (reader["nomeImagem"] != DBNull.Value)
            {
                img.Add((string)reader["nomeImagem"]);
            }

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
        reader.Close();
        return catalogos;
    }


    public List<Catalogo> Buscar(string buscarProduto)
    {
        List<Catalogo> catalogos = new List<Catalogo>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = $@"select * from fn_BuscarProdutoEspecifico(@busca);";

cmd.Parameters.AddWithValue("@busca", "%" + buscarProduto + "%");

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            List<string> img = new List<string>();
            if (reader["nomeImagem"] != DBNull.Value)
            {
                img.Add((string)reader["nomeImagem"]);
            }

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
        reader.Close();
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
        using (SqlCommand cmd = new SqlCommand(@"select * from fn_TamanhoDisponivelPorCalcadoECor(@calcado, @cor)", conn))
        {
            cmd.Parameters.AddWithValue("@calcado", idCalcado);
            cmd.Parameters.AddWithValue("@cor", corPrincipal);
            cmd.Parameters.AddWithValue("@idProduto", idProduto);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    tamanhos.Add(new Tamanho
                    {
                        idTamanho = (int)reader["idTamanho"],
                        tamanho = (string)reader["tamanho"],
                        idProduto = (int)reader["idProduto"],
                        calcadoId = (int)reader["calcadoId"]
                    });
                }
            }
        }

        // 3) Pega todas cores do calçado
        List<Cor> cores = new List<Cor>();
        List<int> listaCores = new List<int>();
        string sql = @"select * from fn_CorDisponivelPorCalcado(@id)";

        using (SqlCommand cmd = new SqlCommand(sql, conn))
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
            @"SELECT i.idImagem, i.nomeImagem FROM Imagens i LEFT JOIN ImagensProdutos ip on ip.imagemId = i.idImagem WHERE ip.corId = @cor AND ip.calcadoId = @calcadoId 
            ORDER BY statusImagem DESC", conn))
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

        // 5) Pega imagem principal das outras cores
        List<Imagem> imagensOutrasCores = new List<Imagem>();

        var where = "";

        if (listaCores.Any())
        {
            where = $"ip.corId IN ({string.Join(",", listaCores)}) AND";
        }

        using (SqlCommand cmd = new SqlCommand(
           @$"WITH ImagemPrincipalProdutoCor AS (
            SELECT 
                p.idProduto,
                p.calcadoId,
				p.corId,
                p.qtd,
                ROW_NUMBER() OVER (
                    PARTITION BY p.corId 
					ORDER BY p.corId
                ) AS rn
            FROM Produtos p
            WHERE p.calcadoId = @calcadoId
                )
            SELECT 
                i.idImagem, i.nomeImagem, ippc.idProduto, ippc.calcadoId
            FROM Imagens i 
            LEFT JOIN ImagensProdutos ip on ip.imagemId = i.idImagem 
            LEFT JOIN ImagemPrincipalProdutoCor ippc ON ip.corId = ippc.corId AND ippc.rn = 1
            WHERE {where} ip.calcadoId = @calcadoId AND statusImagem = 1 AND ip.corId != @corPrincipal AND ippc.qtd > 0", conn))
        {
            cmd.Parameters.AddWithValue("@calcadoId", idCalcado);
            cmd.Parameters.AddWithValue("@corPrincipal", corPrincipal);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    imagensOutrasCores.Add(new Imagem
                    {
                        idProduto = (int)reader["idProduto"],
                        calcadoId = (int)reader["calcadoId"],
                        idImagem = (int)reader["idImagem"],
                        nomeImagem = (string)reader["nomeImagem"]
                    });
                }
            }
        }

        //6)Pega produtos relacionados
        List<Catalogo> catalogos = new List<Catalogo>();

        SqlCommand cmdCatalogo = new SqlCommand();
        cmdCatalogo.Connection = conn;
        cmdCatalogo.CommandText = @"select * from fn_MostrarOutrosProdutos(@idCalcado);";

        cmdCatalogo.Parameters.AddWithValue("@idCalcado", idCalcado);

        SqlDataReader readerCatalogo = cmdCatalogo.ExecuteReader();

        while (readerCatalogo.Read())
        {
            List<string> img = new List<string>();
            if (readerCatalogo["nomeImagem"] != DBNull.Value)
            {
                img.Add((string)readerCatalogo["nomeImagem"]);
            }

            catalogos.Add
            (
                new Catalogo
                {
                    idProduto = (int)readerCatalogo["idProduto"],
                    calcadoId = (int)readerCatalogo["idCalcado"],
                    nomeCalcado = (string)readerCatalogo["nomeCalcado"],
                    marcaCalcado = (string)readerCatalogo["nomeMarca"],
                    preco = (decimal)readerCatalogo["preco"],
                    promocao = readerCatalogo["promocao"] != DBNull.Value ? (decimal?)readerCatalogo["promocao"] : null,
                    ApenasImagemPrincipal = img,
                }

            );
        }

        readerCatalogo.Close();

        // 7) Pega dados gerais do produto
        using (SqlCommand cmd = new SqlCommand(@"select * from fn_DadosProduto(@id)", conn))
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
                        tamanhoId = (int)reader["tamanhoId"],
                        cor = cores,
                        corPrincipal = corPrincipalNome,
                        tamanho = tamanhos,
                        marcaCalcado = (string)reader["nomeMarca"],
                        preco = (decimal)reader["preco"],
                        promocao = reader["promocao"] is DBNull ? 0 : (decimal)reader["promocao"],
                        img = imagens,
                        imgOutrasCores = imagensOutrasCores,
                        catalogo = catalogos
                    };
                }
            }
        }

        return null;
    }


    public List<Compra> Read(int? idCliente)
    {
        List<Compra> compras = new List<Compra>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"select * from dbo.fn_BuscarCarrinhoCliente(@idCliente)";
        cmd.Parameters.AddWithValue("@idCliente", idCliente);

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            compras.Add(
                new Compra
                {
                    idCompra = (int)reader["idCompra"],
                    totalCompra = (decimal)reader["totalCompra"],
                    valorIC = (decimal)reader["valorIC"],
                    qtdIC = (int)reader["qtdIC"],
                    preco = (decimal)reader["preco"],
                    promocao = reader["promocao"] != DBNull.Value ? (decimal)reader["promocao"] : null,
                    idProduto = (int)reader["idProduto"],
                    nomeCalcado = (string)reader["nomeCalcado"],
                    nomeCor = (string)reader["nomeCor"],
                    tamanho = (string)reader["tamanho"],
                    nomeImagem = (string)reader["nomeImagem"],
                    marcaCalcado = (string)reader["nomeMarca"],
                    qtdDisponivel = (int)reader["qtd"]
                }
            );
        }
        reader.Close();

        return compras;

    }


    public void RemoverQtdCarrinho(int idCompra, int idProduto, decimal valorIC)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
        UPDATE Compras SET totalCompra = totalCompra - @valorIC WHERE idCompra = @idCompra;
        UPDATE Itens_Compra SET qtdIC = qtdIC - 1 WHERE idCompra = @idCompra AND idProduto = @idProduto";
        cmd.Parameters.AddWithValue("@idCompra", idCompra);
        cmd.Parameters.AddWithValue("@idProduto", idProduto);
        cmd.Parameters.AddWithValue("@valorIC", valorIC);

        cmd.ExecuteNonQuery(); 
    }


    public void AdicionarQtdCarrinho(int idCompra, int idProduto, decimal valorIC)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
        UPDATE Compras SET totalCompra = totalCompra + @valorIC WHERE idCompra = @idCompra;
        UPDATE Itens_Compra SET qtdIC = qtdIC + 1 WHERE idCompra = @idCompra AND idProduto = @idProduto";
        cmd.Parameters.AddWithValue("@idCompra", idCompra);
        cmd.Parameters.AddWithValue("@idProduto", idProduto);
        cmd.Parameters.AddWithValue("@valorIC", valorIC);

        cmd.ExecuteNonQuery();
    }

    public void Delete(Compra compra)
    {   

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;
        cmd.CommandText = @"exec sp_CarrinhoExcluirItem @idCompra, @idProduto";
        cmd.Parameters.AddWithValue("@idCompra", compra.idCompra);
        cmd.Parameters.AddWithValue("@idProduto", compra.idProduto);

        cmd.ExecuteNonQuery();

    }


    public void ConfirmarCompra(int idCompra)
    {
        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;
        cmd.CommandText = @"UPDATE Compras SET data_entrega = dateadd(day, 7, GETDATE()), statusCompra = 1 WHERE idCompra = @idCompra";
        cmd.Parameters.AddWithValue("@idCompra", idCompra);

        cmd.ExecuteNonQuery();
    }


    public void Carrinho(Catalogo catalogo, int? idCliente)
    {   
        decimal? novoPreco = catalogo.preco - (catalogo.promocao ?? 0);

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"exec sp_CarrinhoAdicionarItem @idCliente, @idProduto, @valorIC;";

        cmd.Parameters.AddWithValue("@idCliente", idCliente);
        cmd.Parameters.AddWithValue("@idProduto", catalogo.idProduto);
        cmd.Parameters.AddWithValue("@valorIC", novoPreco);

        cmd.ExecuteNonQuery();
    }

    public bool IsOnCart(int idProduto, int? idCliente)
    {
        bool result = false;

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"SELECT dbo.fn_IsOnCart(@idProduto, @idCliente) as result";
        cmd.Parameters.AddWithValue("@idProduto", idProduto);
        cmd.Parameters.AddWithValue("@idCliente", idCliente);

        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            result = (bool)reader["result"];
        }

        return result;

    }

}

