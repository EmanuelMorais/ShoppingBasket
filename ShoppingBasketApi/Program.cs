using Microsoft.EntityFrameworkCore;
using ShoppingBasketApi.Application;
using ShoppingBasketApi.Data.Database;
using ShoppingBasketApi.Domain.Abstractions;
using ShoppingBasketApi.Domain.Services;
using ShoppingBasketApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:7288")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseInMemoryDatabase("InMemoryDb"));

var basePath = AppContext.BaseDirectory; // Get the base directory of the application
var absolutePath = Path.Combine(basePath, "4-Infrastructure", "rules.json");

builder.Services.AddSingleton<IRulesFileProvider>(new RulesFileProvider(absolutePath));
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IRulesEngine, DiscountRulesEngine>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Basket API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
