#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/api/WebApplication/WebApplication.csproj", "src/api/WebApplication/"]
COPY ["src/common/EventStore/EventStore.csproj", "src/common/EventStore/"]
COPY ["src/common/Domain/Domain.csproj", "src/common/Domain/"]
COPY ["src/handlers/CartHandler/CartHandler.csproj", "src/handlers/CartHandler/"]
COPY ["src/common/ReadModel/ReadModel.csproj", "src/common/ReadModel/"]
RUN dotnet restore "src/api/WebApplication/WebApplication.csproj"
COPY . .
WORKDIR "/src/src/api/WebApplication"
RUN dotnet build "WebApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication.dll"]