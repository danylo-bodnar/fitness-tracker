# ---------- Stage 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY FitnessTracker.Api/FitnessTracker.Api.csproj FitnessTracker.Api/
RUN dotnet restore FitnessTracker.Api/FitnessTracker.Api.csproj

COPY . .
RUN dotnet publish FitnessTracker.Api/FitnessTracker.Api.csproj -c Release -o out --no-restore

# ---------- Stage 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "FitnessTracker.Api.dll"]
