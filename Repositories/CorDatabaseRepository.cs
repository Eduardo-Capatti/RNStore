namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class CorDatabaseRepository : Connection, ICorRepository
{
    public CorDatabaseRepository(string conn) : base(conn)
    {

    }

    public void Create(Cor cor)
    {

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;

        cmd.CommandText = "INSERT INTO Cores(nomeCor) VALUES (@cor)";

        cmd.Parameters.AddWithValue("@cor", cor.nomeCor);



        cmd.ExecuteNonQuery();

    }

    public void Delete(int idCor)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Cores WHERE idCor = @idCor";
        cmd.Parameters.AddWithValue("idCor", idCor);

        cmd.ExecuteNonQuery();
    }

    public List<Cor> Read()
    {
        List<Cor> cores = new List<Cor>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Cores";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            cores.Add(
                new Cor
                {
                    idCor = (int)reader["idcor"],
                    nomeCor = (string)reader["nomeCor"]
                }
            );
        }
        return cores;
    }
}
