FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["src/backend/BuildingBlocks/", "src/backend/BuildingBlocks/"]
COPY ["src/backend/Services/ProxyServerAggregator/", "src/backend/Services/ProxyServerAggregator/"]
RUN dotnet restore "src/backend/Services/ProxyServerAggregator/ProxyServerAggregator/ProxyServerAggregator.csproj"
COPY . .
WORKDIR "/src/src/backend/Services/ProxyServerAggregator/ProxyServerAggregator"
RUN dotnet build "./ProxyServerAggregator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProxyServerAggregator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProxyServerAggregator.dll"]
