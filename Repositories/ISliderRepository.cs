namespace RNStore.Repositories;

using RNStore.Models;

public interface ISliderRepository
{
    public List<Slider> Read();
    public Slider Read(int idSlider);
    public void Create(Slider slider);
    public void Update(Slider slider);
    public void Delete(int idSlider);
}