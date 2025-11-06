namespace RNStore.Repositories;

using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;


public class UserDatabaseRepository : Connection, IUserRepository
{
    public UserDatabaseRepository(string conn) : base(conn)
    {

    }

    public User LoginUser(Login model)
    {
        string senhaHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(model.senha)));

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT p.nomePessoa, p.idPessoa FROM Funcionarios f LEFT JOIN Pessoas p on p.idPessoa = f.idPessoa WHERE email = @email AND senha = @senha";
        cmd.Parameters.AddWithValue("@email", model.email);
        cmd.Parameters.AddWithValue("@senha", senhaHash);

        SqlDataReader reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new User
            {
                idUsuario = (int)reader["idPessoa"],
                nomeUsuario = (string)reader["nomePessoa"]
            };
        }

        return null;
    }
}