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
COPY ["src/backend/Services/ProxyServerTester/", "src/backend/Services/ProxyServerTester/"]
RUN dotnet restore "src/backend/Services/ProxyServerTester/ProxyServerTester/ProxyServerTester.csproj"
COPY . .
WORKDIR "/src/src/backend/Services/ProxyServerTester/ProxyServerTester"
RUN dotnet build "./ProxyServerTester.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProxyServerTester.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProxyServerTester.dll"]
