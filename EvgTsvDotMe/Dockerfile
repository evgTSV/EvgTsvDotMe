FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:10.0.5-alpine3.23 AS base
WORKDIR /app

ARG TARGETARCH
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_HTTPS_PORTS=443
EXPOSE 8080
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0.201-alpine3.23 AS build
WORKDIR /src

COPY . .

RUN apk add --no-cache nodejs npm curl

RUN dotnet tool restore
RUN dotnet paket update
RUN dotnet paket restore

RUN dotnet restore "EvgTsvDotMe/EvgTsvDotMe.fsproj"

WORKDIR /src/Tailwind
RUN npm install && npm run build

WORKDIR /src/EvgTsvDotMe
RUN dotnet publish "EvgTsvDotMe.fsproj" -a "$TARGETARCH" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "EvgTsvDotMe.dll"]
