using CurrencyConverterAPI.Data;
using CurrencyConverterAPI.Repositories;
using CurrencyConverterAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Configure Database
builder.Services.AddDbContext<CurrencyDbContext>(options =>
options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Redis connection
builder.Services.AddStackExchangeRedisCache(options =>
  {
      options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
      options.InstanceName = "CurrencyExchange_";
  });
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();

