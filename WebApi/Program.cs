using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business;
using Business.ServiceRegistrations;
using Core.Utilities.IOC;
using Core.Extensions;
using Core.Helpers;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.Services.AddServices(builder.Configuration);

// autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacModule()));
builder.Services.CreateServices([new CoreModule()]);

// set static data 
GeneralStaticHelper.DataProtectionKey = GeneralStaticHelper.CreateGuid(3);
GeneralStaticHelper.DataProtectionSalt = Encoding.ASCII.GetBytes(GeneralStaticHelper.DataProtectionKey);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// data protector
var data_provider = app.Services.GetRequiredService<IDataProtectionProvider>();
GeneralStaticHelper.DataProtector = data_provider.CreateProtector(GeneralStaticHelper.DataProtectionKey);

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 404 || context.HttpContext.Response.StatusCode == 405)
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            Success = false,
            Message = $"No such route was found for the {context.HttpContext.Request.Method} request."
        }));
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
