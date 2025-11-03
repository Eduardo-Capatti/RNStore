namespace RNStore.Repositories;

using RNStore.Models;

public interface ICatalogoRepository
{
    List<Catalogo> Read();
    Catalogo Read(int id);

}