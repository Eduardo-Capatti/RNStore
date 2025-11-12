using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class FornecedorController: Controller
{
    private IFornecedorRepository repository;

    public FornecedorController (IFornecedorRepository repository)
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
    public ActionResult Create(Fornecedor fornecedor)
    {
        bool TemCampoNulo(Fornecedor Fornecedor)
        {
            if (fornecedor == null) return true;

            return fornecedor.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.Name != "idFornecedor") // ignora idPessoa
                .Any(p => p.GetValue(fornecedor) == null);
        }

        if (TemCampoNulo(fornecedor))
        {
            ViewBag.Error = "Preencha todas as informações!";
            ViewBag.Fornecedor = fornecedor;
            return View("Create");
        }

        string resultado = repository.Verificar(fornecedor);

        switch (resultado)
        {
            case "cnpj":

                ViewBag.Error = "Esse CNPJ já está sendo utilizado!";
                ViewBag.Fornecedor = fornecedor;
                return View("Create", Create());
        }

        repository.Create(fornecedor);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int idFornecedor)
    {
        var fornecedor = repository.Read(idFornecedor);
        return View(fornecedor);
    }

    [HttpPost]
    public ActionResult Update(Fornecedor fornecedor)
    {

        bool TemCampoNulo(Fornecedor Fornecedor)
        {
            if (fornecedor == null) return true;

            return fornecedor.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.Name != "idFornecedor") // ignora idPessoa
                .Any(p => p.GetValue(fornecedor) == null);
        }

        if (TemCampoNulo(fornecedor))
        {
            ViewBag.Error = "Não deixe informações em branco!";
            var fornecedor_escolhido = repository.Read(fornecedor.idFornecedor);
            return View("Update", fornecedor_escolhido);
        }

        repository.Update(fornecedor);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idFornecedor)
    {
        try
        {
            repository.Delete(idFornecedor);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir o funcionário, pois ele está vinculado a um produto!";

            return View("Index", repository.Read());
        }
       
    }


}
