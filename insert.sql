use RNStoreOficial


Insert into Pessoas (nomePessoa, cpf, email, senha, telefone) values ('admin', '111.111.111-11', 'admin', '8C6976E5B5410415BDE908BD4DEE15DFB167A9C873FC4BB8A81F6F2AB448A918', '(17) 99111-1111'); 
Insert into Pessoas (nomePessoa, cpf, email, senha, telefone) values ('Ana Vendedora', '222.222.222-22', 'ana@rnstore.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4', '(17) 99222-2222'); 
Insert into Pessoas (nomePessoa, cpf, email, senha, telefone) values ('Roberto Estoque', '333.333.333-33', 'roberto@rnstore.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4', '(17) 99333-3333'); 
Insert into Pessoas (nomePessoa, cpf, email, senha, telefone) values ('Lucas Cliente', '444.444.444-44', 'lucas@gmail.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4', '(11) 99444-4444'); 
Insert into Pessoas (nomePessoa, cpf, email, senha, telefone) values ('Mariana Cliente', '555.555.555-55', 'mariana@hotmail.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4', '(21) 99555-5555'); 
Insert into Pessoas (nomePessoa, cpf, email, senha, telefone) values ('Julia Cliente', '666.666.666-66', 'julia@yahoo.com', '03AC674216F3E15C761EE1A5E255F067953623C8B388B4459E13F978D7C846F4', '(31) 99666-6666'); 


Insert into Funcionarios (idPessoa, salario) values (1, 5000.00); 
Insert into Funcionarios (idPessoa, salario) values (2, 2500.00); 
Insert into Funcionarios (idPessoa, salario) values (3, 3000.00); 
Insert into Clientes (idPessoa, status) values (4, 1); 
Insert into Clientes (idPessoa, status) values (5, 1); 
Insert into Clientes (idPessoa, status) values (6, 2);


Insert into Enderecos (cep, rua, numero, bairro, cidade, uf, idCliente) values ('15000000', 'Rua das Flores', '100', 'Centro', 'São José do Rio Preto', 'SP', 4); 
Insert into Enderecos (cep, rua, numero, bairro, cidade, uf, idCliente) values ('01001000', 'Av Paulista', '500', 'Bela Vista', 'São Paulo', 'SP', 5); 
Insert into Enderecos (cep, rua, numero, bairro, cidade, uf, idCliente) values ('20000000', 'Rua Copacabana', '20', 'Zona Sul', 'Rio de Janeiro', 'RJ', 6); 


Insert into Fornecedor (cnpj, nomeForn) values ('12.345.678/0001-90', 'Nike do Brasil LTDA'); 
Insert into Fornecedor (cnpj, nomeForn) values ('98.765.432/0001-10', 'Adidas Industrial'); 
Insert into Fornecedor (cnpj, nomeForn) values ('11.222.333/0001-44', 'Distribuidora Puma'); 
Insert into Fornecedor (cnpj, nomeForn) values ('11.989.212/0001-55', 'Distribuidora Fila'); 
Insert into Fornecedor (cnpj, nomeForn) values ('22.222.222/0001-22', 'Distribuidora Everlast'); 


Insert into Marca (nomeMarca) values ('Nike'); 
Insert into Marca (nomeMarca) values ('Adidas'); 
Insert into Marca (nomeMarca) values ('Puma'); 
Insert into Marca (nomeMarca) values ('Everlast'); 

Insert into Cores (nomeCor) values ('Preto'); 
Insert into Cores (nomeCor) values ('Branco'); 
Insert into Cores (nomeCor) values ('Azul Marinho'); 
Insert into Cores (nomeCor) values ('Vermelho'); 

 
Insert into Tamanhos (tamanho) values ('38'); 
Insert into Tamanhos (tamanho) values ('39'); 
Insert into Tamanhos (tamanho) values ('40'); 
Insert into Tamanhos (tamanho) values ('41'); 
Insert into Tamanhos (tamanho) values ('42'); 
Insert into Tamanhos (tamanho) values ('43'); 


Insert into Calcados (marcaId, nomeCalcado) values (1, 'Air Force'); 
Insert into Calcados (marcaId, nomeCalcado) values (1, 'Air Max 270'); 
Insert into Calcados (marcaId, nomeCalcado) values (1, 'Revolution 6'); 
Insert into Calcados (marcaId, nomeCalcado) values (2, 'Grand Court 6'); 
Insert into Calcados (marcaId, nomeCalcado) values (3, 'Puma Smash V2');
Insert into Calcados (marcaId, nomeCalcado) values (4, 'Climber Pro');

-- ==========================================================
-- 1. RELACIONAMENTO CORES X CALÇADOS (Sem Fila)
-- ==========================================================
-- IDs Calçados: 1=AirForce, 2=AirMax, 3=Revolution, 4=GrandCourt, 5=SmashV2, 6=Climber

INSERT INTO CoresCalcados (corId, calcadoId) VALUES
(3, 1), (1, 1), -- Air Force: Azul e Preto
(1, 2),         -- Air Max 270: Preto
(1, 3),         -- Revolution 6: Preto
(1, 4), (4, 4), -- Grand Court: Preto e Vermelho
(3, 5), (1, 5), -- Smash V2: Azul e Preto
(3, 6), (1, 6); -- Climber Pro: Azul e Preto


-- ==========================================================
-- 2. PREPARAÇÃO DAS ENTRADAS (Para vincular Fornecedor)
-- ==========================================================
-- Criamos as "Notas Fiscais" de entrada para podermos vincular os produtos assim que nascerem.
-- IDs Fornecedores: 1=Nike, 2=Adidas, 3=Puma, 5=Everlast

INSERT INTO Entradas (dataEntrada, idFornecedor) VALUES 
(GETDATE(), 1), -- ID 1: Entrada Nike
(GETDATE(), 2), -- ID 2: Entrada Adidas
(GETDATE(), 3), -- ID 3: Entrada Puma
(GETDATE(), 5); -- ID 4: Entrada Everlast (Atenção: o ID gerado na tabela será 4, mas referencia o forn 5)

-- OBS: Ao rodar o insert acima, assumo que os IDs das Entradas gerados foram:
-- 1 (Nike), 2 (Adidas), 3 (Puma), 4 (Everlast)


-- ==========================================================
-- 3. CRIANDO PRODUTOS E VINCULANDO AO ESTOQUE (ITENS_ENTRADAS)
-- ==========================================================

-- --- NIKE (Air Force, Air Max, Revolution) ---
INSERT INTO Produtos (calcadoId, corId, tamanhoId, preco, qtd) VALUES
(1, 3, 3, 799.90, 10), (1, 3, 4, 799.90, 8), -- Air Force Azul (40, 41)
(1, 1, 3, 799.90, 12), (1, 1, 4, 799.90, 10), -- Air Force Preto (40, 41)
(2, 1, 3, 999.90, 5),  (2, 1, 4, 999.90, 6),  -- Air Max Preto (40, 41)
(3, 1, 2, 349.90, 20), (3, 1, 3, 349.90, 15); -- Revolution Preto (39, 40)

-- Registrando entrada da Nike (Entrada ID 1)
INSERT INTO Itens_Entradas (idProduto, idEntrada, qtdIE, valorIE)
SELECT idProduto, 1, qtd, preco * 0.6 FROM Produtos WHERE calcadoId IN (1, 2, 3);


-- --- ADIDAS (Grand Court) ---
INSERT INTO Produtos (calcadoId, corId, tamanhoId, preco, qtd) VALUES
(4, 1, 4, 399.90, 10), -- Preto 41
(4, 4, 3, 399.90, 10), (4, 4, 4, 399.90, 8); -- Vermelho 40, 41

-- Registrando entrada da Adidas (Entrada ID 2)
INSERT INTO Itens_Entradas (idProduto, idEntrada, qtdIE, valorIE)
SELECT idProduto, 2, qtd, preco * 0.6 FROM Produtos WHERE calcadoId = 4;


-- --- PUMA (Smash V2) ---
INSERT INTO Produtos (calcadoId, corId, tamanhoId, preco, qtd) VALUES
(5, 3, 3, 279.90, 15), -- Azul 40
(5, 1, 3, 279.90, 15), (5, 1, 4, 279.90, 12); -- Preto 40, 41

-- Registrando entrada da Puma (Entrada ID 3)
INSERT INTO Itens_Entradas (idProduto, idEntrada, qtdIE, valorIE)
SELECT idProduto, 3, qtd, preco * 0.6 FROM Produtos WHERE calcadoId = 5;


-- --- EVERLAST (Climber Pro) ---
INSERT INTO Produtos (calcadoId, corId, tamanhoId, preco, qtd) VALUES
(6, 3, 4, 199.90, 25), -- Azul 41
(6, 1, 4, 199.90, 25), (6, 1, 5, 199.90, 10); -- Preto 41, 42

-- Registrando entrada da Everlast (Entrada ID 4)
INSERT INTO Itens_Entradas (idProduto, idEntrada, qtdIE, valorIE)
SELECT idProduto, 4, qtd, preco * 0.6 FROM Produtos WHERE calcadoId = 6;


-- ==========================================================
-- 4. INSERINDO IMAGENS
-- ==========================================================
-- (Sem a imagem do racer carbon)

INSERT INTO Imagens (corId, nomeImagem, statusImagem) VALUES 
-- Air Force
(3, 'airforceazul1.png', 1), (3, 'airforceazul2.png', 0), (3, 'airforceazul3.png', 0), (3, 'airforceazul4.png', 0), (3, 'airforceazul5.png', 0),
(1, 'airforcepreto1.png', 1), (1, 'airforcepreto2.png', 0), (1, 'airforcepreto3.png', 0), (1, 'airforcepreto4.png', 0), (1, 'airforcepreto5.png', 0),
-- Air Max
(1, 'airmax270preto1.png', 1), (1, 'airmax270preto2.png', 0), (1, 'airmax270preto3.png', 0),
-- Climber
(3, 'climberazul.png', 1), (3, 'climberazul2.png', 0), (3, 'climberazul3.png', 0),
(1, 'climberpreto.png', 1),
-- Grand Court
(1, 'grandcourtpreto1.png', 1), (1, 'grandcourtpreto2.png', 0),
(4, 'grandcourtvermelho1.png', 1), (4, 'grandcourtvermelho2.png', 0), (4, 'grandcourtvermelho3.png', 0), (4, 'grandcourtvermelho4.png', 0),
-- Revolution
(1, 'nikerevolutionpreto1.png', 1), (1, 'nikerevolutionpreto2.png', 0), (1, 'nikerevolutionpreto3.png', 0), (1, 'nikerevolutionpreto4.png', 0),
-- Smash V2
(3, 'smashv2azul.png', 1), (3, 'smashv2azul2.png', 0), (3, 'smashv2azul3.png', 0), (3, 'smashv2azul4.png', 0),
(1, 'smashv2preto1.png', 1), (1, 'smashv2preto2.png', 0), (1, 'smashv2preto3.png', 0), (1, 'smashv2preto4.png', 0), (1, 'smashv2preto5.png', 0);


-- ==========================================================
-- 5. VINCULANDO IMAGENS AOS PRODUTOS
-- ==========================================================
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 3, 1, idImagem FROM Imagens WHERE nomeImagem LIKE 'airforceazul%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 1, 1, idImagem FROM Imagens WHERE nomeImagem LIKE 'airforcepreto%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 1, 2, idImagem FROM Imagens WHERE nomeImagem LIKE 'airmax270preto%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 1, 3, idImagem FROM Imagens WHERE nomeImagem LIKE 'nikerevolutionpreto%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 1, 4, idImagem FROM Imagens WHERE nomeImagem LIKE 'grandcourtpreto%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 4, 4, idImagem FROM Imagens WHERE nomeImagem LIKE 'grandcourtvermelho%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 3, 5, idImagem FROM Imagens WHERE nomeImagem LIKE 'smashv2azul%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 1, 5, idImagem FROM Imagens WHERE nomeImagem LIKE 'smashv2preto%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 3, 6, idImagem FROM Imagens WHERE nomeImagem LIKE 'climberazul%';
INSERT INTO ImagensProdutos (corId, calcadoId, imagemId) SELECT 1, 6, idImagem FROM Imagens WHERE nomeImagem LIKE 'climberpreto%';


-- ==========================================================
-- 6. POPULANDO COMPRAS (VENDAS)
-- ==========================================================
-- Regra: Status != 0 -> data_entrega NOT NULL

-- COMPRA 1: Status 1 (Pago) -> Definimos data_entrega futura (Previsão)
INSERT INTO Compras (dataCompra, totalCompra, statusCompra, data_entrega, idCliente) VALUES
(GETDATE(), 1079.80, 1, DATEADD(day, 7, GETDATE()), 4); -- Lucas

-- Itens da Compra 1
INSERT INTO Itens_Compra (idCompra, idProduto, qtdIC, valorIC) 
SELECT TOP 1 1, idProduto, 1, preco FROM Produtos WHERE calcadoId = 1 AND corId = 3; -- Air Force Azul
INSERT INTO Itens_Compra (idCompra, idProduto, qtdIC, valorIC) 
SELECT TOP 1 1, idProduto, 1, preco FROM Produtos WHERE calcadoId = 5 AND corId = 3; -- Smash V2 Azul


-- COMPRA 2: Status 3 (Entregue) -> Data entrega passada
INSERT INTO Compras (dataCompra, totalCompra, statusCompra, data_entrega, idCliente) VALUES
(DATEADD(day, -10, GETDATE()), 399.90, 3, DATEADD(day, -5, GETDATE()), 5); -- Mariana

-- Itens da Compra 2
INSERT INTO Itens_Compra (idCompra, idProduto, qtdIC, valorIC) 
SELECT TOP 1 2, idProduto, 1, preco FROM Produtos WHERE calcadoId = 4 AND corId = 4; -- Grand Court Vermelho


-- COMPRA 3: Status 0 (Carrinho) -> Data entrega PODE ser NULL
INSERT INTO Compras (dataCompra, totalCompra, statusCompra, data_entrega, idCliente) VALUES
(GETDATE(), 199.90, 0, NULL, 6); -- Julia (Cliente Novo/Inativo na tabela pessoas mas inserindo compra de teste)

-- Itens da Compra 3
INSERT INTO Itens_Compra (idCompra, idProduto, qtdIC, valorIC) 
SELECT TOP 1 3, idProduto, 1, preco FROM Produtos WHERE calcadoId = 6 AND corId = 1; -- Climber Preto

Insert into Slider(idFuncionario, img, status) values(1, 'slider1.png', 1)


