# The website image with the game client baked in, for self-hosting.
# Upstream CI serves the client from a CDN instead.
# Build from the repo root:
#   docker build -f selfhost.website.Dockerfile .
# image-webpack-loader ships x86-only binaries.
FROM --platform=linux/amd64 node:24 AS client
WORKDIR /app
COPY agot-bg-game-server/package.json agot-bg-game-server/yarn.lock ./
RUN yarn install --frozen-lockfile
COPY agot-bg-game-server/ .
RUN yarn run generate-json-schemas && yarn run build-client

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY agot-bg-website-dotnet/ .
COPY --from=client /app/dist/ agot-bg-website/wwwroot/static_game/
RUN mkdir -p agot-bg-website/GameClientTemplates \
 && mv agot-bg-website/wwwroot/static_game/index.html agot-bg-website/GameClientTemplates/play.html \
 && dotnet publish agot-bg-website/agot-bg-website.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
USER root
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8000 8001
ENTRYPOINT ["dotnet", "agot-bg-website.dll"]
