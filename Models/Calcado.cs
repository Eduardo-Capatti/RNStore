namespace RNStore.Models;


public class Calcado
{
    public int idCalcado { get; set; }
    public string nomeMarca { get; set; }
    public int marcaId { get; set; }
    public string nomeCalcado { get; set; }
    public List<Marca> selectMarcas { get; set; }



}

