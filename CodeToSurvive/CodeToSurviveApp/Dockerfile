FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CodeToSurviveApp/CodeToSurviveApp.fsproj", "CodeToSurviveApp/"]
RUN dotnet restore "CodeToSurviveApp/CodeToSurviveApp.fsproj"
COPY . .
WORKDIR "/src/CodeToSurviveApp"
RUN dotnet build "CodeToSurviveApp.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CodeToSurviveApp.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CodeToSurviveApp.dll"]
