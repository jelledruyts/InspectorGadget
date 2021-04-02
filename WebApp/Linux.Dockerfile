# Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ARG app_version=1.0.0.0
ARG source_version=local
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app /p:Version=${app_version} /p:SourceRevisionId=${source_version}
# Stage 2
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS final
EXPOSE 5000
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "InspectorGadget.WebApp.dll"]