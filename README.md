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

>Nota: Se puede crear automaticamente el archivo docker desde VS code usando la extension de Docker para VS. Para verla hay que ir a la seccion de extensiones y usar este filtro `@id:ms-azuretools.vscode-docker`

**Docker hub**
https://hub.docker.com

**Crear un archivo Dockerfile**
https://learn.microsoft.com/en-us/dotnet/core/docker/build-container?tabs=windows&pivots=dotnet-9-0#create-the-dockerfile

**Ejemplo SDK 9.0**

```
FROM mcr.microsoft.com/dotnet/sdk:90@sha256:3fcf6f1e809c0553f9feb222369f58749af314af6f063f389cbd2f913b4ad556 AS build
WORKDIR /App

\# Copy everything

COPY . ./

\# Restore as distinct layers

RUN dotnet restore

\# Build and publish a release

RUN dotnet publish -o out

\# Build runtime image

FROM mcr.microsoft.com/dotnet/aspnet:9.0@sha256:b4bea3a52a0a77317fa93c5bbdb076623f81e3e2f201078d89914da71318b5d8

WORKDIR /App

COPY --from=build /App/out .

ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]

```

**Ejemplo para nuestro curso SDK 5.0**

```
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

WORKDIR /app

COPY *.csproj ./

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "PlatformService.dll"]
```


**Instalar Docker desktop**
https://docs.docker.com/desktop/setup/install/windows-install/

### Dockers CLI Cheat Sheet ###
https://docs.docker.com/get-started/docker_cheatsheet.pdf

Comando para construir un contenedor

docker build -t [docker id]/[nombre de la app]

ejemplo:

```docker build -t ahernandezcarrillo/platformservice .```

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

```
"applicationUrl": "https://localhost:6001;http://localhost:6000"
```

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

# Ingess nginx #
NGINX Ingress es un componente fundamental en la arquitectura de Kubernetes. Actúa como un controlador de entrada que gestiona el acceso externo a los servicios dentro del clúster de Kubernetes, redirigiendo las peticiones HTTP y HTTPS de los usuarios a los servicios correspondientes.

NGINX es una de las implementaciones más populares del controlador de entrada de Kubernetes debido a su eficiencia, rendimiento y flexibilidad. Permite a los administradores definir reglas de enrutamiento para el tráfico entrante, balanceo de carga, terminación SSL, y muchas otras funciones avanzadas para optimizar y asegurar el acceso a las aplicaciones desplegadas en Kubernetes.

References

https://github.com/kubernetes/ingress-nginx?tab=readme-ov-file
https://kubernetes.github.io/ingress-nginx/deploy/
https://kubernetes.github.io/ingress-nginx/deploy/#docker-desktop


>Crear el kuberdete de ingress nginx

```kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.0/deploy/static/provider/aws/deploy.yaml
```

Si vemos los deployments (kubectl get deployments) o los pods (kubectl get pods) no vamos a encontrar nada relacionado a ingress-nginx

Para ver lo que hay en el namespace usamos el siguiente commando: 

kubectl get namespace

ahi si vamos a encontrar el ingress-nginx

Para ver los pods dentro del namespace ingress-nginx ejecutamos 

kubectl get pods --namespace=ingress-nginx

obtener los servicios en el namespace ingress-nginx

kubectl get services --namespace=ingress-nginx

En el directorio de K8S agregar un nuevo archivo llamado ingress-srv.yaml

5:00:00

> Editar el archivo hosts ubicado en la ruta C:\Windows\System32\drivers\etc

en mi caso tengo que actualizar esta linea 

127.0.0.1 view-localhost # view localhost server

con esto 

127.0.0.1 acme.com

5:04 

kubectl apply -f ingress-srv.yaml

## Local persistance 

El comando *storageclass* se utiliza en Kubernetes para listar todas las clases de almacenamiento disponibles en el clúster. Las clases de almacenamiento definen las políticas de almacenamiento que se pueden aplicar a los volúmenes persistentes. Cada clase de almacenamiento puede especificar detalles como el proveedor de almacenamiento, las políticas de replicación, los parámetros de rendimiento, entre otros.

Cada clase de almacenamiento tiene un nombre (NAME), un provisionador (PROVISIONER), una política de recuperación (RECLAIMPOLICY), un modo de vinculación de volúmenes (VOLUMEBINDINGMODE), si permite la expansión de volumen (ALLOWVOLUMEEXPANSION) y su edad (AGE) en el clúster.

Ejemplo:
*kubectl get storageclass*

```
NAME                PROVISIONER                RECLAIMPOLICY   VOLUMEBINDINGMODE   ALLOWVOLUMEEXPANSION   AGE
standard            kubernetes.io/gce-pd       Delete          Immediate           true                   10d
fast                kubernetes.io/aws-ebs      Delete          Immediate           false                  5d
slow                kubernetes.io/glusterfs    Retain          WaitForFirstConsumer false                  20d
```

5:09 

**Crear archivo en K8s llamado local-pvc.yaml**
Este archivo nos permitira crear un contenedor de SQL server para guardar la informacion
que ingresemos a la applicacion. Cuando terminemos hay que aplicara el deployment 

kubectl apply -f local-pvc.yaml

### Persistent Volume Claims, PVC ###
Listar todos los volúmenes persistentes (Persistent Volume Claims, PVC) en el clúster. Un PVC es una solicitud de almacenamiento hecha por un usuario que especifica el tamaño y las características del volumen que necesitan para sus aplicaciones. El clúster intentará aprovisionar un volumen persistente (PV) que cumpla con los requisitos del PVC.

```kubectl get pvc```

# Setting up SQL server #
5:14.50

**Guardar el password de SQL en kubernetes**
```kubectl create secret generic mssql --from-literal=SA_PASSWORD="pa55w0rd!"```

**Aplicar el archivo kubberbetes para MSSQL**
```kubectl apply -f mssql-plat-depl.yaml```

Probar el SQL en kubernetes

* Abrir el SQL Management studio
* En el server name poner localhost, 1433
* En el user Login, poner SA
* En el password usar el password usardo en el secreet

En el archivo startup del proyecto de platformService 
usamos IWebHostEnvironment para saber en que ambiente estamos
y asi saber si estamos en develpment usamos in memory 
y si no usamos almacenamiento en SQL

### Migraciones Entity Framework  ###
Para crear una migracion debemos ejecutar el siguiente commando:

```dotnet ef migrations add initialmigration```

Para evitar el error de inMem en el archivo Startup.cs 
en el metodo ConfigureServices hay que comentar temporarlmente la condicion _env.isProduction
y todo el else asi solo quedara la seccion de SQL Server

```services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("PlatformConn")));```

tambien hay que comentar la linea 
```PrepDb.PrepPopulation(app, env.IsProduction());```

Ademas hay que copiar la cadenade conexion del Produccion a Development para que se ejecute la migracion a la base de datos aun estando en development.

ahora si podemos ejecutar la migracion.

```dotnet ef migrations add initialmigration```

>Nota en este paso tuve algunos problemas, para resolverlos tuve que instalar el dotnet entity framework
>dotnet tool install --global dotnet-ef --version 5.0.8

Una vez terminada la migracion volver a deployar el proyecto
el la carpeta del proyecto PlatformService 

1. docker build -t ahernandezcarrillo/platformservice .
2. docker push ahernandezcarrillo/platformservice

3. kubectl get deployments 
4. kubectl rollout restart deployment platforms-depl 
5. kubectl get pods

>Nota: en el video hubo un error en el password asi que se tivo que eliminar el deploy usando 
>el commando kubectl delete deployment platforms-depl
>y volviendo a repetir 

Volvemos a aplicar el deploy 

kubectl apply -f platforms-depl.yaml

### Agregar endopoints al proyecto de commands

Insertar imagen 004 aqui

1. Crear la capeta Models, crear los modelos de platforms y commands
2. Usar data annotations para definir los campos de la tabla
3. Crear la carpeta Data y crear la clase appDbContext.cs (Usar como ejemplo el mismo archivo de PlatformService)
4. Agregamos los DbSets usando los modelos antes creados. (estas seran las tablas que EF creara)
5. Agregamos las relaciones de las tablas sobre escribiendo el metodo OnModelCreating
```
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(p => p.Platform!)
                .HasForeignKey(p => p.PlatformId);
        }
```
Este código establece una relación entre Platform y Command donde una Platform puede tener muchos Commands, y cada Command tiene una clave foránea PlatformId que hace referencia a su Platform correspondiente.

**modelBuilder.Entity<Platform>():** Esto le dice a Entity Framework Core que estamos configurando la entidad Platform.

**.HasMany(p => p.Commands):** Esto define que la entidad Platform tiene una relación de "uno a muchos" con la entidad Command. En otras palabras, una Platform puede tener muchos Commands.

**.WithOne(p => p.Platform!):** Esto define la parte inversa de la relación, especificando que cada Command tiene una relación de "uno a uno" con una Platform. El signo de exclamación (!) es un operador de supresión de advertencias de nulabilidad que le dice al compilador que Platform no será null.

**.HasForeignKey(p => p.PlatformId):** Esto configura la clave foránea (foreign key) en la entidad Command que hace referencia a la entidad Platform. En este caso, PlatformId es la clave foránea en Command que se utiliza para vincular a la entidad Platform.


```
            modelBuilder.Entity<Command>()
                .HasOne(p => p.Platform)
                .WithMany(p => p.Commands)
                .HasForeignKey(p => p.PlatformId);
```
Este código establece la relación entre Command y Platform donde cada Command está asociado con una Platform, y una Platform puede tener muchos Commands. La clave foránea PlatformId en Command se utiliza para mantener esta relación.

### Implementar repositorio
En el folder Data crear la interfaz ICommandRepo.cs donde definimos las firmas de los metodos 
tanto para plataform como para command

```
        bool SaveChanges();

        // Platforms
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform plat);
        bool PlatformExists(int PlatformId);

        // Commands
        IEnumerable<Command> GetAllCommanfsForPlatform(int platformId);
        Command GetCommand (int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
```
Ahora debemos crear la clase CommandRepo.cs que implementa esta interfaz.
Implementamos los metodos de la interfaz y rellenamos estos metodos con el codigo para devolver la informacion requerida en cada caso

**Ejemplo:**
```
        public IEnumerable<Command> GetAllCommandsForPlatform(int platformId)
        {
            return _context.Commands
                           .Where(c => c.PlatformId == platformId)
                           .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
```  

### DTOs ###
Crear folder llamado DTOS
* Agregar DTOs PlatformReadDto, CommandReadDto y CommandCreateDto.

### Agregar Automapper 
* En el archivo startup.cs en el metodo ConfigureServices agregar 

``` services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); ```
6:42:00
### Agregar Profiles 
* Crear un folder llamado Profiles crear un archivo de profiles llamado CommandsProfiles.cs
Insertar los profiles a mapear
```
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
```

* Implemetar todos los los endpoints en los controladores de PlatformsController y CommnadsController

7:20:00

