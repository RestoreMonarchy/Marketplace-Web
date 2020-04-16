FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Solution.sln ./
COPY ClassLibraryProject/*.csproj ./ClassLibraryProject/
COPY WebAPIProject/*.csproj ./WebAPIProject/

RUN dotnet restore
COPY . .
WORKDIR /src/ClassLibraryProject
RUN dotnet build -c Release -o /app

WORKDIR /src/WebAPIProject
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebAPIProject.dll"]




# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
# WORKDIR /src

# RUN dotnet build "src/Server/Marketplace.Server.csproj" -c Release -o /app/build

# FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
# WORKDIR /src
# RUN dir
# COPY ["Server/Marketplace.Server.csproj", "Server/"]
# COPY ["ApiKeyAuthentication/Marketplace.ApiKeyAuthentication.csproj", "ApiKeyAuthentication/"]
# COPY ["Shared/Marketplace.Shared.csproj", "Shared/"]
# COPY ["DatabaseProvider/Marketplace.DatabaseProvider.csproj", "DatabaseProvider/"]
# COPY ["Client/Marketplace.Client.csproj", "Client/"]
# RUN dotnet restore "Server/Marketplace.Server.csproj"
# COPY . .
# WORKDIR "/src/Server"
# RUN dotnet build "Marketplace.Server.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "Marketplace.Server.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# EXPOSE 80
# EXPOSE 443
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "Marketplace.Server.dll"]