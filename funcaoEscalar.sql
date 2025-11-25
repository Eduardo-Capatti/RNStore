use RNStoreOficial
go

create function fn_CalcularPrecoFinal(@preco money, @promocao money)
returns money
as
begin

	declare @precoFinal money

	set @precoFinal = @preco - ISNULL(@promocao, 0)

	if (@precoFinal < 0)
		set @precoFinal = 0

	return @precoFinal
end
GO


create function fn_VerificarDuplicataFuncionario(@email varchar(40), @telefone varchar(15), @cpf varchar(14))
returns varchar(10) 
as
begin
	declare @duplicata varchar(10)

	SELECT @duplicata = 
	CASE WHEN EXISTS (SELECT 1 FROM Pessoas WHERE email = @email) THEN 'email'
		WHEN EXISTS (SELECT 1 FROM Pessoas WHERE telefone = @telefone) THEN 'telefone'
		WHEN EXISTS (SELECT 1 FROM Pessoas WHERE cpf = @cpf) THEN 'cpf'
		ELSE 'ok'
	END

	return @duplicata
end
go

create function fn_VerificarDuplicataFornecedor(@cnpj varchar(18))
returns varchar(4) 
as
begin
	declare @duplicata varchar(10)

	SELECT @duplicata = 
    CASE 
        WHEN EXISTS (SELECT 1 FROM Fornecedor WHERE cnpj = @cnpj) THEN 'cnpj'
        ELSE 'ok'
    END

	return @duplicata
end
go

create function fn_ObterStatusEstoque(@qtd int)
returns varchar(20)
as
begin
	declare @status varchar(20)

	if (@qtd <= 0)
		set @status = 'Esgotado'
	else if (@qtd <= 3)
		set @status = 'Em baixa'
	else
		set @status = 'Disponível'
	return @status
end
go


create function fn_IsOnCart(@idProduto int, @idCliente int)
returns bit
as
begin
	declare @idCompra int = isnull((SELECT idCompra FROM Compras WHERE idCliente = @idCliente AND statusCompra = 0),0);
	
	if @idCompra != 0
	begin
		if exists(SELECT idCompra FROM Itens_Compra WHERE idCompra = @idCompra AND idProduto = @idProduto)
		begin 
			return 1;
		end
		return 0;
	end

	return 0
end
go
