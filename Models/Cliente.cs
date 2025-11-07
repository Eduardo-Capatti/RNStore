namespace RNStore.Models;

using System.ComponentModel.DataAnnotations; // Precisa disso!

public class Cliente
{
    public int IdPessoa { get; set; }


    public string NomePessoa { get; set; }


    public string Cpf { get; set; }

    public string Email { get; set; }


    public string Senha { get; set; }
    
    public string Telefone { get; set; } 

    public int  Status { get; set; }
    
    public virtual ICollection<Endereco> Enderecos { get; set; }

    public Cliente()
    {
        Enderecos = new HashSet<Endereco>();
    }
}