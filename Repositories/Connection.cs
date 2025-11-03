namespace RNStore.Repositories;

using Microsoft.Data.SqlClient;

public abstract class Connection : IDisposable
{
    protected SqlConnection conn;

    public Connection(string conn)
    {
        this.conn = new SqlConnection(conn);
        this.conn.Open();
    }

    public void Dispose()
    {
        conn.Close();
    }
}