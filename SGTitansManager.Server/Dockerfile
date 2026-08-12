FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SGTitansManagerBackend/SGTitansManagerBackend.csproj", "SGTitansManagerBackend/"]
COPY ["SGTitansManager.Models/SGTitansManager.Models.csproj", "SGTitansManager.Models/"]
RUN dotnet restore "SGTitansManagerBackend/SGTitansManagerBackend.csproj"
COPY . .
WORKDIR "/src/SGTitansManagerBackend"
RUN dotnet build "./SGTitansManagerBackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SGTitansManagerBackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SGTitansManagerBackend.dll"]
