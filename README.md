# Les Jackson Microservices course 
Repo microservices-course
 
# Les Jackson github repository
https://github.com/binarythistle

https://github.com/binarythistle/S04E03---.NET-Microservices-Course-


**Comando para crear proyecto web api**
dotnet new webapi -n PlatformService 

**Comando para crear proyecto web api con el framework especifico**

dotnet new webapi -n PlatformService --framework net5.0


Abrir folder del proyecto con code

code -r PlatformService

**Instalar packetes** 
* dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection (Version="8.1.1")
* dotnet add package Microsoft.EntityFrameworkCore (Version="5.0.8)
* dotnet add package Microsoft.EntityFrameworkCore.Design (Version="5.0.8)
* dotnet add package Microsoft.EntityFrameworkCore.InMemory (Version="5.0.8)
* dotnet add package Microsoft.EntityFrameworkCore.SqlServer (Version="5.0.8)

** Build the project
dotnet build

>Para poblar la base de datos usando el contexto
se debe crear una clase llamada PrepDb en la carpeta de Data. En la clase de Startup agregar la linea *PrepDb.PrepPopulation(app);* para que se ejecute la poblacion de datos.

* Para correr el proyecto
dotnet run

# Dockers #
Crear archivo Dockerfile en la raiz 

>Nota: Se puede crear automaticamente el archivo docker desde VS code usando la extension de Docker para VS. Para verla hay que ir a la seccion de extensiones y usar este filtro \`@id:ms-azuretools.vscode-docker\`

**Docker hub**
https://hub.docker.com

**Crear un archivo Dockerfile**
https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows&pivots=dotnet-9-0#create-the-dockerfile

**Ejemplo SDK 9.0**

\`\`\`
>FROM mcr.microsoft.com/dotnet/sdk:90@sha256:3fcf6f1e809c0553f9feb222369f58749af314af6f063f389cbd2f913b4ad556 AS build
>WORKDIR /App
>
>\# Copy everything
>
>COPY . ./
>
>\# Restore as distinct layers
>
>RUN dotnet restore
>
>\# Build and publish a release
>
>RUN dotnet publish -o out
>
>\# Build runtime image
>
>FROM mcr.microsoft.com/dotnet/aspnet:9.0@sha256:b4bea3a52a0a77317fa93c5bbdb076623f81e3e2f201078d89914da71318b5d8
>
>WORKDIR /App
>
>COPY --from=build /App/out .
>
>ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]

\`\`\`

**Ejemplo para nuestro curso SDK 5.0**

>FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
>
>WORKDIR /app
>
>COPY *.csproj ./
>
>COPY . ./
>
>RUN dotnet publish -c Release -o out
>
>FROM mcr.microsoft.com/dotnet/aspnet:5.0
>
>WORKDIR /app
>
>COPY --from=build-env /app/out .
>
>ENTRYPOINT ["dotnet", "PlatformService.dll"]
