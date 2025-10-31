--drop database RNCalcados
--use master
create database RNCalcados
go
use RNCalcados

create table Pessoas(
	idPessoa     int       not null   primary key  identity,
	nomePessoa   varchar(50)  not null,
	cpf          varchar(14)  not null unique,
	email        varchar(40) not null unique,
	senha		 varchar(255) not null,
	telefone	 varchar(14) null 	
)


create table Funcionarios(
	idPessoa     int	      not null	  primary key references Pessoas(idPessoa),
	salario      money        not null
)

create table Clientes(
	idPessoa     int	      not null	  primary key references Pessoas(idPessoa),
	status       int          not null	default 1 check(status in(1,2)) -- 1 - ativo, 2 - inativo
)

create table Enderecos(
	idEndereco			 int	   not null	 primary key identity,
	cep			 char(8)   not null,
	rua          varchar(40)  not null,
	complemento  varchar(40)	   null,
	numero		 varchar(5)   not null,
	bairro       varchar(30)  not null,
	cidade		 varchar(40)  not null,
	uf			 char(2) not null,
	idCliente	 int not null references Clientes(idPessoa)
)

create table Slider(
	idSlider  int not null primary key identity,
	img		  varchar(100) not null,
	status    int not null default 1 check(status in(1,2)), -- 1 - ativo, 2 - inativo
	idFuncionario int null references Funcionarios(idPessoa)
)

create table Fornecedor(
	idFornecedor  int	      not null	  primary key identity,
	cnpj		  varchar(18) not null	  unique,
	nomeForn      varchar(20)  not null
)

create table Marca(
	idMarca		 int	      not null	  primary key identity,
	nomeMarca    varchar(20)  not null
)

create table Calcados(
	idCalcado    int          not null    primary key identity,
	marcaId      int          not null    references Marca(idMarca),
	nomeCalcado  varchar(50)  not null
)

create table Tamanhos(
	idTamanho	int not null primary key identity,
	tamanho		varchar(30) not null
)



create table Produtos(
	idProduto	 int		  not null	  primary key identity,
	calcadoId    int          not null    references Calcados(idCalcado),
	cor          varchar(30)  not null,
	promocao     money            null,
	preco        money        not null,
	qtd			 int		  not null   default 0,
	tamanho		 int		  not null	 references Tamanhos(idTamanho)
)	

create table Imagens(
	idImagem int not null primary key identity,
	nome     varchar(100) not null,
	idProduto int not null references Produtos(idProduto)
)

create table Compras(
	idCompra     int			not null     primary key identity,
	dataCompra	 date			not null	 default GETDATE(),	  
	totalCompra	 decimal(10,2)      null,
	statusCompra int				null	 check(statusCompra in(1,2,3)),
	data_entrega date			not null,
	idCliente int not null references Clientes(idPessoa),
	idFuncionario int not null references Funcionarios(idPessoa)
)

create table Itens_Compra(
	idProduto int not null references Produtos(idProduto),
	idCompra int not null references Compras(idCompra),
	qtdIC int null,
	valorIC money null,
	primary key(idProduto, idCompra)
)

create table Entradas(
	idEntrada int not null primary key identity,
	dataEntrada date not null default GETDATE(),
	idFornecedor int not null references Fornecedor(idFornecedor)
)

create table Itens_Entradas(
	idProduto int not null references Produtos(idProduto),
	idEntrada int not null references Entradas(idEntrada),
	valorIE money null,
	qtdIE	int null,
	primary key(idProduto, idEntrada)
)