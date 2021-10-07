FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

COPY . .
RUN dotnet restore ./AuthenticationService.Web

FROM build as dev
CMD ["dotnet", "watch", "--project", "/AuthenticationService.Web/AuthenticationService.Web.csproj", "run"]

FROM build AS publish
RUN dotnet publish AuthenticationService.Web/*.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationService.Web.dll"]
