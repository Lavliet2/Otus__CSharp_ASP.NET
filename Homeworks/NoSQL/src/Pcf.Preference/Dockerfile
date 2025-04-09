FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Pcf.Preference.Core/Pcf.Preference.Core.csproj ./Pcf.Preference.Core/
COPY Pcf.Preference.DataAccess/Pcf.Preference.DataAccess.csproj ./Pcf.Preference.DataAccess/
COPY Pcf.Preference.WebHost/Pcf.Preference.WebHost.csproj ./Pcf.Preference.WebHost/

RUN dotnet restore ./Pcf.Preference.WebHost/Pcf.Preference.WebHost.csproj

COPY . .
WORKDIR /app/Pcf.Preference.WebHost
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/Pcf.Preference.WebHost/out ./

ENTRYPOINT ["dotnet", "Pcf.Preference.WebHost.dll"]
