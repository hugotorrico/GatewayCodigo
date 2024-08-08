using GatewayCodigo;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile("ocelot.json");
//builder.Services.AddOcelot();
builder.Services.AddOcelot().
    AddTransientDefinedAggregator<SimpleAggregator>()
    .AddTransientDefinedAggregator<DemoAggregator>()
    .AddTransientDefinedAggregator<TestAggregator>()
    .AddTransientDefinedAggregator<FinalAggregator>()
    ;


var app = builder.Build();
app.UseOcelot();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();
