FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Elara.sln", "."]
COPY ["src/Elara.API/Elara.API.csproj", "src/Elara.API/"]
COPY ["src/Elara.Application/Elara.Application.csproj", "src/Elara.Application/"]
COPY ["src/Elara.Domain/Elara.Domain.csproj", "src/Elara.Domain/"]
COPY ["src/Elara.Infrastructure/Elara.Infrastructure.csproj", "src/Elara.Infrastructure/"]
RUN dotnet restore "Elara.sln"

COPY . .
WORKDIR "/src/src/Elara.API"
RUN dotnet build "Elara.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Elara.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Elara.API.dll"]