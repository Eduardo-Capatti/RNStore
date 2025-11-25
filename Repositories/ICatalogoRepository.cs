namespace RNStore.Repositories;

using RNStore.Models;

public interface ICatalogoRepository
{
    List<Catalogo> Read();

    List<Slider> ReadSlides();
    Catalogo Read(int idProduto, int idCalcado);

    List<Catalogo> Buscar(string buscarProduto);

    List<Compra> Read(int? idCliente);

    void AdicionarQtdCarrinho(int idCompra, int idProduto, decimal valorIC);

    void RemoverQtdCarrinho(int idCompra, int idProduto, decimal valorIC);

    void Delete (Compra compra);

    void ConfirmarCompra(int idCompra);

    void Carrinho(Catalogo catalogo, int? idCliente);

    bool IsOnCart(int idProduto, int? idCliente);

}