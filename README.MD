# 利用.NET 6.0 开发Microsoft 365应用 - 网站应用
> 作者：陈希章 2022-4-20

## 概述

这个范例代码是用来展示如何利用.NET 6.0 快速开发 Microsoft 365应用，我们将展示一个网站应用，用来访问Microsoft Graph，并显示出来当前用户信息和最近十封邮件。

## 如何使用

有多种方式使用这个范例代码，例如

1. 你可以将其克隆到本地，然后通过 `dotnet run --urls=http://localhost:5023` 命令运行
1. 你可以直接在线访问这个应用 https://hellodotnetweb.azurewebsites.net
1. 你还可以通过docker运行这个应用，`docker run --rm -it chenxizhang/dotnet-graph-web -p 5023:80`

## 如何实现

这是一个标准的网页应用程序，请按如下的介绍进行练习

1. 通过 `dotnet new webapp -au MultiOrg --client-id fc341661-31f0-4a4f-8f18-0afb8a56a98a --calls-graph true --called-api-scopes "user.read mail.read"` 创建项目, 这里的 `client-id` 需要填入你自己注册的编号
1. 修改 `appsettings.json` 文件，这里需要填入你的密钥（ClientSecret）

1. 修改 `Pages/Index.cshtml.cs` 文件

    ```csharp
    // 注入GraphServiceClient

    private readonly GraphServiceClient _client;

    public IndexModel(ILogger<IndexModel> logger,
                        GraphServiceClient client,
                        IDownstreamWebApi downstreamWebApi)
    {
        _logger = logger;
        _downstreamWebApi = downstreamWebApi;
        _client = client;

    }


    // 修改OnGet方法
    public string[]? Messages { get; set; }
    public User? Me { get; set; }
    public async Task OnGet()
    {
        Me = await _client.Me.Request().GetAsync();
        var messages = await _client.Me.Messages.Request().GetAsync();
        Messages = messages.Select(x => x.Subject).ToArray();
    }

    ```
1. 修改 `Pages/Index.cshtml` 文件

    ```html
    @page
    @model IndexModel
    @{
        ViewData["Title"] = "利用.NET 6.0 开发Microsoft 365应用";
    }


    <div>基本信息</div>

    @if (Model.Me != null)
    {
        <div>
            显示名称： @Model.Me.DisplayName ， 邮件地址：@Model.Me.UserPrincipalName
        </div>

    }

    <br />


    <div>邮件信息</div>
    <ol>
        @if (Model.Messages != null)
        {
            @foreach (var item in Model.Messages)
            {
                <li><b>@item</b></li>
            }
        }

    </ol>

    ```

1. 如果需要打包为Docker镜像，请参考下面的定义 Dockerfile

    ```docker
    # https://hub.docker.com/_/microsoft-dotnet
    FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
    WORKDIR /source

    # 复制文件
    COPY *.csproj .
    RUN dotnet restore

    # 编译
    COPY . .
    WORKDIR /source
    RUN dotnet publish -c release -o /app --no-restore

    # 最后处理
    FROM mcr.microsoft.com/dotnet/aspnet:6.0
    WORKDIR /app
    COPY --from=build /app ./
    EXPOSE 80
    EXPOSE 443
    # 下面这一句特别关键，否则会出现错误
    ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
    ENTRYPOINT ["dotnet", "graphweb.dll"]
    ```

## 更多参考

更多关于Microsoft Graph 的开发，可以参考 

1. 官方文档 https://docs.microsoft.com/zh-cn/graph/overview 
1. 《三十天学会Microsoft Graph》 https://aka.ms/30DaysMSGraph
1. 《解密和实战Microsoft Identity Platform》 https://identityplatform.xizhang.com 