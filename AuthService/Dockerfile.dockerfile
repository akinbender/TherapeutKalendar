# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["AuthService/AuthService.csproj", "AuthService/"]
COPY ["TherapeutKalendar.Shared.Protos/TherapeutKalendar.Shared.Protos.csproj", "TherapeutKalendar.Shared.Protos/"]

# Restore dependencies
RUN dotnet restore "AuthService/AuthService.csproj"

COPY . .

# Build
RUN dotnet build "AuthService/AuthService.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "AuthService/AuthService.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install required tools
RUN apt-get update && apt-get install -y curl

COPY --from=publish /app/publish .
EXPOSE 8081

# Health check
HEALTHCHECK --interval=30s --timeout=3s \
  CMD curl -f http://localhost:8081/health || exit 1

ENTRYPOINT ["dotnet", "AuthService.dll"]