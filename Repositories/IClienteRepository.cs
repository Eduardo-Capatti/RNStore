namespace RNStore.Repositories;

using RNStore.Models;

public interface IClienteRepository
{

    public User Create(Cliente cliente);
    public User LoginUser(Login model);

    public void ColocarCarrinho(int idCliente, int idProduto);
}
