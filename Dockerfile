FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV DB_PROVIDER=Sqlite
# Ensure persistent data directory exists (can be mounted as volume)
RUN mkdir -p /app/data
VOLUME ["/app/data"]
ENTRYPOINT ["dotnet", "DigitalisierungsManager.dll"]


