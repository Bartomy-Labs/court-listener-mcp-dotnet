# Multi-stage Dockerfile for CourtListener MCP Server
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["CourtListener.MCP.Server/CourtListener.MCP.Server.csproj", "CourtListener.MCP.Server/"]
RUN dotnet restore "CourtListener.MCP.Server/CourtListener.MCP.Server.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/CourtListener.MCP.Server"
RUN dotnet build "CourtListener.MCP.Server.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "CourtListener.MCP.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Expose MCP server port
EXPOSE 8000

# Copy published application from build stage
COPY --from=build /app/publish .

# Configure health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl --fail http://localhost:8000/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "CourtListener.MCP.Server.dll"]
