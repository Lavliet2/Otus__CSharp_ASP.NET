# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Pcf.sln ./
COPY Pcf.Shared/Pcf.Shared.csproj Pcf.Shared/
COPY Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/Pcf.GivingToCustomer.Core.csproj Pcf.GivingToCustomer/Pcf.GivingToCustomer.Core/
COPY Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/Pcf.GivingToCustomer.DataAccess.csproj Pcf.GivingToCustomer/Pcf.GivingToCustomer.DataAccess/
COPY Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Pcf.GivingToCustomer.WebHost.csproj Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/

RUN dotnet restore Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost/Pcf.GivingToCustomer.WebHost.csproj

COPY Pcf.Shared/ Pcf.Shared/
COPY Pcf.GivingToCustomer/ Pcf.GivingToCustomer/

WORKDIR /app/Pcf.GivingToCustomer/Pcf.GivingToCustomer.WebHost
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pcf.GivingToCustomer.WebHost.dll"]
