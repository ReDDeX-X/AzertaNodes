using AzertaNodes.Application.Implementations;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Interfaces;
using AzertaNodes.Infrastructure.Nodes;
using AzertaNodes.Infrastructure.Persistence;
using AzertaNodes.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();

//builder.Services.AddHttpClient("Default").ConfigurePrimaryHttpMessageHandler(() => new Status201To200Handler { InnerHandler = new HttpClientHandler() });
builder.Services.AddAutoMapper(cfg => { }, typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHostedService<DiscordBootstrapService>();


builder.Services.AddSingleton<ICredentialEncryptor, AesCredentialEncryptor>();
builder.Services.AddSingleton<IKernelFactory, KernelFactory>();
builder.Services.AddSingleton<IContextService, ContextService>();
builder.Services.AddSingleton<IRetryPolicyFactory, RetryPolicyFactory>();
builder.Services.AddSingleton<IDiscordConnectionManager, DiscordConnectionManager>();


builder.Services.AddScoped<IGraphRunner, DirectedAcyclicGraph>();
builder.Services.AddScoped<IGraphService, GraphService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICredentialService, CredentialService>();
builder.Services.AddScoped<ICredentialRepository, CredentialRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INodeFactory, NodeFactory>();
builder.Services.AddScoped<IGraphValidationService, GraphValidationService>();


builder.Services.AddTransient<ILlmService, LlmService>();

//Nodes
builder.Services.AddTransient<LlmNode>();
builder.Services.AddTransient<HttpRequestNode>();
builder.Services.AddTransient<HttpResponseNode>();
builder.Services.AddTransient<TestConsoleWriteLineNode>();
builder.Services.AddTransient<LogicNode>();

builder.Services.AddHttpClient();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
