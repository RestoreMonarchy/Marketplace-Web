FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY Marketplace.sln ./
COPY ApiKeyAuthentication/*.csproj ./ApiKeyAuthentication/
COPY Client/*.csproj ./Client/
COPY Database/*.csproj ./Database/
COPY DatabaseProvider/*.csproj ./DatabaseProvider/
COPY Server/*.csproj ./Server/
COPY Shared/*.csproj ./Shared/

RUN dotnet restore
COPY . .
WORKDIR /src/ApiKeyAuthentication
RUN dotnet build -c Release -o /app

WORKDIR /src/Client
RUN dotnet build -c Release -o /app

WORKDIR /src/Database
RUN dotnet build -c Release -o /app

WORKDIR /src/DatabaseProvider
RUN dotnet build -c Release -o /app

WORKDIR /src/Server
RUN dotnet build -c Release -o /app

WORKDIR /src/Shared
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Marketplace.Server.dll"]