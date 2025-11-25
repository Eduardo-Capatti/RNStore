use RNStoreOficial
go

create function fn_DadosProduto(@idProduto int)
returns table 
as
return(
	SELECT p.idProduto, c.nomeCalcado, m.nomeMarca, p.preco, p.promocao, p.tamanhoId
	FROM Produtos p
	JOIN Calcados c ON p.calcadoId = c.idCalcado
	JOIN Marca m ON c.marcaId = m.idMarca
	WHERE p.idProduto = @idProduto
)
go


create function fn_BuscarProdutoEspecifico(@busca text)
returns table
as
return(
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
					WHEN p.promocao IS NOT NULL AND p.promocao > 0 THEN 0
					ELSE 1
					END ASC,
					CASE 
						WHEN p.promocao IS NOT NULL AND p.promocao > 0 THEN p.promocao
						ELSE p.preco
					END ASC
			) AS rn
		FROM Produtos p
		WHERE p.qtd > 0
			),
			ImagensEscolhidas AS (
				SELECT 
			ip.calcadoId,
			ip.corId,
			i.nomeImagem,
			ROW_NUMBER() OVER (
				PARTITION BY ip.calcadoId, ip.corId
				ORDER BY 
					CASE WHEN i.nomeImagem IS NOT NULL THEN 0 ELSE 1 END,  -- prioriza imagens não nulas
					i.idImagem ASC
			) AS rn
				FROM ImagensProdutos ip
			LEFT JOIN Imagens i ON i.idImagem = ip.imagemId 
			WHERE i.statusImagem = 1 AND i.nomeImagem is not null
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
	INNER JOIN ImagensEscolhidas ie 
		ON ie.calcadoId = c.idCalcado AND  ie.corId = pe.corId AND ie.rn = 1 AND ie.corId = pe.corId
	WHERE c.nomeCalcado like(@busca) OR m.nomeMarca like(@busca) 
)
go

create function fn_MostrarOutrosProdutos(@idCalcado int)
returns table
as
return(
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
					WHEN p.promocao IS NOT NULL AND p.promocao > 0 THEN 0
					ELSE 1
					END ASC,
					CASE 
						WHEN p.promocao IS NOT NULL AND p.promocao > 0 THEN p.promocao
						ELSE p.preco
					END ASC
                ) AS rn
            FROM Produtos p
            WHERE p.qtd > 0
                ),
                ImagensEscolhidas AS (
                    SELECT 
                ip.calcadoId,
                ip.corId,
                i.nomeImagem,
                ROW_NUMBER() OVER (
                    PARTITION BY ip.calcadoId, ip.corId
                    ORDER BY 
                        CASE WHEN i.nomeImagem IS NOT NULL THEN 0 ELSE 1 END,  -- prioriza imagens não nulas
                        i.idImagem ASC
                ) AS rn
                    FROM ImagensProdutos ip
                LEFT JOIN Imagens i ON i.idImagem = ip.imagemId 
				WHERE i.statusImagem = 1 AND i.nomeImagem is not null
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
        INNER JOIN ImagensEscolhidas ie 
            ON ie.calcadoId = c.idCalcado AND ie.corId = pe.corId AND ie.rn = 1  AND ie.corId = pe.corId
        WHERE pe.calcadoId != @idCalcado
)
go

create function fn_TamanhoDisponivelPorCalcadoECor(@idCalcado int, @idCor int)
returns table
as
return (
	SELECT DISTINCT t.idTamanho, t.tamanho, p.idProduto, p.calcadoId
    FROM Produtos p
    JOIN Tamanhos t ON p.tamanhoId = t.idTamanho
    WHERE p.calcadoId = @idCalcado AND p.corId = @idCor AND p.qtd > 0
)
go

create function fn_CorDisponivelPorCalcado(@idCalcado int)
returns table
as
return( 
	SELECT c.idCor, c.nomeCor 
    FROM Cores c
    JOIN CoresCalcados cc ON c.idCor = cc.corId
    WHERE cc.calcadoId = 1
)
go

create function fn_BuscarCarrinhoCliente(@idCliente int)
returns table
as
return(
	SELECT  c.idCompra, c.totalCompra, ic.qtdIC, ic.valorIC, p.idProduto, p.preco, p.promocao, p.qtd,
			ca.nomeCalcado, m.nomeMarca, co.nomeCor, t.tamanho, i.nomeImagem
    FROM Compras c LEFT JOIN Itens_Compra ic on c.idCompra = ic.idCompra LEFT JOIN Produtos p on p.idProduto = ic.idProduto 
    LEFT JOIN Calcados ca on ca.idCalcado = p.calcadoId
    LEFT JOIN Marca m on m.idMarca = ca.marcaId
    LEFT JOIN Cores co on co.idCor = p.corId 
    LEFT JOIN Tamanhos t on p.tamanhoId = t.idTamanho 
    LEFT JOIN ImagensProdutos ip on ip.calcadoId = ca.idCalcado AND ip.corId = co.idCor 
    LEFT JOIN Imagens i on i.idImagem = ip.imagemId
    WHERE c.idCliente = @idCliente AND c.statusCompra = 0 AND i.statusImagem = 1
)
go

create function fn_BuscarItensCompra(@idCompra int)
returns table
as
return(
	select ic.idCompra, ic.qtdIC, ic.valorIC, ca.nomeCalcado, co.nomeCor, t.tamanho, m.nomeMarca 
	from Itens_Compra ic
    LEFT JOIN Produtos pr on pr.idProduto = ic.idProduto 
    LEFT JOIN Calcados ca on ca.idCalcado = pr.calcadoId 
    LEFT JOIN Marca m on m.idMarca = ca.marcaId 
    LEFT JOIN Cores co on co.idCor = pr.corId 
    LEFT JOIN Tamanhos t on t.idTamanho = pr.tamanhoId 
    WHERE ic.idCompra = @idCompra
)
go