namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class FornecedorDatabaseRepository : Connection, IFornecedorRepository
{
    public FornecedorDatabaseRepository(string conn) : base(conn)
    {

    }

    public string Verificar(Fornecedor fornecedor)
    {
        SqlCommand cmd = new SqlCommand();
        
        cmd.Connection = conn;

        cmd.CommandText = @"
            SELECT 
                CASE 
                    WHEN EXISTS (SELECT 1 FROM Fornecedor WHERE cnpj = @cnpj) THEN 'cnpj'
                    ELSE 'ok'
                END AS Resultado
        ";;

        cmd.Parameters.AddWithValue("@cnpj", fornecedor.cnpj);

        string resultado = cmd.ExecuteScalar()?.ToString();

        return resultado;

    }

    public void Create(Fornecedor fornecedor)
    {

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;

        cmd.CommandText = @"INSERT INTO Fornecedor(cnpj, nomeForn) VALUES (@cnpj, @nomeForn);";

        cmd.Parameters.AddWithValue("@cnpj", fornecedor.cnpj);
        cmd.Parameters.AddWithValue("@nomeForn", fornecedor.nomeFornecedor);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int idFornecedor)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Fornecedor WHERE idFornecedor = @idFornecedor";
        cmd.Parameters.AddWithValue("idFornecedor", idFornecedor);

        cmd.ExecuteNonQuery();
    }

    public List<Fornecedor> Read()
    {
        List<Fornecedor> fornecedor = new List<Fornecedor>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Fornecedor";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            fornecedor.Add(
                new Fornecedor
                {
                    idFornecedor = (int)reader["idFornecedor"],
                    nomeFornecedor = (string)reader["nomeForn"],
                    cnpj = (string)reader["cnpj"],
                }
            );
        }
        return fornecedor;
    }

    public Fornecedor Read(int idFornecedor)
    {

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Fornecedor WHERE idFornecedor = @idFornecedor";
        cmd.Parameters.AddWithValue("@idFornecedor", idFornecedor);
        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Fornecedor
            {
                idFornecedor = (int)reader["idFornecedor"],
                nomeFornecedor = (string)reader["nomeForn"],
                cnpj = (string)reader["cnpj"],
            };
            
        }
        return null;
    }


    public void Update(Fornecedor fornecedor)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
        UPDATE Fornecedor SET nomeForn = @nomeFornecedor, cnpj = @cnpj 
        WHERE idFornecedor = @idFornecedor";
        cmd.Parameters.AddWithValue("@nomeFornecedor", fornecedor.nomeFornecedor);
        cmd.Parameters.AddWithValue("@cnpj", fornecedor.cnpj);
        cmd.Parameters.AddWithValue("@idFornecedor", fornecedor.idFornecedor);

        cmd.ExecuteNonQuery();

    }
}
