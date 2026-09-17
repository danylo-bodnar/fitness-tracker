FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY Directory.Build.props global.json ./

COPY FitnessTracker.Api/*.csproj FitnessTracker.Api/
COPY FitnessTracker.Application/*.csproj FitnessTracker.Application/
COPY FitnessTracker.Contracts/*.csproj FitnessTracker.Contracts/
COPY FitnessTracker.Domain/*.csproj FitnessTracker.Domain/
COPY FitnessTracker.Infrastructure/*.csproj FitnessTracker.Infrastructure/
COPY FitnessTracker.ServiceDefaults/*.csproj FitnessTracker.ServiceDefaults/

RUN dotnet restore FitnessTracker.Api/FitnessTracker.Api.csproj

COPY FitnessTracker.Api/ FitnessTracker.Api/
COPY FitnessTracker.Application/ FitnessTracker.Application/
COPY FitnessTracker.Contracts/ FitnessTracker.Contracts/
COPY FitnessTracker.Domain/ FitnessTracker.Domain/
COPY FitnessTracker.Infrastructure/ FitnessTracker.Infrastructure/
COPY FitnessTracker.ServiceDefaults/ FitnessTracker.ServiceDefaults/

RUN dotnet publish FitnessTracker.Api/FitnessTracker.Api.csproj \
    -c Release -o /app/out --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .

USER app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "FitnessTracker.Api.dll"]
