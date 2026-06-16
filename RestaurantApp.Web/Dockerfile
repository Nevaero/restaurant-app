# Build context is the repository root: docker build -f RestaurantApp.Web/Dockerfile .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RestaurantApp.Web/*.csproj RestaurantApp.Web/
RUN dotnet restore RestaurantApp.Web/RestaurantApp.Web.csproj

COPY RestaurantApp.Web/ RestaurantApp.Web/
RUN dotnet publish RestaurantApp.Web/RestaurantApp.Web.csproj -c Release -o /app/publish

# Serve the app at the same origin as the API: blank ApiBaseUrl makes the client use
# its own origin, and nginx (below) reverse-proxies /api to the API container.
RUN printf '{\n  "ApiBaseUrl": ""\n}\n' > /app/publish/wwwroot/appsettings.json

FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY RestaurantApp.Web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
