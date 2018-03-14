FROM microsoft/dotnet:2.0.0-sdk AS build

WORKDIR /app
#RUN dotnet restore

COPY . .
WORKDIR /app/CustomHeroCreator
RUN dotnet build

FROM build AS testrunner
WORKDIR /app/XUnitTests
#ENTRYPOINT ["dotnet", "test", "XUnitTests.csproj ", "--logger:trx"]
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM build AS test
WORKDIR /app/XUnitTests
RUN dotnet test

FROM build AS publish
WORKDIR /app/CustomHeroCreator
RUN dotnet publish -c Release -r linux-x64 -o out

FROM microsoft/dotnet:2-runtime-jessie AS runtime
WORKDIR /app
COPY --from=publish /app/CustomHeroCreator/out ./
ENTRYPOINT ["./CustomHeroCreator"]
