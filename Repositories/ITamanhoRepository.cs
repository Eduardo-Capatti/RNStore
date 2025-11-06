namespace RNStore.Repositories;

using RNStore.Models;

public interface ITamanhoRepository
{
    public List<Tamanho> Read();

    public void Create(Tamanho tamanho);
    public void Delete(int idTamanho);
}