using MongoDbBookstoreApi.Models;
using MongoDbBookstoreApi.Services;
using Scalar.AspNetCore;

//Ref: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-10.0&tabs=visual-studio
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));
builder.Services.AddSingleton<BooksService>();
builder.Services.AddScoped<IMediaRepository, MongoMediaRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                // EXTREMELY IMPORTANT: Expose this header so JS can read the filename
                .WithExposedHeaders("Content-Disposition");
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseStaticFiles();

// 2. MIDDLEWARE ORDER IS CRITICAL
app.UseRouting(); // First: Resolve the endpoint

app.UseCors("AllowAll"); // Second: Apply CORS before Auth/Endpoint

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // Or app.UseSwaggerUI()
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
