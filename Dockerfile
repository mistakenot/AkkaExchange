FROM microsoft/dotnet:latest
COPY ./AkkaExchange.Web /app
COPY ./AkkaExchange /AkkaExchange
WORKDIR /app
 
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
 

ENV ASPNETCORE_URLS http://*:$PORT
 
CMD dotnet run
