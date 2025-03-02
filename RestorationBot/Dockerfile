FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RestorationBot/RestorationBot.csproj", "RestorationBot/"]
RUN dotnet restore "RestorationBot/RestorationBot.csproj"
COPY . .
WORKDIR "/src/RestorationBot"
RUN dotnet build "RestorationBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RestorationBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY RestorationBot/appsettings*.json ./

COPY RestorationBot/Resources/ Resources/
ENTRYPOINT ["dotnet", "RestorationBot.dll"]
