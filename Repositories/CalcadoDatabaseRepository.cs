namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class CalcadoDatabaseRepository : Connection, ICalcadoRepository
{
    public CalcadoDatabaseRepository(string conn) : base(conn)
    {

    }

    public void Create(Calcado calcado)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "INSERT INTO Calcados VALUES(@marcaId, @nomeCalcado)";
        cmd.Parameters.AddWithValue("@marcaId", calcado.marcaId);
        cmd.Parameters.AddWithValue("@nomeCalcado", calcado.nomeCalcado);
        cmd.Parameters.AddWithValue("@idCalcado", calcado.idCalcado);

        cmd.ExecuteNonQuery();
    }
    
    public Calcado Create()
    {
        List<Marca> marcas = new List<Marca>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Marca";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            marcas.Add
            (
                new Marca
                {
                    idMarca = (int)reader["idMarca"],
                    nomeMarca = (string)reader["nomeMarca"]
                }
            );
        }

        return new Calcado
        {
            selectMarcas = marcas
        };
    }

    public void Delete(int idCalcado)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Calcados WHERE idCalcado = @idCalcado";
        cmd.Parameters.AddWithValue("idCalcado", idCalcado);

        cmd.ExecuteNonQuery();
    }

    public List<Calcado> Read()
    {
        List<Calcado> Calcados = new List<Calcado>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Calcados c LEFT JOIN Marca m on c.marcaId = m.idMarca";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Calcados.Add(
                new Calcado
                {
                    idCalcado = (int)reader["idCalcado"],
                    nomeCalcado = (string)reader["nomeCalcado"],
                    nomeMarca = (string)reader["nomeMarca"]
                }
            );
        }
        return Calcados;
    }

    public Calcado Read(int idCalcado)
    {

        List<Marca> marcas = new List<Marca>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Marca";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            marcas.Add
            (
                new Marca
                {
                    idMarca = (int)reader["idMarca"],
                    nomeMarca = (string)reader["nomeMarca"]
                }
            );
        }

        reader.Close();

        SqlCommand cmd2 = new SqlCommand();
        cmd2.Connection = conn;
        cmd2.CommandText = "SELECT * FROM Calcados WHERE idCalcado = @idCalcado";
        cmd2.Parameters.AddWithValue("@idCalcado", idCalcado);
        SqlDataReader reader2 = cmd2.ExecuteReader();

        if (reader2.Read())
        {
            return new Calcado
            {
                idCalcado = (int)reader2["idCalcado"],
                nomeCalcado = (string)reader2["nomeCalcado"],
                selectMarcas = marcas,
                marcaId = (int)reader2["marcaId"]

            };
            
        }
        return null;
    }


    public void Update(Calcado calcado)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
        UPDATE Calcados SET nomeCalcado = @nomecalcado, marcaId = @idMarca
        WHERE idCalcado = @idCalcado";
        cmd.Parameters.AddWithValue("@nomeCalcado", calcado.nomeCalcado);
        cmd.Parameters.AddWithValue("@idMarca", calcado.marcaId);
        cmd.Parameters.AddWithValue("@idCalcado", calcado.idCalcado);
    
        cmd.ExecuteNonQuery();

    }
}
