# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Client.WASM/Client.WASM.csproj Client.WASM/
COPY Client.WASM/ Client.WASM/
COPY . .
RUN dotnet publish Client.WASM/Client.WASM.csproj -c Release -o /app

# Serve stage
FROM nginx:alpine AS serve
WORKDIR /usr/share/nginx/html
COPY --from=build /app/wwwroot .
COPY ./Client.WASM/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]