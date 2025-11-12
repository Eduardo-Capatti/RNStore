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
            ViewBag.Func = funcionario;
            return View("Create", Create());
        }

        string resultado = repository.Verificar(funcionario);

        switch (resultado)
        {
            case "cpf":

                ViewBag.Error = "Esse CPF já está sendo utilizado!";
                ViewBag.Func = funcionario;
                return View("Create", Create());

            case "telefone":

                ViewBag.Error = "Esse telefone já está sendo utilizado!";
                ViewBag.Func = funcionario;
                return View("Create", Create());

            case "email":

                ViewBag.Error = "Esse email já está sendo utilizado!";
                ViewBag.Func = funcionario;
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
                .Where(p => p.CanRead && (p.Name == "email" || p.Name == "telefone" || p.Name == "salario")) 
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
