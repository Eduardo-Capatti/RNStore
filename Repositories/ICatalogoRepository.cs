namespace RNStore.Repositories;

using RNStore.Models;

public interface ICatalogoRepository
{
    List<Catalogo> Read();

    List<Slider> ReadSlides();
    Catalogo Read(int idProduto, int idCalcado);

    List<Catalogo> Buscar(string buscarProduto);

}