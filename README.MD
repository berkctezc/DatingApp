# NET 5.0 & Angular Application Project

This is a project for learning purposes.

## 1 - Creating the .Net API

### dotnet-cli

[check my blog post to get some basic information on bash](https://berkctezc.medium.com/bash-komutlar%C4%B1-47a0811260e3)

```bash
# some of the most useful dotnet commands
dotnet -h # get help about common commands
dotnet command -h # help about specified command like parameters
dotnet --info # info about dotnet installations on the machine

dotnet new # create a new project
dotnet new -l # lists templates for new project
dotnet sln # modify vs solution file

dotnet run # runs the project in pwd
dotnet build # build the project in pwd
dotnet watch run # runs with a file watcher

dotnet dev-certs https --trust # required for your browser to trust api address
```

for our project we are going to run

```bash
dotnet new sln # gets its name from directory
dotnet new webapi -o API # creates a webapi project inside API folder
dotnet sln add API # adds project to the solution we created
```

### Required VS Code Extensions & Tips

* **F1:** command palette, **Ctrl+":** terminal 
* disable auto save, exclude `**/obj and **/bin` folders in user settings, disable compact folders, add code command to path
* Extensions: **Name:** [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) , [C# Extensions](https://marketplace.visualstudio.com/items?itemName=kreativ-software.csharpextensions) 
* from command palette type generate assets and click it. (appsettings.json and appsettings.Development.json files should be created after this operation) 

### API Project Files

```bash
cd API
dotnet run
```

* access in https://localhost:5001/Controller , 
* after running the project dotnet looks for Main method (in Program.cs) and the main method calls CreateHostBuilder. We could do some configuration in this method. CreateHostBuilder uses our Startup.cs file as startup. And we could also make configurations there.
* in appsettings.Development.json change `"Microsoft": "Warning"` to `"Microsoft": "Information"` to see more info data on terminal output

* Startup.cs
  * **ConfigureServices:** Dependency injection container. If we want to make a class or a service available to other areas of our application, we can add them inside ConfigureServices method.
  * **Configure:** Used to configure http request pipeline. Our requests goes through a series of middleware in this method.
    * `env.IsDevelopment > app.UseDeveloperExceptinPage()` : if we are in development mode, when application encounters a problem then we use the developer exception page.
    * `app.HttpsRedirection()`: Rediretcs HTTP to HTTPS
    * `app.UseRouting()`: Routing our actions to access them
    * `app.UseAuthorization()`: Authorization for our actions
    * `app.UseEndpoints(endpoints=>{endpoints.MapControllers();})`: Maps actions in controllers in controllers folder 
* **Properties/launchSettings.json**: configure launch options of project

