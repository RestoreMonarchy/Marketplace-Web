#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Server/Marketplace.Server.csproj", "Server/"]
COPY ["ApiKeyAuthentication/Marketplace.ApiKeyAuthentication.csproj", "ApiKeyAuthentication/"]
COPY ["Shared/Marketplace.Shared.csproj", "Shared/"]
COPY ["DatabaseProvider/Marketplace.DatabaseProvider.csproj", "DatabaseProvider/"]
COPY ["Client/Marketplace.Client.csproj", "Client/"]
RUN dotnet restore "Server/Marketplace.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Marketplace.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Marketplace.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.Server.dll"]