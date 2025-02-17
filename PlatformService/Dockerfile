# Ejemplo de Docker file autogenerado por la extension de docker de Visual Studio
# usando el filtro de extensiones @id:ms-azuretools.vscode-docker

# FROM mcr.microsoft.com/dotnet/aspnet:5.0-nanoserver-1809 AS base
# WORKDIR /app
# EXPOSE 5000

# ENV ASPNETCORE_URLS=http://+:5000

# FROM mcr.microsoft.com/dotnet/sdk:5.0-nanoserver-1809 AS build
# ARG configuration=Release
# WORKDIR /src
# COPY ["PlatformService.csproj", "./"]
# RUN dotnet restore "PlatformService.csproj"
# COPY . .
# WORKDIR "/src/."
# RUN dotnet build "PlatformService.csproj" -c $configuration -o /app/build

# FROM build AS publish
# ARG configuration=Release
# RUN dotnet publish "PlatformService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "PlatformService.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.csproj ./

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "PlatformService.dll"]