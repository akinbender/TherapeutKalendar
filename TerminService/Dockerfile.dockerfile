# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["TerminService/TerminService.csproj", "TerminService/"]
COPY ["TherapeutKalendar.Shared.Protos/TherapeutKalendar.Shared.Protos.csproj", "TherapeutKalendar.Shared.Protos/"]

# Restore dependencies
RUN dotnet restore "TerminService/TerminService.csproj"

# Copy everything else
COPY . .

# Build
RUN dotnet build "TerminService/TerminService.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "TerminService/TerminService.csproj" -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl

COPY --from=publish /app/publish .
EXPOSE 8080

# Health check (adjust to your gRPC health endpoint)
HEALTHCHECK --interval=30s --timeout=3s \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TerminService.dll"]