# Les Jackson Microservices course 
Este projecto es el producto del curso de microservicios de Les Jacson, contiene 2 microservicios platform service y commands service. 

* Se hace uso de NGINX como API Gateway, 
* RabbitMQ como Messaging Bus and 
* gRPC as RPC framework

Este proyecto usa Docker para crear imagenes de microservicios y 
Docker Hub para almacenar estas imagenes en la nube.
Los contenedores estan orquestados usando archivos yaml y Kubernetes.

https://www.youtube.com/watch?v=DgVjEo3OGBI&t=9592s&ab_channel=LesJackson

Repo microservices-course
 
## Les Jackson github repository
https://github.com/binarythistle

https://github.com/binarythistle/S04E03---.NET-Microservices-Course-

https://dotnetplaybook.com/ 


**Comando para crear proyecto web api**
dotnet new webapi -n PlatformService 

**Comando para crear proyecto web api con el framework especifico**

dotnet new webapi -n PlatformService --framework net5.0


Abrir folder del proyecto con code

code -r PlatformService

**Instalar packetes** 
* dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 8.1.1
* dotnet add package Microsoft.EntityFrameworkCore --version 5.0.8
* dotnet add package Microsoft.EntityFrameworkCore.Design --version 5.0.8
* dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 5.0.8
* dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 5.0.8

** Build the project
dotnet build

>Para poblar la base de datos usando el contexto
se debe crear una clase llamada PrepDb en la carpeta de Data. En la clase de Startup agregar la linea *PrepDb.PrepPopulation(app);* para que se ejecute la poblacion de datos.

* Para correr el proyecto
dotnet run

## Dockers ##
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

**Instalar Docker desktop**
https://docs.docker.com/desktop/setup/install/windows-install/

### Dockers CLI Cheat Sheet ###
https://docs.docker.com/get-started/docker_cheatsheet.pdf

Comando para construir un contenedor

docker build -t [docker id]/[nombre de la app]

ejemplo:

\`docker build -t ahernandezcarrillo/platformservice .\`

>Nota: el docker id es el nombre de usuario de docker hub. 
>El nombre de la app debe estar totalmente en minusculas
>El punto al final es muy importante

Subir el contenedor
docker push ahernandezcarrillo/platformservice

* Ejecuta un docker 

docker run -p 8080:80 -d ahernandezcarrillo/platformservice
>Npta el parametro puerto 8080 es el puerto desde donde se accede a la aplicacion en el contenedor
>Nota: Cada que se ejecuta un contenedor se crea uno nuevo, si se desea reiniciar un contenedor ya existente usar start.

* Listar los contenedores que estan corriendo

docker ps

* Detener un contenedor

docker stop [Container ID]

* Reiniciar un contenedor

docker start [container ID]

* Subir el contenedor 

docker push [docker id]/[nombre de la app]

Ejemplo: 

docker push ahernandezcarrillo/platformservice

Para ver el contenedor corriendo desde un browser o postma hay que acceder a 
http://localhost:8080/api/platforms


# Kubernetes # 
2:42:00

Kubernetes es un conjunto de contenedores, kubernetes se encarga de manejar u orquestar un conjunto de contenedores para que funcionen de forma coordinada.

### Kubernetes Cheat Sheets ###
https://kubernetes.io/docs/reference/kubectl/quick-reference/

https://spacelift.io/blog/kubernetes-cheat-sheet

https://medium.com/@kv2023/kubernetes-kubectl-command-cheat-sheet-3f09ddf47cea

### Arquitectura Kubernetes ###

Kubernetes se compone de varios elementos

* Cluster: Un clúster en Kubernetes es un conjunto de nodos (máquinas) que trabajan juntos para ejecutar aplicaciones y servicios de forma orquestada.

* Nodo: Un nodo en Kubernetes es una máquina que forma parte del clúster y ejecuta aplicaciones y cargas de trabajo. Cada nodo puede ser una máquina física o una máquina virtual. Los nodos en un clúster Kubernetes trabajan juntos para ejecutar y gestionar aplicaciones de manera eficiente y escalable.

Hay dos tipos principales de nodos en Kubernetes:
 
Nodo Maestro (Control Plane Node): Responsable de la gestión y coordinación del clúster. Contiene componentes que gestionan el estado del clúster, planifican la ejecución de pods y coordinan la comunicación entre nodos. Incluye el kube-apiserver, etcd, kube-scheduler y kube-controller-manager.

Nodos Trabajadores (Worker Nodes): Donde realmente se ejecutan los pods y las aplicaciones. Cada nodo trabajador contiene el kubelet (agente que se comunica con el nodo maestro), el kube-proxy (gestiona la red del clúster) y un motor de contenedores como Docker o containerd.

En resumen:

Nodos Maestros: Gestionan el clúster.

Nodos Trabajadores: Ejecutan las aplicaciones.

* Pod: Un pod en Kubernetes es la unidad más pequeña y fundamental de ejecución en el sistema. Es un grupo lógico que puede contener **uno o varios contenedores Docker que comparten recursos de red y almacenamiento**. Los contenedores dentro de un pod están diseñados para ejecutarse juntos y se benefician de compartir el mismo espacio de red, permitiendo una comunicación eficiente.

Características clave de un pod:

**Aislamiento:** Los contenedores dentro de un pod comparten el mismo espacio de red y almacenamiento, lo que facilita la comunicación entre ellos.

**Ephemeral:** Los pods están diseñados para ser temporales, pudiendo ser creados y eliminados según sea necesario para escalar aplicaciones, manejar fallos, o realizar actualizaciones.

**Escalabilidad:** Kubernetes se encarga de gestionar los pods para asegurar que las aplicaciones se ejecuten de manera eficiente y puedan escalar según la demanda.

Los pods son gestionados por el nodo maestro y se ejecutan en los nodos trabajadores dentro del clúster de Kubernetes.

En nuestro proyecto existe 1 Nodo con 5 pods / Containers

1. Pod Platform Service Container port 80:666
2. Pod Command Service Container port 80
3. Pod Platform SQL Server Container port 1433
4. Pod Ingress Nginx Container port 80: este pod ayuda a balancear el trafico
5. Pod RabbitMQ Container port 5672: 15672 

El Node Port es el puerto de comunicacion hacia el exterior 3xxxx que apunta al puerto 80 interior que apunta al puerto 80 del pod Platform service 
Hay un puerto 80 para el Ingress Nginx load balancer

Todos los Nodos comparten el Cluster IP

Insertar imagen 002xxx.jpg aqui

3:00:00
### YAML files ###
Crear folder K8S al mismo nivel de nuestro proyecto platform service
Dentro de este folder crear archivo platforms-depl.yaml

verificar version de kubernetes
kubectl version

>Para aplicar el archivo yaml platforms-service

kubectl apply -f platforms-depl.yaml

>Para ver los deployiments

kubectl get deployments

>Para ver los pods

kubectl get pods

>Para eliminar un deployment

kubectl delete deployment platforms-depl

>Para ver los servicios

kubectl get service

>Para aplicar el archivo yaml 

kubectl apply -f platform-np-srv.yaml

>Para ver la app funcionando en kubernetes correr el comando 

kubectl get service

>verificar el puerto del servicio platformnpservice-srv 
en este caso es 30470

insertar aqui imagen 003.jpg

3:22:00
## Crear segundo Proyecto CommandsService ##

*Agregar nuevo proyect .net core 5.0*

dotnet new webapi -n CommandsService --framework net5.0

**Instalar packetes** 
* dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection --version 8.1.1
* dotnet add package Microsoft.EntityFrameworkCore --version 5.0.8
* dotnet add package Microsoft.EntityFrameworkCore.Design --version 5.0.8
* dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 5.0.8

>Nota: El paquete de sql no es agregado a este proyecto

Eliminar el controlador y la clase de weather

En el archivo launchSettings.json cambiar los puertos 5000 y 5001 por 6000 y 6001
para evitar usar los mismos del otro servicio


Asi quedaria:

\`\`\`
"applicationUrl": "https://localhost:6001;http://localhost:6000"
\`\`\`

## Mensajeria Sincrona ##
(Synchronous Messaging)
La mensajeria sincrona se base en un ciclo de request / response, donde el "requester" tiene que esperar "await" por la respuesta "response".

* Request / Response Cycle.
* Requester will "wait" for response.
* Externally facing Services usually synchronous (e.g. http requests)
* Services Usually need to "know" about each other.

We are using 2 forms
* Http
* Grpc

## Mensajeria Asincrona ##
(Asynchronous Messaging)

* No Request / Response Cycle.
* Requester does not "wait" for response.
* Event model, e.g. publish - subscribe
* Typically used between services.
* Event bus is often used (we will using RabbitMQ)
* Services don't need to "know" about each other just the bus.

***RabbitMQ*** es un software de cola de mensajes (message broker) que permite la comunicación entre aplicaciones distribuidas. Actúa como intermediario, recibiendo mensajes de una aplicación y entregándolos a otra, facilitando la comunicación asíncrona y mejorando la escalabilidad y la tolerancia a fallos.


**Resumen hasta el momento** 
* Crear un proyecto webapi para framework 5.0 llamado PlatformService
* Preparar este proyecto para trabajar con entity Framework
* Crear la infaestrucctura de datos EF context, Modelos, Dtos, automapper profiles, etc.
* Crear la interfaz repository con los metodos SaveChanges, GetAllPlatforms, * GetPlatformById, CreatePlatform(Platform plat).
* Implementar la interfaz
* Crear el controlador Platformservice e injectar las interfaces IPlatformRepo, IMapper y ICommandDataClient

* Implementar los siguientes endpoints
GetPlatforms, GetPlatformById y CreatePlatform

* Crear dockerfile para platforms
* Crear y publicar imagen del Platform Service
* Crear un kuberbete para ejecutar Platform Service

* Crear otro Projecto webapi pare el framework 5.0 llamado commandsService
* Agregar las mismas referencias que el proyecto anterior excepto sql
* Agregar un controlador para *PlatformService*
* En este controlador solo agregamos un endopoint llamad TestInboundConnection de tipo POST
* 

* Regresamos al proyecto PlatformService y agregamos el folder SyncDataservices/http 
* Creamos una interfaz ICommandDataCient
* Creamos una clase llamada HttpCommandDataClient que implementa la interfaz antes creada
* A esta clase le injectamos las interfaces HttpClient y IConfiguration 

4:16:00 

En este momento debemos ejecutar el commandService y el platformService y agregar una nueva plataforma desde postman, si todo sale bien se creara sin ningun contratiempo.

Ahora detenemos el commandService y volvemos a agregar una plataforma desde postman, ahora va a tardar mucho en ejecutarla, despues de un tiempo la va a crear pre en la consola  recibiremos este mensaje: "--> Could not send synchronosyly: No se puede establecer una conexión ya que el equipo de destino denegó expresamente dicha conexión. (localhost:6000)"

Crear archivo dockerfile para commandsService
docker build -t ahernandezcarrillo/commandservice .

docker push ahernandezcarrillo/commandservice

docker run -p 8080:80 ahernandezcarrillo/commandservice

kubectl apply -f commands-depl.yaml

>Restart deployment
kubectl rollout restart deployment platforms-depl 

>Nota el error "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3] Failed to determine the https port for redirect." se soluciona comentando o eliminado la linea app.UseHttpsRedirection(); del archivo start de los 2 proyectos.

