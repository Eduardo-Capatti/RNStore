namespace RNStore.Models;

public class Catalogo
{
    public List<Catalogo> catalogo {get;set;}
    public int idProduto { get; set; }
    public int calcadoId { get; set; }
    public string nomeCalcado { get; set; }

    public string marcaCalcado { get; set; }

    public List<Tamanho>tamanho { get; set; }

    public List<Cor> cor { get; set; }

    public string corPrincipal { get; set; }

    public decimal preco { get; set; }

    public decimal? promocao { get; set; }

    public int qtd { get; set; }

    public List<Imagem> img { get; set; }

    public List<Imagem> imgOutrasCores { get; set; }

    public List<string> ApenasImagemPrincipal { get; set; }

    public List<Slider> slides { get; set; }

}
