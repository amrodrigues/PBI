using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Garante que os Enums (TipoDocumento, Situacao) apareçam como texto no Swagger e na API
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Configuração dos serviços do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Solicitação de Documentos v1");
        c.RoutePrefix = string.Empty; // Faz a interface do Swagger abrir direto na raiz (http://localhost:<porta>/)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
app.Run();