FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CodeToSurviveResource/CodeToSurviveResource.fsproj", "CodeToSurviveResource/"]
RUN dotnet restore "CodeToSurviveResource/CodeToSurviveResource.fsproj"
COPY . .
WORKDIR "/src/CodeToSurviveResource"
RUN dotnet build "CodeToSurviveResource.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeToSurviveResource.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeToSurviveResource.dll"]
