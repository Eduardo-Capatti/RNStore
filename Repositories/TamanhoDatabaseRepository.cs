namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class TamanhoDatabaseRepository : Connection, ITamanhoRepository
{
    public TamanhoDatabaseRepository(string conn) : base(conn)
    {

    }

    public void Create(Tamanho tamanho)
    {

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;

        cmd.CommandText = "INSERT INTO Tamanhos(tamanho) VALUES (@Novotamanho)";

        cmd.Parameters.AddWithValue("@Novotamanho", tamanho.tamanho);



        cmd.ExecuteNonQuery();

    }

    public void Delete(int idTamanho)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Tamanhos WHERE idTamanho = @idTamanho";
        cmd.Parameters.AddWithValue("idTamanho", idTamanho);

        cmd.ExecuteNonQuery();
    }

    public List<Tamanho> Read()
    {
        List<Tamanho> tamanhos = new List<Tamanho>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Tamanhos";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            tamanhos.Add(
                new Tamanho
                {
                    idTamanho = (int)reader["idTamanho"],
                    tamanho = (string)reader["tamanho"]
                }
            );
        }
        return tamanhos;
    }

    public void Update(int idTamanho)
    {
        
    }
}
