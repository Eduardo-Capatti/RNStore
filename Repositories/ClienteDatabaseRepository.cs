namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models; // (Onde seu model Cliente, Endereco, etc. estão)
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

// Assumindo que sua interface chame IClienteRepository
public class ClienteDatabaseRepository : Connection, IClienteRepository
{
    // Construtor (herdado da sua classe base Connection)
    public ClienteDatabaseRepository(string conn) : base(conn)
    {
    }
    public User Create(Cliente cliente)
    {

        string senhaHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(cliente.Senha)));

        using (conn)
        {
            SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                int idPessoa = 0;


                using (SqlCommand cmdPessoa = new SqlCommand())
                {
                    cmdPessoa.Connection = conn;
                    cmdPessoa.Transaction = transaction; // Associa à transação

                    cmdPessoa.CommandText = @"
                        INSERT INTO Pessoas(nomePessoa, cpf, email, senha, telefone) 
                        VALUES (@nomePessoa, @cpf, @email, @senha, @telefone);
                        SELECT CAST(SCOPE_IDENTITY() AS int);"; // Pega o ID que acabou de ser criado

                    cmdPessoa.Parameters.AddWithValue("@nomePessoa", cliente.NomePessoa);
                    cmdPessoa.Parameters.AddWithValue("@cpf", cliente.Cpf);
                    cmdPessoa.Parameters.AddWithValue("@email", cliente.Email);
                    cmdPessoa.Parameters.AddWithValue("@senha", senhaHash);


                    cmdPessoa.Parameters.AddWithValue("@telefone", (object)cliente.Telefone ?? DBNull.Value);


                    idPessoa = (int)cmdPessoa.ExecuteScalar();
                }


                using (SqlCommand cmdCliente = new SqlCommand())
                {
                    cmdCliente.Connection = conn;
                    cmdCliente.Transaction = transaction; // Associa à transação

                    cmdCliente.CommandText = "INSERT INTO Clientes(idPessoa, status) VALUES(@idPessoa, @status)";

                    cmdCliente.Parameters.AddWithValue("@idPessoa", idPessoa);
                    cmdCliente.Parameters.AddWithValue("@status", 1); // Converte o Enum para int

                    cmdCliente.ExecuteNonQuery();
                }


                var endereco = cliente.Enderecos.FirstOrDefault();

                if (endereco != null)
                {
                    using (SqlCommand cmdEndereco = new SqlCommand())
                    {
                        cmdEndereco.Connection = conn;
                        cmdEndereco.Transaction = transaction; // Associa à transação

                        cmdEndereco.CommandText = @"
                            INSERT INTO Enderecos(cep, rua, complemento, numero, bairro, cidade, uf, idCliente) 
                            VALUES (@cep, @rua, @complemento, @numero, @bairro, @cidade, @uf, @idCliente)";

                        cmdEndereco.Parameters.AddWithValue("@cep", endereco.Cep);
                        cmdEndereco.Parameters.AddWithValue("@rua", endereco.Rua);
                        cmdEndereco.Parameters.AddWithValue("@numero", endereco.Numero);
                        cmdEndereco.Parameters.AddWithValue("@bairro", endereco.Bairro);
                        cmdEndereco.Parameters.AddWithValue("@cidade", endereco.Cidade);
                        cmdEndereco.Parameters.AddWithValue("@uf", endereco.Uf);

                        // O 'idCliente' do endereço é o 'idPessoa'
                        cmdEndereco.Parameters.AddWithValue("@idCliente", idPessoa);

                        // Tratamento para campo opcional (complemento)
                        cmdEndereco.Parameters.AddWithValue("@complemento", (object)endereco.Complemento ?? DBNull.Value);

                        cmdEndereco.ExecuteNonQuery();
                    }
                }


                transaction.Commit();
                Login model = new Login {
                    email = cliente.Email,
                    senha = cliente.Senha
                };

                var retorno = LoginUser(model);

                return retorno;

            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;

            }


        }


    }
    public User LoginUser(Login model)
    {
        string senhaHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(model.senha)));

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT p.nomePessoa, p.idPessoa FROM Clientes c LEFT JOIN Pessoas p on p.idPessoa = c.idPessoa WHERE email = @email AND senha = @senha";
        cmd.Parameters.AddWithValue("@email", model.email);
        cmd.Parameters.AddWithValue("@senha", senhaHash);

        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            var user = new User
            {
                idUsuario = (int)reader["idPessoa"],
                nomeUsuario = (string)reader["nomePessoa"]
            };

            reader.Close();
            return user;
        }

        reader.Close();
        return null;
    }

    public void ColocarCarrinho(int idCliente, int idProduto)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;

        cmd.CommandText = @"exec sp_ColocarCarrinhoAposLogin @idCliente, @idProduto";

        cmd.Parameters.AddWithValue("@idCliente", idCliente);
        cmd.Parameters.AddWithValue("@idProduto", idProduto);

        cmd.ExecuteNonQuery();
    }
}
