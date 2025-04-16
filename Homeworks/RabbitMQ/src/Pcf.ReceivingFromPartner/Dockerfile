# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Pcf.Shared/Pcf.Shared.csproj Pcf.Shared/
COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Core/Pcf.ReceivingFromPartner.Core.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Core/
COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.DataAccess/Pcf.ReceivingFromPartner.DataAccess.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.DataAccess/
COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Integration/Pcf.ReceivingFromPartner.Integration.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.Integration/
COPY Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/Pcf.ReceivingFromPartner.WebHost.csproj Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/

RUN dotnet restore Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost/Pcf.ReceivingFromPartner.WebHost.csproj

COPY Pcf.Shared/ Pcf.Shared/
COPY Pcf.ReceivingFromPartner/ Pcf.ReceivingFromPartner/

WORKDIR /app/Pcf.ReceivingFromPartner/Pcf.ReceivingFromPartner.WebHost
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "Pcf.ReceivingFromPartner.WebHost.dll"]
