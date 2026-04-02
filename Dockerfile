FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InovaBank.Api/InovaBank.Api.csproj", "InovaBank.Api/"]
COPY ["InovaBank.Worker/InovaBank.Worker.csproj", "InovaBank.Worker/"]
COPY ["InovaBank.Application/InovaBank.Application.csproj", "InovaBank.Application/"]
COPY ["InovaBank.Domain/InovaBank.Domain.csproj", "InovaBank.Domain/"]
COPY ["InovaBank.Infrastructure/InovaBank.Infrastructure.csproj", "InovaBank.Infrastructure/"]

RUN dotnet restore "InovaBank.Api/InovaBank.Api.csproj"

COPY . .

ARG PROJECT_PATH=InovaBank.Api
RUN dotnet publish "${PROJECT_PATH}/${PROJECT_PATH}.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

USER root
RUN mkdir -p /app/wwwroot/documents && chmod 777 /app/wwwroot/documents
USER app

ENTRYPOINT ["sh", "-c", "dotnet ${PROJECT_DLL}"]
