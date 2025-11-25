use RNStoreOficial
go

create view v_BuscarProdutosEstoque
as
SELECT DISTINCT p.idProduto, c.nomeCalcado, t.tamanho, co.nomeCor, p.preco, p.corId, p.calcadoId, f.nomeForn, m.nomeMarca, p.qtd, p.promocao, dbo.fn_ObterStatusEstoque(qtd) as statusEstoque
FROM Produtos p 
LEFT JOIN Calcados c on c.idCalcado = p.calcadoId 
LEFT JOIN Cores co on p.corId = co.idCor 
LEFT JOIN Tamanhos t on p.tamanhoId = t.idTamanho
LEFT JOIN Marca m on m.idMarca = c.marcaId
LEFT JOIN Itens_Entradas ie on ie.idProduto = p.idProduto
LEFT JOIN Entradas e on e.idEntrada = ie.idEntrada
LEFT JOIN Fornecedor f on f.idFornecedor = e.idFornecedor
go

select * from Fornecedor
select * from Produtos
create view v_BuscarPorProdutosCatalogo
as
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
    ON ie.calcadoId = c.idCalcado AND  ie.corId = pe.corId AND ie.rn = 1  AND ie.corId = pe.corId
go

create view v_BuscarVendas
as
SELECT distinct c.idCompra, c.data_entrega, c.dataCompra, c.statusCompra, p.nomePessoa, p.email, c.totalCompra, e.bairro, e.cidade, e.numero, e.rua
FROM Compras c 
LEFT JOIN Pessoas p on c.idCliente = p.idPessoa  
LEFT JOIN Itens_Compra ic on c.idCompra = ic.idCompra 
LEFT JOIN Produtos pr on pr.idProduto = ic.idProduto 
LEFT JOIN Enderecos e on e.idCliente = p.idPessoa
WHERE c.statusCompra != 0 
