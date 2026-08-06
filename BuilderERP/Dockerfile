FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BuilderERP.sln", "."]
COPY ["src/BuilderERP.Domain/BuilderERP.Domain.csproj", "src/BuilderERP.Domain/"]
COPY ["src/BuilderERP.Application/BuilderERP.Application.csproj", "src/BuilderERP.Application/"]
COPY ["src/BuilderERP.Infrastructure/BuilderERP.Infrastructure.csproj", "src/BuilderERP.Infrastructure/"]
COPY ["src/BuilderERP.Shared/BuilderERP.Shared.csproj", "src/BuilderERP.Shared/"]
COPY ["src/BuilderERP.Web/BuilderERP.Web.csproj", "src/BuilderERP.Web/"]
RUN dotnet restore "src/BuilderERP.Web/BuilderERP.Web.csproj"

COPY . .
WORKDIR "/src/src/BuilderERP.Web"
RUN dotnet build "BuilderERP.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BuilderERP.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BuilderERP.Web.dll"]
