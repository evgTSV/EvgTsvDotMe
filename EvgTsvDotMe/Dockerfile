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

COPY .paket/. ./src/

COPY EvgTsvDotMe/. ./src/

COPY Tailwind ./src/Tailwind

RUN apk add --no-cache nodejs npm

RUN dotnet restore "src/EvgTsvDotMe/EvgTsvDotMe.fsproj" --configfile nuget.config --nologo -v q -a "$TARGETARCH" --property WarningLevel=0 /clp:ErrorsOnly

WORKDIR "src/EvgTsvDotMe/"

RUN dotnet build "EvgTsvDotMe.fsproj" -a "$TARGETARCH" -c Release --no-restore -o /app/build --nologo -v q --property WarningLevel=0 /clp:ErrorsOnly

WORKDIR "src/Tailwind"

RUN npm install

RUN npm run build

WORKDIR "src/EvgTsvDotMe/"

FROM build AS publish
RUN dotnet publish "EvgTsvDotMe.fsproj" -a "$TARGETARCH" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER $APP_UID
ENTRYPOINT ["dotnet", "EvgTsvDotMe.dll"]
