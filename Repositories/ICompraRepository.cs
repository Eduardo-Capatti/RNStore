namespace RNStore.Repositories;

using RNStore.Models;

public interface ICompraRepository
{
    public List<Compra> Read();

    public List<Compra> Read(int idCompra);

    public void Update(Compra compra);
}