FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
WORKDIR "HillMetrics.MIND/src/HillMetrics.MIND.API/"
RUN dotnet restore "HillMetrics.MIND.API.csproj"
RUN dotnet build "HillMetrics.MIND.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HillMetrics.MIND.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

CMD ["dotnet", "HillMetrics.MIND.API.dll"]
