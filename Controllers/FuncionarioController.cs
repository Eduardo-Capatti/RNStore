using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class FuncionarioController: Controller
{
    private IFuncionarioRepository repository;

    public FuncionarioController (IFuncionarioRepository repository)
    {
        this.repository = repository;
    }
    public ActionResult Index()
    {
        return View(repository.Read());
    }
    
    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(Funcionario funcionario)
    {
        bool TemCampoNulo(Funcionario funcionario)
        {
            if (funcionario == null) return true;

            return funcionario.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.Name != "idPessoa") // ignora idPessoa
                .Any(p => p.GetValue(funcionario) == null);
        }

        if (TemCampoNulo(funcionario))
        {
            ViewBag.Error = "Preencha todas as informações!";
            return View("Create", Create());
        }

        repository.Create(funcionario);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int idFuncionario)
    {
        var funcionario = repository.Read(idFuncionario);
        return View(funcionario);
    }

    [HttpPost]
    public ActionResult Update(Funcionario funcionario)
    {
        bool TemCampoNulo(Funcionario funcionario)
        {
            if (funcionario == null) return true;

            return funcionario.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.Name != "idPessoa") // ignora idPessoa
                .Any(p => p.GetValue(funcionario) == null);
        }

        if (TemCampoNulo(funcionario))
        {
            ViewBag.Error = "Preencha todas as informações!";
            var funcionario_escolhido = repository.Read(funcionario.idPessoa);
            return View("Update", funcionario_escolhido);
        }

        repository.Update(funcionario);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idFuncionario)
    {
        try
        {
            repository.Delete(idFuncionario);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir o funcionário, contate o suporte para mais detalhes!";

            return View("Index", repository.Read());
        }
       
    }


}
