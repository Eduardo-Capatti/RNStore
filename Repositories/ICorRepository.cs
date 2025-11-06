namespace RNStore.Repositories;

using RNStore.Models;

public interface ICorRepository
{
    public List<Cor> Read();

    public void Create(Cor cor);
    public void Delete(int idCor);
}