use RNStoreOficial
go

CREATE TRIGGER tg_AtualizaTotalCompra 
ON Itens_Compra 
AFTER INSERT, UPDATE, DELETE 
AS 
BEGIN 
    UPDATE c 
    SET c.totalCompra = ISNULL((SELECT SUM(ic.qtdIC * ic.valorIC)  
                                FROM Itens_Compra ic  
                                WHERE ic.idCompra = c.idCompra), 0) 
    FROM Compras c 
    WHERE c.idCompra IN (SELECT DISTINCT idCompra FROM inserted  
                         UNION  
                         SELECT DISTINCT idCompra FROM deleted) 
END 
GO 

CREATE TRIGGER tg_AtualizaItensCompra
ON Produtos
AFTER INSERT, UPDATE, DELETE 
AS 
BEGIN 
	UPDATE ic
	SET ic.valorIC = ISNULL(p.preco, 0)
	FROM Itens_Compra ic
	INNER JOIN Produtos p ON p.idProduto = ic.idProduto
	WHERE ic.idProduto IN (
		SELECT DISTINCT idProduto FROM inserted
		UNION
		SELECT DISTINCT idProduto FROM deleted
	)
END 
GO 