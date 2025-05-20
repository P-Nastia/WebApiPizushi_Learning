using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebApiPizushi.Data;
using WebApiPizushi.Interfaces;
using WebApiPizushi.Models.Category;
using WebApiPizushi.Services;
using WebApiPizushi.Validators.Category;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());// ��������� AutoMapper

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddControllers();

builder.Services.AddScoped<IValidator<CategoryCreateModel>, CategoryCreateValidator>();
builder.Services.AddCors();// ���� ������� ����� ��������� �� �����

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

app.UseStaticFiles(new StaticFileOptions // ������� ������� �� ����� � ����
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = $"/{dir}" // ���� ����������
});

await app.SeedData();

app.Run();
