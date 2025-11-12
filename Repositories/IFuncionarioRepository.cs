namespace RNStore.Repositories;

using RNStore.Models;

public interface IFuncionarioRepository
{
    public List<Funcionario> Read();
    public Funcionario Read(int idFuncionario);
    public void Create(Funcionario funcionario);
    public void Update(Funcionario funcionario);
    public void Delete(int idFuncionario);

    public string Verificar(Funcionario funcionario);
}