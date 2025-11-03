namespace RNStore.Models;

public class Catalogo
{
    public int idProduto { get; set; }
    public string nomeCalcado { get; set; }

    public string marcaCalcado { get; set; }

    public List<string>tamanho { get; set; }

    public List<string> cor { get; set; }

    public float promocao { get; set; }

    public int qtd { get; set; }

    public List<string> img { get; set; }

}
