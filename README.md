# Inspector Gadget

## About

Inspector Gadget is a simple web app that you can use to get a lot of info on the web server where it's running (the *inspector*) and perform additional tasks (the *gadgets*) from there.

## Usage

There are two versions that you can choose from:

- A single [default.aspx](Page/default.aspx) page that you can simply drop in any Windows and ASP.NET Framework based host (like IIS or Azure App Service).
  - You can just copy this file into e.g. the `wwwroot` directory of your site (optionally rename it) and navigate to it directly without any compilation required.
- A more elaborate .NET Core based [web app](WebApp) on Windows or Linux.
  - You can find the latest published version of the Docker container on Docker Hub at [jelledruyts/inspectorgadget](https://hub.docker.com/r/jelledruyts/inspectorgadget).
  - You can also compile the source code and publish the app to your host of choice directly, or build and run it as a container.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
