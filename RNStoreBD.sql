create database RNStore2
go
use RNStoreTeste

create table Pessoas(
	idPessoa     int       not null   primary key  identity,
	nomePessoa   varchar(50)  not null,
	cpf          varchar(14)  not null unique,
	email        varchar(40) not null unique,
	senha		 varchar(255) not null,
	telefone	 varchar(15) null 	
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
	nomeForn      varchar(100)  not null
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

CREATE TABLE Cores (
    idCor INT PRIMARY KEY IDENTITY,
    nomeCor VARCHAR(50) NOT NULL
);


CREATE TABLE CoresCalcados (
    corId INT not null references Cores(idCor),
    calcadoId INT NOT NULL REFERENCES Calcados(idCalcado)
);

CREATE TABLE Produtos (
    idProduto INT PRIMARY KEY IDENTITY,
    calcadoId INT NOT NULL REFERENCES Calcados(idCalcado),
    corId INT NOT NULL REFERENCES Cores(idCor),
    tamanhoId INT NOT NULL REFERENCES Tamanhos(idTamanho),
    preco MONEY NOT NULL,
    promocao MONEY NULL,
    qtd INT NOT NULL DEFAULT 0
);


CREATE TABLE Imagens (
    idImagem INT PRIMARY KEY IDENTITY,
    corId INT NOT NULL REFERENCES Cores(idCor),
    nomeImagem VARCHAR(100) NOT NULL,
    statusImagem INT NOT NULL CHECK(statusImagem IN (0,1)) -- 1 = principal
);

Create Table ImagensProdutos (
	corId int not null references Cores(idCor),
	calcadoId int not null references Calcados(idCalcado),
	imagemId int not null references Imagens(idImagem)
)

create table Compras(
	idCompra     int			not null     primary key identity,
	dataCompra	 date			not null	 default GETDATE(),	  
	totalCompra	 decimal(10,2)      null,
	statusCompra int				null	 check(statusCompra in(0,1,2,3)), -- 0 = carrinho, 1 = pago, 2= enviado, 3 = entregue,
	data_entrega date				null,
	idCliente int not null references Clientes(idPessoa)
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