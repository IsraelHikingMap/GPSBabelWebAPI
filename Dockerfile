FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM microsoft/aspnetcore:2.0
EXPOSE 11984
ENV LD_LIBRARY_PATH=.
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GPSBabelWebAPI.dll"]
