FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

WORKDIR /app
EXPOSE 800
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . .
RUN dotnet restore ./AuthenticationService.Web
#COPY . .
#WORKDIR "/AuthenticationService.Web"
#RUN dotnet build AuthenticationService.Web/*.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish AuthenticationService.Web/*.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationService.Web.dll"]
