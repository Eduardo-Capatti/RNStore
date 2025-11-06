namespace RNStore.Repositories;

using RNStore.Models;

public interface IMarcaRepository
{
    public List<Marca> Read();

    public void Create(Marca marca);
    public void Delete(int idMarca);
}