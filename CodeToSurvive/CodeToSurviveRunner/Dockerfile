FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CodeToSurviveRunner/CodeToSurviveRunner.fsproj", "CodeToSurviveRunner/"]
RUN dotnet restore "CodeToSurviveRunner/CodeToSurviveRunner.fsproj"
COPY . .
WORKDIR "/src/CodeToSurviveRunner"
RUN dotnet build "CodeToSurviveRunner.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeToSurviveRunner.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeToSurviveRunner.dll"]
