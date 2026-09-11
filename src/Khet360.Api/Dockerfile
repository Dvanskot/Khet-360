# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for restore
COPY ["src/Khet360.Api/Khet360.Api.csproj", "src/Khet360.Api/"]
COPY ["src/Khet360.Application/Khet360.Application.csproj", "src/Khet360.Application/"]
COPY ["src/Khet360.Infrastructure/Khet360.Infrastructure.csproj", "src/Khet360.Infrastructure/"]
COPY ["src/Khet360.Domain/Khet360.Domain.csproj", "src/Khet360.Domain/"]

# Restore dependencies
RUN dotnet restore "src/Khet360.Api/Khet360.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Khet360.Api"
RUN dotnet build "Khet360.Api.csproj" -c Release -o /app/build --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish "Khet360.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Khet360.Api.dll"]