using Bloom.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseSwaggerDocumentation();

app.UseCors("AllowAngularClient");

app.MapControllers();

app.Run();