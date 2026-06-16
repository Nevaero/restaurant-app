# Build context is the repository root: docker build -f RestaurantApp.Api/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore (copy only csproj files first for better layer caching)
COPY RestaurantApp.Core/*.csproj RestaurantApp.Core/
COPY RestaurantApp.Infrastructure/*.csproj RestaurantApp.Infrastructure/
COPY RestaurantApp.Api/*.csproj RestaurantApp.Api/
RUN dotnet restore RestaurantApp.Api/RestaurantApp.Api.csproj

# Build + publish
COPY RestaurantApp.Core/ RestaurantApp.Core/
COPY RestaurantApp.Infrastructure/ RestaurantApp.Infrastructure/
COPY RestaurantApp.Api/ RestaurantApp.Api/
RUN dotnet publish RestaurantApp.Api/RestaurantApp.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
WORKDIR /app

# QuestPDF / SkiaSharp need fontconfig + a font available at runtime.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# SQLite database lives on a mounted volume so it survives container restarts.
RUN mkdir -p /app/data
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    ConnectionStrings__Default="Data Source=/app/data/restaurant.db"

EXPOSE 8080
VOLUME /app/data
ENTRYPOINT ["dotnet", "RestaurantApp.Api.dll"]
