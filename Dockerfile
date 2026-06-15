FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY CampusHub.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
RUN mkdir -p /app/wwwroot/uploads

# Installer netcat pour tester la connexion
RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Script d'attente pour SQL Server
ENTRYPOINT ["/bin/sh", "-c", "echo 'Attente de SQL Server...'; while ! nc -z campushub-db 1433; do sleep 2; done; echo 'SQL Server pret!'; dotnet CampusHub.dll"]