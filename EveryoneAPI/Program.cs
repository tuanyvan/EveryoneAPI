using Microsoft.OpenApi.Models;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using EveryoneAPI.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Azure.Security.KeyVault.Secrets;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Media;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("EveryoneKeyVault"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("api", new OpenApiInfo { Title = "Everyone API", Version = "v1.0" });
});

builder.Services.AddScoped<EveryoneDBContext>();

builder.Services.AddMvc(option => option.EnableEndpointRouting = false);

var test = builder.Configuration["Authentication:Google:ClientId"];
var test2 = builder.Configuration["Authentication:Google:ClientSecret"];

builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/api/swagger.json", "Everyone API");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
