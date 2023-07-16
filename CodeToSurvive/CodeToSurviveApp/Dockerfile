FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CodeToSurviveRunner/CodeToSurviveApp.fsproj", "CodeToSurviveApp/"]
RUN dotnet restore "CodeToSurviveRunner/CodeToSurviveApp.fsproj"
COPY . .
WORKDIR "/src/CodeToSurviveApp"
RUN dotnet build "CodeToSurviveApp.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeToSurviveApp.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeToSurviveApp.dll"]
