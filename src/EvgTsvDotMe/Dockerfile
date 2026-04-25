FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0.5-alpine3.23 AS base
WORKDIR /app

ARG TARGETARCH
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_HTTPS_PORTS=443
EXPOSE 8080
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.201-alpine3.23 AS build
COPY /src /src

COPY /global.json /global.json
COPY /paket.dependencies /paket.dependencies
COPY /paket.lock /paket.lock
COPY /.fantomasignore /.fantomasignore
COPY /.config/dotnet-tools.json /.config/dotnet-tools.json
COPY /build /build

WORKDIR /

RUN apk add --no-cache nodejs npm curl

RUN dotnet tool restore
RUN dotnet paket restore

RUN dotnet run --project build/build.fsproj -t Publish -- --TargetProject "src/EvgTsvDotMe/EvgTsvDotMe.fsproj" --Arch "$TARGETARCH" --PublishOutput "/app/publish"

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "EvgTsvDotMe.dll"]
