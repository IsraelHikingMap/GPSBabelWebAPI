FROM mcr.microsoft.com/dotnet/sdk:5.0.300 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
RUN apt-get update -y && apt-get install -y gpsbabel && apt-get install -y curl
COPY --from=build-env /app/publish .
EXPOSE 80
HEALTHCHECK --interval=5s --timeout=3s CMD curl --fail http://localhost:80/api/health || exit 1 
ENTRYPOINT ["dotnet", "GPSBabelWebAPI.dll"]
