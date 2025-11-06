namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class MarcaDatabaseRepository : Connection, IMarcaRepository
{
    public MarcaDatabaseRepository(string conn) : base(conn)
    {

    }

    public void Create(Marca marca)
    {

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;

        cmd.CommandText = "INSERT INTO Marca(nomeMarca) VALUES (@marca)";

        cmd.Parameters.AddWithValue("@marca", marca.nomeMarca);



        cmd.ExecuteNonQuery();

    }

    public void Delete(int idMarca)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Marca WHERE idMarca = @idMarca";
        cmd.Parameters.AddWithValue("idMarca", idMarca);

        cmd.ExecuteNonQuery();
    }

    public List<Marca> Read()
    {
        List<Marca> marcas = new List<Marca>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Marca";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            marcas.Add(
                new Marca
                {
                    idMarca = (int)reader["idMarca"],
                    nomeMarca = (string)reader["nomeMarca"]
                }
            );
        }
        return marcas;
    }
}
