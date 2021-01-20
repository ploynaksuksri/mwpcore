#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
EXPOSE 80
EXPOSE 443

#COPY ["certificates/", "https/"]
#
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
#WORKDIR /src
#COPY ["src/Mwp.HttpApi.Host/Mwp.HttpApi.Host.csproj", "src/Mwp.HttpApi.Host/"]
#COPY ["src/Mwp.EntityFrameworkCore.DbMigrations/Mwp.EntityFrameworkCore.DbMigrations.csproj", "src/Mwp.EntityFrameworkCore.DbMigrations/"]
#COPY ["src/Mwp.EntityFrameworkCore/Mwp.EntityFrameworkCore.csproj", "src/Mwp.EntityFrameworkCore/"]
#COPY ["src/Mwp.Domain/Mwp.Domain.csproj", "src/Mwp.Domain/"]
#COPY ["src/Mwp.Domain.Shared/Mwp.Domain.Shared.csproj", "src/Mwp.Domain.Shared/"]
#COPY ["src/Mwp.HttpApi/Mwp.HttpApi.csproj", "src/Mwp.HttpApi/"]
#COPY ["src/Mwp.Application.Contracts/Mwp.Application.Contracts.csproj", "src/Mwp.Application.Contracts/"]
#COPY ["src/Mwp.Application/Mwp.Application.csproj", "src/Mwp.Application/"]
#RUN dotnet restore "src/Mwp.HttpApi.Host/Mwp.HttpApi.Host.csproj"
#COPY . .
#WORKDIR "/src/src/Mwp.HttpApi.Host"
#RUN dotnet build "Mwp.HttpApi.Host.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "Mwp.HttpApi.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
#COPY --from=publish /app/publish .
COPY src/Mwp.HttpApi.Host/publish .
ENTRYPOINT ["dotnet", "Mwp.HttpApi.Host.dll"]