FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RestorationBot/RestorationBot.csproj", "RestorationBot/"]
RUN dotnet restore "RestorationBot/RestorationBot.csproj"
COPY RestorationBot/appsettings.json ./
COPY RestorationBot/appsettings.Release.json ./
COPY . .
WORKDIR "/src/RestorationBot"
RUN dotnet build "RestorationBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RestorationBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY RestorationBot/appsettings.Release.json /app/appsettings.Release.json
ENTRYPOINT ["dotnet", "RestorationBot.dll"]
