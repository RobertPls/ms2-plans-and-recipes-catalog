FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src


COPY Shared/Shared.csproj Shared/
COPY Domain/Catalog.Domain.csproj Domain/
COPY Application/Catalog.Application.csproj Application/
COPY Infrastructure/Catalog.Infrastructure.csproj Infrastructure/
COPY WebApi/WebApi.csproj WebApi/
RUN dotnet restore WebApi/WebApi.csproj

COPY . .
RUN dotnet publish WebApi/WebApi.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app


RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .


ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApi.dll"]