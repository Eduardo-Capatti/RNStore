namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

public class SliderDatabaseRepository : Connection, ISliderRepository
{
    public SliderDatabaseRepository(string conn) : base(conn)
    {

    }

    public void Create(Slider slider)
    {

        SqlCommand cmd = new SqlCommand();

        cmd.Connection = conn;

        cmd.CommandText = @"
        INSERT INTO Slider(img, status, idFuncionario) VALUES (@img, @status, @idFuncionario)";

        cmd.Parameters.AddWithValue("@img", slider.img);
        cmd.Parameters.AddWithValue("@status", slider.status);
        cmd.Parameters.AddWithValue("@idFuncionario", slider.idFuncionario);

        cmd.ExecuteNonQuery();
    }

    public void Delete(int idSlider)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM Slider WHERE idSlider = @idSlider";
        cmd.Parameters.AddWithValue("idSlider", idSlider);

        cmd.ExecuteNonQuery();
    }

    public List<Slider> Read()
    {
        List<Slider> sliders = new List<Slider>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM Slider LEFT JOIN Pessoas on idFuncionario = idPessoa";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            sliders.Add(
                new Slider
                {
                    idSlider = (int)reader["idSlider"],
                    nomePessoa = (string)reader["nomePessoa"],
                    img = (string)reader["img"],
                    status = (int)reader["status"],
                }
            );
        }
        return sliders;
    }

    public Slider Read(int idSlider)
    {

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT status FROM Slider WHERE idSlider = @idSlider";
        cmd.Parameters.AddWithValue("@idSlider", idSlider);
        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Slider
            {
                idSlider = idSlider,
                status = (int)reader["status"]
            };
            
        }
        return null;
    }


    public void Update(Slider slider)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"
        UPDATE Slider SET status = @status
        WHERE idSlider = @idSlider";
        cmd.Parameters.AddWithValue("@status", slider.status);
        cmd.Parameters.AddWithValue("@idSlider", slider.idSlider);
        
        cmd.ExecuteNonQuery();

    }
}
