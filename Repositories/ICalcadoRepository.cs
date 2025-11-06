namespace RNStore.Repositories;

using RNStore.Models;

public interface ICalcadoRepository
{
    public List<Calcado> Read();
    public Calcado Read(int idCalcado);
    public void Create(Calcado calcado);

    public Calcado Create();
    public void Update(Calcado calcado);
    public void Delete(int idCalcado);
}