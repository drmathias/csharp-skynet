⚠️ _This repository is not maintained_ ⚠️

---

# Skynet C# SDK

A client for interacting with a Sia Skynet webportal.

![.NET Core](https://github.com/drmathias/csharp-skynet/workflows/.NET%20Core/badge.svg?branch=master) [![NuGet](https://img.shields.io/nuget/v/Sia.Skynet)](https://www.nuget.org/packages/Sia.Skynet/) ![MIT License](https://img.shields.io/github/license/drmathias/csharp-skynet)

## Usage

### Setup

This library targets [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard#net-implementation-support), so can be used across various platforms. The webportal client is a [typed client](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients), that uses `System.Net.Http.HttpClient` to make HTTP requests.

```csharp
using var httpClient = new HttpClient { BaseAddress = new Uri("https://siasky.net") };
var skynetWebPortal = new SkynetWebPortal(httpClient);
```

#### With Dependency Injection

When using `Microsoft.Extensions.DependencyInjection`, the client can easily be configured and set up to work with any Skynet webportal.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient<ISkynetWebPortal, SkynetWebPortal>(client =>
    {
        client.BaseAddress = new Uri("https://siasky.net");
    });
}
```

### Downloading Files

Files can be downloaded from a Sia Skynet webportal, by providing a Skylink and optionally a path.

```csharp
try
{
    var skylink = Skylink.Parse("AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA");
    HttpContent response = await skynetWebPortal.DownloadFile(skylink);
}
catch(HttpException e)
{
    // unsuccessful (non-2XX) response
}
```

### Uploading Files

This library uses `Microsoft.Extensions.FileProviders.Abstractions` (see [FileProviders](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers)) which allows you to upload from many file providers. There are several methods available to simplify uploading single files, multiple files and directories.

#### Upload a File

```csharp
try
{
    var file = new PhysicalFileInfo(new FileInfo("path/to/file.json"));
    Skylink response = await skynetWebPortal.UploadFile(file);
}
catch(HttpException e)
{
    // unsuccessful (non-2XX) response
}
catch(HttpResponseException e)
{
    // invalid response from webportal
}
catch(IOException e)
{
    // file access errors
}
```

#### Upload a Directory

```csharp
try
{
    var fileProvider = new PhysicalFileProvider("");
    Skylink response = await skynetWebPortal.UploadDirectory(fileProvider, "directory/to/upload", recurse: true);
}
catch(DirectoryNotFoundException e)
{
    // invalid directory
}
catch(HttpException e)
{
    // unsuccessful (non-2XX) response
}
catch(HttpResponseException e)
{
    // invalid response from webportal
}
catch(IOException e)
{
    // file access errors
}
```

#### File Configuration

By default, the Skynet path of an uploaded file is set to the file name. This behaviour can be changed, by specifying the Skynet path on an individual `UploadItem`.

```csharp
new UploadItem(file, "/images/sunset.jpg");
// file will become available at https://siasky.net/{skylink}/images/sunset.jpg
```

For file uploads, the MIME type is automatically resolved based on the file extension. If you want to override this behaviour, you can explicitly specify the MIME type on an individual `UploadItem`.

```csharp
new UploadItem(file, null, MediaTypeHeaderValue.Parse("image/gif"));
// when downloaded, the Content-Type header will be set to image/gif
```

#### Portal Configuration

`UploadOptions` and `MultiFileUploadOptions` can be used to configure how Skynet webportals handle requests for the upload.

```csharp
var options = new MultiFileUploadOptions
{
    FileName = "tag:siasky.net,2020-10-10:AABFphGLnADQbFx3tXOQdtjKf0MvFzqZoDIqj_VaebkqcA",
    DefaultPath = "wwwroot/index.html"
    DryRun = true
};
Skylink response = await skynetWebPortal.UploadDirectory(fileProvider, "directory/to/upload", recurse: true, options);
```
