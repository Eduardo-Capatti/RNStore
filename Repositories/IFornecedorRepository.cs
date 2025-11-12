namespace RNStore.Repositories;

using RNStore.Models;

public interface IFornecedorRepository
{
    public List<Fornecedor> Read();
    public Fornecedor Read(int idFornecedor);
    public void Create(Fornecedor fornecedor);
    public void Update(Fornecedor fornecedor);
    public void Delete(int idFornecedor);

    public string Verificar(Fornecedor fornecedor);
}