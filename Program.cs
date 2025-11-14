using RNStore.Repositories;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICatalogoRepository>(_ =>
    new CatalogoDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<ITamanhoRepository>(_ => 
    new TamanhoDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<ICorRepository>(_ =>
    new CorDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IFuncionarioRepository>(_ =>
    new FuncionarioDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IMarcaRepository>(_ =>
    new MarcaDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));
        
builder.Services.AddTransient<ICalcadoRepository>(_ => 
    new CalcadoDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IFornecedorRepository>(_ =>
    new FornecedorDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IClienteRepository>(_ =>
    new ClienteDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<IUserRepository>(_ => 
    new UserDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<ISliderRepository>(_ =>
    new SliderDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));
        
builder.Services.AddTransient<IEstoqueRepository>(_ => 
    new EstoqueDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));




var cultura = new CultureInfo("pt-BR");


builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new[] { cultura },
    SupportedUICultures = new[] { cultura }
});

app.UseStaticFiles();


app.UseSession();

//rota default
app.MapControllerRoute("default","{controller=Catalogo}/{action=Index}/{idProduto?}/{idCalcado?}");


app.Run();
