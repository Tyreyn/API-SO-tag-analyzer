#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API-SO-tag-analyzer/API-SO-tag-analyzer.csproj", "."]
RUN dotnet restore "./API-SO-tag-analyzer.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet test "./API-SO-tag-analyzer.test/API-SO-tag-analyzer.test.csproj" --logger:"trx;LogFilePrefix=testResults"
RUN dotnet build "./API-SO-tag-analyzer/API-SO-tag-analyzer.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API-SO-tag-analyzer/API-SO-tag-analyzer.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM publish AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API-SO-tag-analyzer.dll"]