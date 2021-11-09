#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Auth-Service.Web/Auth-Service.Web.csproj", "Auth-Service.Web/"]
RUN dotnet restore "Auth-Service.Web/Auth-Service.Web.csproj"
COPY . .
WORKDIR "/src/Auth-Service.Web"
RUN dotnet build "Auth-Service.Web.csproj" -c Release -o /app/build

FROM build AS dev
WORKDIR "/src/Auth-Service.Web"
ENTRYPOINT ["dotnet", "watch", "run", "--urls=http://5000"]

FROM build AS publish
RUN dotnet publish "Auth-Service.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth-Service.Web.dll"]