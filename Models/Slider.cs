namespace RNStore.Models;


public class Slider
{
    public int idSlider { get; set; }
    public int idFuncionario { get; set; }
    public string nomePessoa { get; set; }
    public string img { get; set; }

    public IFormFile downloadImg { get; set; }
    public int status { get; set; }

}

