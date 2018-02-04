FROM microsoft/dotnet:latest
COPY ./AkkaExchange.Web /app
COPY ./AkkaExchange /AkkaExchange
WORKDIR /app
 
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
  
CMD dotnet run --server.urls=http://0.0.0.0:$PORT