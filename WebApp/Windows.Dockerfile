# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0-nanoserver-1809 AS build
ARG app_version=1.0.0.0
ARG source_version=local
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app /p:Version=%app_version% /p:SourceRevisionId=%source_version%
# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:6.0-nanoserver-1809 AS final
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "InspectorGadget.WebApp.dll"]