# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Pcf.sln ./
COPY Pcf.Shared/Pcf.Shared.csproj Pcf.Shared/
COPY Pcf.Administration/Pcf.Administration.Core/Pcf.Administration.Core.csproj Pcf.Administration/Pcf.Administration.Core/
COPY Pcf.Administration/Pcf.Administration.DataAccess/Pcf.Administration.DataAccess.csproj Pcf.Administration/Pcf.Administration.DataAccess/
COPY Pcf.Administration/Pcf.Administration.WebHost/Pcf.Administration.WebHost.csproj Pcf.Administration/Pcf.Administration.WebHost/

RUN dotnet restore Pcf.Administration/Pcf.Administration.WebHost/Pcf.Administration.WebHost.csproj

COPY Pcf.Shared/ Pcf.Shared/
COPY Pcf.Administration/ Pcf.Administration/

WORKDIR /app/Pcf.Administration/Pcf.Administration.WebHost
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pcf.Administration.WebHost.dll"]
