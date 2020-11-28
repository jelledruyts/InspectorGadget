# Inspector Gadget

## About

Inspector Gadget is a simple web app that you can use to get a lot of info on the web server where it's running (the *inspector*) and perform additional tasks (the *gadgets*) from there.

## Deployment

There are two versions that you can choose from:

- A single [default.aspx](Page/default.aspx) page that you can simply drop in any Windows and ASP.NET Framework based host (like IIS or Azure App Service).
  - You can just copy this file into e.g. the `wwwroot` directory of your site (optionally rename it) and navigate to it directly without any compilation required.
- A more elaborate .NET Core based [web app](WebApp) that can run on Windows or Linux.
  - You can compile the source code and publish the app to your host of choice directly, or build and run it as a container.
  - You can also find the latest published version of the Docker container publicly on Docker Hub at [jelledruyts/inspectorgadget](https://hub.docker.com/r/jelledruyts/inspectorgadget).
    - For Linux: specify the image name and tag as `jelledruyts/inspectorgadget:latest` or simply `jelledruyts/inspectorgadget`.
    - For Windows: specify the image name and tag as `jelledruyts/inspectorgadget:latest-windows`.

## Configuration

The app can be configured with the configuration settings below. Note that the single page version does not support all gadgets that the full version has, so the configuration settings for the missing gadgets don't apply.

| Setting                                       | Purpose                                                                                                                         |
|-----------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------|
| `BackgroundColor`                             | An HTML/CSS color value to set as the background color for all pages, e.g. to easily distinguish multiple instances of this app |
| `InfoMessage`                                 | An informational message to put at the top of all pages, e.g. to easily distinguish multiple instances of this app              |
| `DefaultCallChainUrls`                        | The default value for the Call Chain URL's (applies to all gadgets)                                                             |
| `DisableAzureManagedIdentity`                 | Allows you to disable the **Azure Managed Identity** gadget                                                                     |
| `DefaultAzureManagedIdentityResource`         | The default value for the Azure Managed Identity gadget resource                                                                |
| `DisableDnsLookup`                            | Allows you to disable the **DNS Lookup** gadget                                                                                 |
| `DefaultDnsLookupHost`                        | The default value for the DNS Lookup gadget host                                                                                |
| `DisableHttpRequest`                          | Allows you to disable the **HTTP Request** gadget                                                                               |
| `DefaultHttpRequestUrl`                       | The default value for the HTTP Request gadget URL                                                                               |
| `DefaultHttpRequestHostName`                  | The default value for the HTTP Request gadget host name                                                                         |
| `DisableIntrospector`                         | Allows you to disable the **Introspector** gadget                                                                               |
| `DefaultIntrospectorGroup`                    | The default value for the Introspector gadget group                                                                             |
| `DefaultIntrospectorKey`                      | The default value for the Introspector gadget key                                                                               |
| `DisableProcessRun`                           | Allows you to disable the **Process Run** gadget                                                                                |
| `DefaultProcessRunFileName`                   | The default value for the Process Run gadget file name                                                                          |
| `DefaultProcessRunArguments`                  | The default value for the Process Run gadget arguments                                                                          |
| `DefaultProcessRunTimeoutSeconds`             | The default value for the Process Run gadget timeout (in seconds)                                                               |
| `DisableSocketConnection`                     | Allows you to disable the **Socket Connection** gadget                                                                          |
| `DefaultSocketConnectionRequestHostName`      | The default value for the Socket Connection gadget request host name                                                            |
| `DefaultSocketConnectionRequestPort`          | The default value for the Socket Connection gadget request port                                                                 |
| `DefaultSocketConnectionRequestBody`          | The default value for the Socket Connection gadget request body                                                                 |
| `DefaultSocketConnectionReadResponse`         | The default value for the Socket Connection gadget setting to read the response                                                 |
| `DisableSqlConnection`                        | Allows you to disable the **SQL Connection** gadget                                                                             |
| `DefaultSqlConnectionSqlConnectionString`     | The default value for the SQL Connection gadget SQL connection string                                                           |
| `DefaultSqlConnectionSqlQuery`                | The default value for the SQL Connection gadget SQL query                                                                       |
| `DefaultSqlConnectionUseAzureManagedIdentity` | The default value for the SQL Connection gadget setting to use the Azure Managed Identity of the app                            |

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
