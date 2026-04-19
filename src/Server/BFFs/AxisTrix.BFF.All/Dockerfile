FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Server/BFFs/AxisTrix.BFF.All/AxisTrix.BFF.All.csproj", "src/Server/BFFs/AxisTrix.BFF.All/"]
RUN dotnet restore "src/Server/BFFs/AxisTrix.BFF.All/AxisTrix.BFF.All.csproj"
COPY . .
WORKDIR "/src/src/Server/BFFs/AxisTrix.BFF.All"
RUN dotnet build "./AxisTrix.BFF.All.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AxisTrix.BFF.All.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AxisTrix.BFF.All.dll"]
