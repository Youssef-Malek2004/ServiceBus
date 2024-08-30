FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ESB.Core/ESB.Core.csproj", "ESB.Core/"]
COPY ["ESB.Infrastructure/ESB.Infrastructure.csproj", "ESB.Infrastructure/"]
COPY ["ESB.Configurations/ESB.Configurations.csproj", "ESB.Configurations/"]
RUN dotnet restore "ESB.Core/ESB.Core.csproj"
COPY . .
WORKDIR "/src/ESB.Core"
RUN dotnet build "ESB.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ESB.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ESB.Core.dll"]
