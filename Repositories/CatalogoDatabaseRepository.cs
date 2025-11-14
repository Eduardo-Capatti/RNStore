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
        ),
        ImagensEscolhidas AS (
            SELECT 
        ip.calcadoId,
        ip.corId,
        i.nomeImagem,
        ROW_NUMBER() OVER (
            PARTITION BY ip.calcadoId 
            ORDER BY 
                CASE WHEN i.nomeImagem IS NOT NULL THEN 0 ELSE 1 END,  -- prioriza imagens não nulas
                i.idImagem ASC
        ) AS rn
            FROM ImagensProdutos ip
        LEFT JOIN Imagens i ON i.idImagem = ip.imagemId AND i.statusImagem = 1
    )
SELECT 
    c.idCalcado,
    pe.idProduto,
    c.nomeCalcado,
    m.nomeMarca,
    pe.preco,
    pe.promocao,
    ie.nomeImagem
FROM Calcados c
JOIN ProdEscolhido pe 
    ON c.idCalcado = pe.calcadoId AND pe.rn = 1
JOIN Marca m 
    ON m.idMarca = c.marcaId
LEFT JOIN ImagensEscolhidas ie 
    ON ie.calcadoId = c.idCalcado AND ie.rn = 1;";

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
        return catalogos;
    }


    public List<Catalogo> Buscar(string buscarProduto)
    {
        List<Catalogo> catalogos = new List<Catalogo>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = $@"
            
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
        ),
        ImagensEscolhidas AS (
            SELECT 
        ip.calcadoId,
        ip.corId,
        i.nomeImagem,
        ROW_NUMBER() OVER (
            PARTITION BY ip.calcadoId 
            ORDER BY 
                CASE WHEN i.nomeImagem IS NOT NULL THEN 0 ELSE 1 END,  -- prioriza imagens não nulas
                i.idImagem ASC
        ) AS rn
            FROM ImagensProdutos ip
        LEFT JOIN Imagens i ON i.idImagem = ip.imagemId AND i.statusImagem = 1
    )
SELECT 
    c.idCalcado,
    pe.idProduto,
    c.nomeCalcado,
    m.nomeMarca,
    pe.preco,
    pe.promocao,
    ie.nomeImagem
FROM Calcados c
JOIN ProdEscolhido pe 
    ON c.idCalcado = pe.calcadoId AND pe.rn = 1
JOIN Marca m 
    ON m.idMarca = c.marcaId
LEFT JOIN ImagensEscolhidas ie 
    ON ie.calcadoId = c.idCalcado AND ie.rn = 1
WHERE c.nomeCalcado like(@busca) OR m.nomeMarca like(@busca)
;";

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
        string sql = @"
        SELECT c.idCor, c.nomeCor 
        FROM Cores c
        JOIN CoresCalcados cc ON c.idCor = cc.corId
        WHERE cc.calcadoId = @id";

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

        var where = $"corId IN ({string.Join(",", listaCores)})";


        using (SqlCommand cmd = new SqlCommand(
           $"SELECT idImagem, nomeImagem FROM Imagens WHERE {where} AND statusImagem = 1", conn))
        {

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

        //6)Pega produtos relacionados
        List<Catalogo> catalogos = new List<Catalogo>();

        SqlCommand cmdCatalogo = new SqlCommand();
        cmdCatalogo.Connection = conn;
        cmdCatalogo.CommandText = @"
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
                ),
                ImagensEscolhidas AS (
                    SELECT 
                ip.calcadoId,
                ip.corId,
                i.nomeImagem,
                ROW_NUMBER() OVER (
                    PARTITION BY ip.calcadoId 
                    ORDER BY 
                        CASE WHEN i.nomeImagem IS NOT NULL THEN 0 ELSE 1 END,  -- prioriza imagens não nulas
                        i.idImagem ASC
                ) AS rn
                    FROM ImagensProdutos ip
                LEFT JOIN Imagens i ON i.idImagem = ip.imagemId AND i.statusImagem = 1
            )
        SELECT top 9
            c.idCalcado,
            pe.idProduto,
            c.nomeCalcado,
            m.nomeMarca,
            pe.preco,
            pe.promocao,
            ie.nomeImagem
        FROM Calcados c
        JOIN ProdEscolhido pe 
            ON c.idCalcado = pe.calcadoId AND pe.rn = 1
        JOIN Marca m 
            ON m.idMarca = c.marcaId
        LEFT JOIN ImagensEscolhidas ie 
            ON ie.calcadoId = c.idCalcado AND ie.rn = 1
        WHERE pe.idProduto != @idProduto
        ORDER BY 
            CASE 
                WHEN idCalcado = @idCalcado THEN 0 
                ELSE 1 
            END,
            idCalcado
        ;";

        cmdCatalogo.Parameters.AddWithValue("@idProduto", idProduto);
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
                        imgOutrasCores = imagensOutrasCores,
                        catalogo = catalogos
                    };
                }
            }
        }

        return null;
    }
}
