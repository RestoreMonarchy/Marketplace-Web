FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
EXPOSE 80
EXPOSE 443

# Install required packages
RUN apt-get update && \ 
    apt-get install curl

COPY ./app /app
WORKDIR /app/web

CMD ["dotnet", "Marketplace.Server.dll"] 