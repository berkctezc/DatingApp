# NET 5.0 & Angular App Project

This is a project for learning purposes named DatingApp.

## 1 - Creating the .Net API

### dotnet-cli

[check my blog post to get some basic information on bash](https://berkctezc.medium.com/bash-komutlar%C4%B1-47a0811260e3)

```bash
#####################################################
#### some of the most useful dotnet cli commands ####
#####################################################
dotnet -h # get help about common commands
dotnet command -h # help about specified command like parameters
dotnet --info # info about dotnet installations on the machine

dotnet new # create a new project
dotnet new -l # lists templates for new project
dotnet sln # modify vs solution file
ß
dotnet run # runs the project in pwd
dotnet build # build the project in pwd
dotnet watch run # runs with a file watcher

dotnet dev-certs https --trust # required for your browser to trust api address

dotnet tool install <TOOLNAME> # installs other cli extensions, tools

###############
## dotnet ef ##
###############
dotnet ef migrations add <MIGNAME> # creates a migration you can use -o to specify output directory
dotnet ef database update # updates db content using migrations
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
* **Extensions:** [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) , [C# Extensions](https://marketplace.visualstudio.com/items?itemName=kreativ-software.csharpextensions) , [NuGet Gallery](https://marketplace.visualstudio.com/items?itemName=patcx.vscode-nuget-gallery) , [SQLite](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite) , [MSBuild project tools]( https://marketplace.visualstudio.com/items?itemName=tintoy.msbuild-project-tools) , [vscode-solution-explorer](https://marketplace.visualstudio.com/items?itemName=fernandoescolar.vscode-solution-explorer)
* from command palette type generate assets and click it. (appsettings.json and appsettings.Development.json files should be created after this operation) 
* troubleshooting debugging problems: downgrade c# extension, in settings,   "omnisharp.useGlobalMono": "always", "omnisharp.path": "latest"

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
    * `app.HttpsRedirection()`: Redirects HTTP to HTTPS
    * `app.UseRouting()`: Routing our actions to access them
    * `app.UseAuthorization()`: Authorization for our actions
    * `app.UseEndpoints(endpoints=>{endpoints.MapControllers();})`: Maps actions  in controllers in controllers folder 
* **Properties/launchSettings.json**: configure launch options of project

### Getting to work

* Create Entities folder and create an Entity

* **Entity Framework:** An Object Relational Mapper that translates our code into SQL and update our tables in the database. EF features are Querying, Change Tracking, Saving, Concurrency, Caching, Built-in conventions, Configurations, Migrations

* Use NuGet Gallery to install Microsoft.EntityFrameworkCore (F1: >Open NuGet Gallery)

* Create Data folder and inside Data Create Context file that implements DbContext

* add to services in ConfigureServices >      

  ```cs
  services.AddDbContext<DataContext>(options=>{   options.UseSqlite("Connection String");});
  ```

* add connection string to appsettings.Development.json

    ```cs
       "ConnectionStrings": {
          "DefaultConnection":"Data source=datingapp.db"
       }, 
    ```

* use it in Startup.cs

```cs
// initialize _config
      private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
// ...
// ...
public void ConfigureServices(IServiceCollection services){
            services.AddDbContext<DataContext>(options =>
            {                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });
// .....
```

* **install dotnet-ef  in cli **=> 

  ```bash
  dotnet tool install --global dotnet-ef # you can specify a version by --version
  ```

* install **EntityFrameworkCore.Design** to API project with NuGet gallery

* Create a migration

  ```bash
  dotnet ef migrations add InitialCreate -o Data/Migrations # -o specifies directory
  ```


* Update database

 ```bash
  dotnet ef database update
 ```

* Use sqlite extension to take a look at the sqlite db with F1: SQLite open database
* right click and click new query insert to add some data to db

```sqlite
INSERT INTO AppUsers (Id, UserName) VALUES (1,"Bob");
```

### Adding an API Controller

* in Controller folder create your controller like **UsersController.cs**
* inherit it from ControllerBase and add **[ApiController]** and **[Route("api/[controller]")]** annotations
* Generate Constructor and initialize DataContext field from parameter

some action examples

```cs
[HttpGet]
public ActionResult<IEnumerable<AppUser>> GetUsers()
{
    return _context.AppUsers.ToList();
}

// api/users/1
[HttpGet("{id}")]
public ActionResult<AppUser> GetUsers(int id)
{
    return _context.AppUsers.Find(id);
} 
```

### Making our code Async

* syntax: `async Task<YOURRETURNTYPE> YourAction` and return await ....ToListAsync(),FindAsync....

* example:

  ```cs
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
  {
      return await _context.AppUsers.ToListAsync();
  }
  ```

## 2 - Creating the Angular Client

### angular-cli

* install with `npm install -g @angular/cli`

```bash
######################################################
#### some of the most useful angular cli commands ####
######################################################
ng new <APPNAME> # create an angular app
ng serve # compiles and serves the app on machine
```

### Required VS Code Extensions for Angular Development

[Angular Language Service](https://marketplace.visualstudio.com/items?itemName=Angular.ng-template) , [Angular Snippets](https://marketplace.visualstudio.com/items?itemName=johnpapa.Angular2) , [Bracket Pair Colorizer 2](https://marketplace.visualstudio.com/items?itemName=CoenraadS.bracket-pair-colorizer-2)
