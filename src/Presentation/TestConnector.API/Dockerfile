FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5025

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Presentation/TestConnector.API/TestConnector.API.csproj", "Presentation/TestConnector.API/"]
COPY ["Core/TestConnector.Domain/TestConnector.Domain.csproj", "Core/TestConnector.Domain/"]
COPY ["Infrastructure/TestConnector.Infrastructure/TestConnector.Infrastructure.csproj", "Infrastructure/TestConnector.Infrastructure/"]
RUN dotnet restore "Presentation/TestConnector.API/TestConnector.API.csproj"
COPY . .
WORKDIR "/src/Presentation/TestConnector.API"
RUN dotnet build "TestConnector.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TestConnector.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestConnector.API.dll"]
