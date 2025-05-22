using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebApiPizushi.Data;
using WebApiPizushi.Filters;
using WebApiPizushi.Interfaces;
using WebApiPizushi.Models.Category;
using WebApiPizushi.Services;
using WebApiPizushi.Validators.Category;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// реєстрація AutoMapper

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddControllers();

//Виключаємо стандартну валідацію через ModelState

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

//Додаємо валідацію через FluentValidation
//Шукаємо всі можливі валідатори, які наслідуються від AbstractValidator
// Фільтр може змінювати дані до і після контролера
builder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

//Додаємо фільтр
builder.Services.AddMvc(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddCors();// щоби сервери могли взаємодіяти між собою

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

var dir = builder.Configuration["ImagesDir"];
string path = Path.Combine(Directory.GetCurrentDirectory(), dir);
Directory.CreateDirectory(path);

app.UseStaticFiles(new StaticFileOptions // надання доступу до папки з фото
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dir}" // куди звертатися
});

await app.SeedData();

app.Run();
