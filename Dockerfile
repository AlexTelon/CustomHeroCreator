FROM microsoft/dotnet:2.0.0-sdk AS build-env

WORKDIR /app

COPY CustomHeroCreator .

RUN dotnet restore

# RUN dotnet publish -c Release -r win10-x64 -o out
RUN dotnet publish -c Release -r linux-x64 -o out

FROM microsoft/dotnet:2-runtime-jessie
WORKDIR /app
COPY --from=build-env /app/out ./
ENTRYPOINT ["./CustomHeroCreator"]
