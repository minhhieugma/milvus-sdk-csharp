# milvus-sdk-csharp

<div class="column" align="middle">
  <img src="https://img.shields.io/github/workflow/status/weianweigan/milvus-sdk-csharp/Build"/>
  <img src="https://img.shields.io/nuget/v/io.milvus"/>
  <img src="https://img.shields.io/nuget/dt/io.milvus"/>
</div>

<div align="middle">
    <img src="milvussharp.png"/>
</div>

C# SDK for [Milvus](https://github.com/milvus-io/milvus).

## Getting Started

**Visual Studio**

Visual Studio 2019  or higher

**NuGet**

IO.Milvus is delivered via NuGet package manager. You can find the package here:
https://www.nuget.org/packages/IO.Milvus/

### Prerequisites

**Framework**

 [![NET6](https://img.shields.io/badge/.NET-6.0-red)](https://github.com/lepoco/wpfui/blob/main/src/Wpf.Ui/WPFUI.csproj) 
 ![NET5](https://img.shields.io/badge/.NET-5.0-blue)

```
.net 5.0 or .net 6.0
```

### Installing

```
Install-Package IO.Milvus -Version 2.0.0-alpha.2
```
Or

```xml
<PackageReference Include="IO.Milvus" Version="2.0.0-alpha.2" />
```


### Examples

Connect to a Milvus server.

```csharp
var milvusClient = new MilvusServiceClient(
    ConnectParam.Create(
    host: "192.168.100.139",
    port: 19531));
```
Disconnect from a Milvus server.

```csharp
milvusClient.Close();
```

Please refer to [Test Project](https://github.com/weianweigan/milvus-sdk-csharp/tree/develop/src/IO.MilvusTests) for more examples.

### Grpc client

You can find code that auto generated by grpc tools in namespace of IO.Milvus.Grpc,then use auto-generated serviceclient to connect server and send request.
```csharp
var defualtClient = MilvusServiceClient.CreateGrpcDefaultClient(
    ConnectParam.Create(
        host: "192.168.100.139",
        port: 19531));
```
### Asp.net core

If you want to use io.milvus in asp.net core, you can take a look at this [doc](https://docs.microsoft.com/zh-cn/aspnet/core/grpc/?view=aspnetcore-6.0).

Use namespace of IO.Milvus.Grpc with [DI](https://docs.microsoft.com/zh-cn/aspnet/core/grpc/clientfactory?view=aspnetcore-6.0).