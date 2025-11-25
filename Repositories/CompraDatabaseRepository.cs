namespace RNStore.Repositories;

using System.Collections.Generic;
using System.Data.Common;
using RNStore.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

public class CompraDatabaseRepository : Connection, ICompraRepository
{
    public CompraDatabaseRepository(string conn) : base(conn)
    {

    }


    public List<Compra> Read()
    {
        List<Compra> compras = new List<Compra>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "SELECT * FROM v_BuscarVendas ORDER BY nomePessoa ";
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            compras.Add(
                new Compra
                {
                    idCompra = (int)reader["idCompra"],
                    nomePessoa = (string)reader["nomePessoa"],
                    statusCompra = (int)reader["statusCompra"],
                    email = (string)reader["email"],
                    dataCompra = Convert.ToDateTime(reader["dataCompra"]).ToString("dd/MM/yyyy"),
                    dataEntrega = Convert.ToDateTime(reader["data_entrega"]).ToString("dd/MM/yyyy"),
                    cidade = (string)reader["cidade"],
                    bairro = (string)reader["bairro"],
                    numero = (string)reader["numero"],
                    rua = (string)reader["rua"],
                    totalCompra = (decimal)reader["totalCompra"]
                }
            );
        }
        reader.Close();
        return compras;
    }

    public List<Compra> Read(int idCompra)
    {
        List<Compra> compras = new List<Compra>();

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"SELECT * FROM dbo.fn_BuscarItensCompra(@idCompra)";

        cmd.Parameters.AddWithValue("@idCompra", idCompra);
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            compras.Add(
                new Compra
                {
                    idCompra = (int)reader["idCompra"],
                    nomeCalcado = (string)reader["nomeCalcado"],
                    nomeCor = (string)reader["nomeCor"],
                    marcaCalcado = (string)reader["nomeMarca"],
                    tamanho = (string)reader["tamanho"],
                    valorIC = (decimal)reader["valorIC"],
                    qtdIC = (int)reader["qtdIC"]
                }
            );
        }
        reader.Close();
        return compras;
    }

    public void Update(Compra compra)
    {
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = @"UPDATE Compras SET statusCompra = @statusCompra WHERE idCompra = @idCompra ";

        cmd.Parameters.AddWithValue("@idCompra", compra.idCompra);
        cmd.Parameters.AddWithValue("@statusCompra", compra.statusCompra);
        cmd.ExecuteNonQuery();
    }
}
