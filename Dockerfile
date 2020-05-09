FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80
EXPOSE 443

RUN  apt-get update \
 && apt-get install -y wget \
 && rm -rf /var/lib/apt/lists/*

RUN curl -s https://api.github.com/repos/restoremonarchy/marketplace-web/releases \
 && grep "browser_download_url.*deb" \
 && cut -d : -f 2,3 \
 && tr -d \" \
 && wget -qi -

RUN unzip Marketplace-Web.zip

COPY ./app /app
WORKDIR /app/

CMD ["dotnet", "Marketplace.Server.dll"]