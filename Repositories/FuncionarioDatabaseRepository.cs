namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

public class FuncionarioDatabaseRepository : Connection, IFuncionarioRepository
{
    public FuncionarioDatabaseRepository(string conn) : base(conn)
    {

    }

    public void Create(Funcionario funcionario)
    {
        int idPessoa = 0;

        string senhaHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(funcionario.senha)));

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;

        cmd.CommandText = @"
        INSERT INTO Pessoas(nomePessoa, cpf, email, senha, telefone) VALUES (@nomePessoa, @cpf, @email, @senha, @telefone);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

        cmd.Parameters.AddWithValue("@nomePessoa", funcionario.nomePessoa);
        cmd.Parameters.AddWithValue("@cpf", funcionario.cpf);
        cmd.Parameters.AddWithValue("@email", funcionario.email);
        cmd.Parameters.AddWithValue("@senha", senhaHash);
        cmd.Parameters.AddWithValue("@telefone", funcionario.telefone);

        idPessoa = (int)cmd.ExecuteScalar();

        SqlCommand cmd2 = new SqlCommand();

        cmd2.Connection = conn;
        cmd2.CommandText = "INSERT INTO Funcionarios VALUES(@idPessoa, @salario)";
        cmd2.Parameters.AddWithValue("@idPessoa", idPessoa);
        cmd2.Parameters.AddWithValue("@salario", funcionario.salario);

        cmd2.ExecuteNonQuery();
    }

    public void Delete(int idFuncionario)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Funcionarios WHERE idPessoa = @idFuncionario";
        cmd.Parameters.AddWithValue("idFuncionario", idFuncionario);

        cmd.ExecuteNonQuery();
    }

    public List<Funcionario> Read()
    {
        List<Funcionario> funcionarios = new List<Funcionario>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Funcionarios f LEFT JOIN Pessoas p on p.idPessoa = f.idPessoa";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            funcionarios.Add(
                new Funcionario
                {
                    idPessoa = (int)reader["idPessoa"],
                    nomePessoa = (string)reader["nomePessoa"],
                    cpf = (string)reader["cpf"],
                    email = (string)reader["email"],
                    telefone = (string)reader["telefone"],
                    salario = (decimal)reader["salario"]
                }
            );
        }
        return funcionarios;
    }

    public Funcionario Read(int idFuncionario)
    {

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Funcionarios f LEFT JOIN Pessoas p on p.idPessoa = f.idPessoa WHERE p.idPessoa = @idFuncionario";
        cmd.Parameters.AddWithValue("@idFuncionario", idFuncionario);
        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Funcionario
            {
                idPessoa = (int)reader["idPessoa"],
                nomePessoa = (string)reader["nomePessoa"],
                cpf = (string)reader["cpf"],
                senha = (string)reader["senha"],
                email = (string)reader["email"],
                telefone = (string)reader["telefone"],
                salario = (decimal)reader["salario"]
            };
            
        }
        return null;
    }


    public void Update(Funcionario funcionario)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
        UPDATE Pessoas SET nomePessoa = @nomePessoa, cpf = @cpf, email = @email, senha = @senha, telefone = @telefone 
        WHERE idPessoa = @idPessoa";
        cmd.Parameters.AddWithValue("@nomePessoa", funcionario.nomePessoa);
        cmd.Parameters.AddWithValue("@cpf", funcionario.cpf);
        cmd.Parameters.AddWithValue("@senha", funcionario.senha);
        cmd.Parameters.AddWithValue("@email", funcionario.email);
        cmd.Parameters.AddWithValue("@telefone", funcionario.telefone);
        cmd.Parameters.AddWithValue("@idPessoa", funcionario.idPessoa);

        cmd.ExecuteNonQuery();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = @"
        UPDATE Funcionarios SET salario = @salario WHERE idPessoa = @idPessoa";
        cmd2.Parameters.AddWithValue("@salario", funcionario.salario);
        cmd2.Parameters.AddWithValue("@idPessoa", funcionario.idPessoa);

        cmd2.ExecuteNonQuery();

    }
}
