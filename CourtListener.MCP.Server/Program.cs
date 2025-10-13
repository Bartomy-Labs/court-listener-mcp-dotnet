using CourtListener.MCP.Server.Configuration;
using CourtListener.MCP.Server.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/server.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7);
});

// Add CourtListener HTTP client
builder.Services.AddCourtListenerClient(builder.Configuration);

// Add MCP server with all tools from assembly
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly(typeof(Program).Assembly);

// Configure Kestrel to listen on 0.0.0.0:8000
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8000); // HTTP on 0.0.0.0:8000
});

var app = builder.Build();

// Add global exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Map health check endpoint for Docker
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Server = "CourtListener MCP Server",
    Timestamp = DateTime.UtcNow
}));

// Map MCP endpoint
app.MapMcp("/mcp/");

// Log startup
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("CourtListener MCP Server starting on http://0.0.0.0:8000/mcp/");

app.Run();
