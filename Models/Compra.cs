namespace RNStore.Models;

public class Compra
{
    public List<Compra> compras {get;set;}

    public int idCompra {get; set; }
    public int idProduto { get; set; }
    
    public string nomeCalcado { get; set; }

    public string nomeCor {get;set;}

    public string marcaCalcado { get; set; }

    public string tamanho {get;set;}

    public decimal totalCompra { get; set; }

    public decimal preco {get;set;}

    public decimal? promocao { get; set; }

    public int qtdIC { get; set; }

    public decimal valorIC {get; set;}

    public string nomeImagem { get; set; }

    public int qtdDisponivel {get;set;}

    public string operacao {get; set;}

    public int statusCompra {get;set;}

    public string nomePessoa{get;set;}

    public string email{get;set;}

    public string dataEntrega {get;set;}

    public string dataCompra{get;set;}

    public string rua{get;set;}
    public string bairro{get;set;}
    public string numero{get;set;}

    public string cidade{get;set;}
}
