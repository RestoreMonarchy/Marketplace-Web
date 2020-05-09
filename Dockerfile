FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80
EXPOSE 443

RUN  apt-get update \
 && apt-get install -y wget \
 &&     apt-get install -y unzip \
 && rm -rf /var/lib/apt/lists/*


RUN curl -s https://api.github.com/repos/RestoreMonarchy/Marketplace-Web/releases \
 | grep "browser_download_url" \
 | cut -d : -f 2,3 \
 | tr -d \" \
 | wget -qi -

RUN unzip Marketplace-Web.zip

COPY ./app /app
WORKDIR /app/

CMD ["dotnet", "Marketplace.Server.dll"]