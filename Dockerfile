FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InovaBank.Api/InovaBank.Api.csproj", "InovaBank.Api/"]
COPY ["InovaBank.Domain/InovaBank.Domain.csproj", "InovaBank.Domain/"]
COPY ["InovaBank.Application/InovaBank.Application.csproj", "InovaBank.Application/"]
COPY ["InovaBank.Infrastructure/InovaBank.Infrastructure.csproj", "InovaBank.Infrastructure/"]
COPY ["InovaBank.Worker/InovaBank.Worker.csproj", "InovaBank.Worker/"]

RUN dotnet restore "InovaBank.Api/InovaBank.Api.csproj"

COPY . .
WORKDIR "/src/InovaBank.Api"
RUN dotnet build "InovaBank.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InovaBank.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/wwwroot/documents && chmod 777 /app/wwwroot/documents
USER $APP_USER

EXPOSE 8080
ENTRYPOINT ["dotnet", "InovaBank.Api.dll"]
