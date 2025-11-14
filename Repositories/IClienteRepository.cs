namespace RNStore.Repositories;

using RNStore.Models;

public interface IClienteRepository
{

    void Create(Cliente cliente);
    public User LoginUser(Login model);
}
