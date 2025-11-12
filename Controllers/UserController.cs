using Microsoft.AspNetCore.Mvc;
using RNStore.Models;
using RNStore.Repositories;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
namespace RNStore.Controllers;


public class UserController : Controller
{

    private IUserRepository repository;

    public UserController(IUserRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Login()
    {
        return View(new Login());
    }

    [HttpPost]
    public ActionResult Login(Login model)
    {
        User user = repository.LoginUser(model);

        if (user == null)
        {
            ViewBag.Error = "Usuário e/ou senha inválidos!";
            return View(model);
        }

        HttpContext.Session.SetInt32("idUsuario", user.idUsuario);
        HttpContext.Session.SetString("nomeUsuario", user.nomeUsuario);

        return RedirectToAction("Index", "Estoque");
    }

    public ActionResult Sair()
    {
        HttpContext.Session.Clear();

        return RedirectToAction("Login", "User");
    }


}
