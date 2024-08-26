#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ImbaBetWeb/ImbaBetWeb.csproj", "ImbaBetWeb/"]
RUN dotnet restore "./ImbaBetWeb/ImbaBetWeb.csproj"
COPY . .
WORKDIR "/src/ImbaBetWeb"
RUN dotnet build "./ImbaBetWeb.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ImbaBetWeb.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER root
RUN mkdir -p /app/wwwroot/Resources/UserUploads
RUN chown -R app /app/wwwroot/Resources/UserUploads
RUN chmod 755 /app/wwwroot/Resources/UserUploads
USER app


ENTRYPOINT ["dotnet", "ImbaBetWeb.dll"]