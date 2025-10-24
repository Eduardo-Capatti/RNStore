namespace RNStore.Models;

public class Estoque
{
    public int idProduto { get; }
    public int calcadoId { get; set; }

    public int tamanho { get; set; }

    public string cor { get; set; }

    public float promocao { get; set; }

    public int qtd { get; set; }

    public string img { get; set; }
}
