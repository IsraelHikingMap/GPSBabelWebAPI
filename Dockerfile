FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
RUN apt-get update -y && apt-get install -y gpsbabel
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "GPSBabelWebAPI.dll"]
