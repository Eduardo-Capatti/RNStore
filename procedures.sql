use RNStoreOficial
go

create procedure sp_InsertFuncionario
(
@nome varchar(50),
@cpf varchar(14),
@email varchar(40),
@senha varchar(255),
@telefone varchar(15),
@salario money
)
as
begin
	set nocount on;
	begin transaction
	begin try
		insert into Pessoas (nomePessoa, cpf, email, senha, telefone) values (@nome, @cpf, @email, @senha, @telefone);

		declare @id int = SCOPE_IDENTITY();

		insert into Funcionarios (idPessoa, salario) values (@id, @salario);

		commit transaction;
	end try
	begin catch
		rollback transaction;
		throw;
	end catch
end
GO


create procedure sp_UpdateFuncionario
(
@idPessoa int,
@email varchar(40),
@telefone varchar(15),
@salario money
)
as
begin
	set nocount on;
	begin transaction
	begin try
		update Pessoas set email = @email, telefone = @telefone where idPessoa = @idPessoa;

		update Funcionarios set salario = @salario where idPessoa = @idPessoa;

		commit transaction;
	end try

	begin catch
		rollback transaction;
		throw;
	end catch
end
GO


create procedure sp_InsertImagem(@corId int, @calcadoId int, @nomeImagem varchar(100))
as
begin
	set nocount on;
	begin transaction
	begin try
		INSERT INTO Imagens (corId, nomeImagem, statusImagem) VALUES(@corId, @nomeImagem, 0);

		declare @id int = SCOPE_IDENTITY();

		INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) VALUES(@corId, @calcadoId, @id)

		commit transaction;
	end try

	begin catch
		rollback transaction;
		throw;
	end catch
end
GO

create procedure sp_UpdateImagem(@corId int, @calcadoId int, @idImagem int)
as
begin
	set nocount on;
	begin transaction
	begin try
		declare @idimagemPrincipalAntigo int =	isnull((SELECT i.idImagem 
							FROM ImagensProdutos ip LEFT JOIN Imagens i on i.idImagem = ip.imagemId 
							WHERE i.corId = @corId AND i.statusImagem = 1 AND ip.calcadoId = @calcadoId), 0)

		if (@idimagemPrincipalAntigo = 0)
		begin
			UPDATE Imagens SET statusImagem = 1 WHERE idImagem = @idImagem;
			commit transaction; 
		end
		else
		begin
			UPDATE Imagens SET statusImagem = 0 WHERE idImagem = @idimagemPrincipalAntigo

			UPDATE Imagens SET statusImagem = 1 WHERE idImagem = @idImagem;
			commit transaction; 
		end
	end try

	begin catch
		rollback transaction;
		throw;
	end catch
end
go

create procedure sp_DeleteImagem(@idImagem int)
as
begin
	set nocount on;
	begin transaction
	begin try
		delete from ImagensProdutos where imagemId = @idImagem;

		delete from Imagens where idImagem = @idImagem;

		commit transaction;
	end try

	begin catch
		rollback transaction;
		throw;
	end catch
end
GO


create procedure sp_UpdateProduto(@qtd int, @idFornecedor int, @idProduto int, @valorIE money, @preco money, @promocao money)
as
begin
	set nocount on;
	begin transaction
	begin try
		declare @qtdAntiga int = isnull((SELECT qtd FROM Produtos WHERE idProduto = @idProduto), 0)

		if @qtdAntiga <= @qtd
		begin
			INSERT INTO Entradas (idFornecedor) values(@idFornecedor);

			declare @idEntrada int = SCOPE_IDENTITY();

			INSERT INTO Itens_Entradas (idProduto, idEntrada, valorIE, qtdIE) values(@idProduto, @idEntrada, @valorIE, @qtd - @qtdAntiga);

			UPDATE Produtos SET qtd = @qtd, preco = @preco, promocao = @promocao WHERE idProduto = @idProduto

			commit transaction;

		end
		else
		begin
			UPDATE Produtos SET qtd = @qtd, preco = @preco, promocao = @promocao WHERE idProduto = @idProduto

			commit transaction;
		end
	end try
	begin catch
		rollback transaction;
		throw;
	end catch
end
GO


create procedure sp_ColocarCarrinhoAposLogin(@idCliente int, @idProduto int)
as
begin
	set nocount on;
	begin transaction
	begin try
		declare @precoCorrigido money = isnull((SELECT preco - isnull(promocao,0) FROM Produtos WHERE idProduto = @idProduto), 0);

		declare @idCompraExistente int = isnull((SELECT idCompra FROM Compras WHERE idCliente = @idCliente AND statusCompra = 0), 0);


		if @idCompraExistente != 0
		begin
			SELECT idCompra FROM Itens_Compra WHERE idProduto = @idProduto AND idCompra = @idCompraExistente

			declare @existeItem int = @@ROWCOUNT;

			if @existeItem = 0
			begin
				INSERT INTO Itens_Compra(idProduto, idCompra, qtdIC, valorIC) VALUES (@idProduto, @idCompraExistente, 1, @precoCorrigido);
			end	

			commit transaction;
		end
		else
		begin
			INSERT INTO Compras(idCliente, totalCompra, statusCompra) VALUES (@idCliente, @precoCorrigido, 0)

			declare @idCompra int = SCOPE_IDENTITY();

			INSERT INTO Itens_Compra(idProduto, idCompra, qtdIC, valorIC) VALUES (@idProduto, @idCompra, 1, @precoCorrigido);

			commit transaction;
		end
	end try
	begin catch
		rollback transaction;
		throw;
	end catch
end
GO

create procedure sp_CarrinhoExcluirItem(@idCompra int, @idProduto int)
as
begin
	set nocount on;
	begin transaction
	begin try
		delete from Itens_Compra WHERE idCompra = @idCompra AND idProduto = @idProduto;

		if not exists(SELECT idCompra from Itens_Compra WHERE idCompra = @idCompra)
		begin
			delete from Compras where idCompra = @idCompra;
		end

		commit transaction;
	end try
	begin catch
		rollback transaction;
		throw;
	end catch
end
GO


create procedure sp_CarrinhoAdicionarItem(@idCliente int, @idProduto int, @valorIC money)
as
begin
	set nocount on;
	begin transaction
	begin try
		declare @idCompra int = isnull((SELECT idCompra FROM Compras WHERE idCliente = @idCliente AND statusCompra = 0), 0);

		if @idCompra != 0
		begin
			INSERT INTO Itens_Compra(idProduto, idCompra, qtdIC, valorIC) VALUES (@idProduto, @idCompra, 1, @valorIC)
		end
		else
		begin
			INSERT INTO Compras(idCliente, totalCompra, statusCompra) VALUES (@idCliente, 0, 0)
			declare @novoIdCompra int = SCOPE_IDENTITY();

			INSERT INTO Itens_Compra(idProduto, idCompra, qtdIC, valorIC) VALUES (@idProduto, @novoIdCompra, 1, @valorIC)
		end
		commit transaction;
	end try
	begin catch
		rollback transaction;
		throw;
	end catch
end
GO
