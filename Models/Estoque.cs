namespace RNStore.Models;

public class Estoque
{
    public int idProduto { get; set; }

    public decimal? promocao { get; set; }

    public decimal preco { get; set; }

    public int qtd { get; set; }

    public string img { get; set; }

    public int idImagem { get; set; }
    
    public int calcadoId { get; set; }

    public int tamanhoId { get; set; }

    public decimal valorIE{ get; set; }

    public int corId { get; set; }
    public List<Tamanho> tamanhos { get; set; }
    
    public List<Cor> cores { get; set; }

    public List<Calcado> calcados { get; set; }

    public List<Marca> marcas { get; set; }
    
     public string nomeCalcado { get; set; }

    public string tamanho { get; set; }

    public string nomeCor { get; set; }

    public string nomeMarca { get; set; }

    public List<Calcado> selectCalcado { get; set; }
    
    public List<Tamanho> selectTamanho { get; set; }

    public List<Cor> selectCor { get; set; }

    public List<Fornecedor> selectFornecedor { get; set; }

    public List<Imagem> imagens { get; set; }

    public IFormFile downloadImg { get; set; }

    public int idFornecedor { get; set; }

    public string nomeFornecedor { get; set; }
}
