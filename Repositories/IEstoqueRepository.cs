namespace RNStore.Repositories;

using RNStore.Models;

public interface IEstoqueRepository
{
    public List<Estoque> Read();
    public Estoque Read(int idEstoque);
    public void Create(Estoque estoque);
    public Estoque Create();
    public void CreateImg(Estoque estoque);
    public void Update(Estoque estoque);
    public void UpdateImg(Estoque estoque);
    public void Delete(int idEstoque);

    public void DeleteImg(int idImagem);
}