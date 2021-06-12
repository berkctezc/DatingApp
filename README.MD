# NET 5.0 & Angular App Project

This is a project for learning purposes named DatingApp.

## 1 - Creating the .Net API (BE)

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

- **F1:** command palette, **Ctrl+":** terminal
- disable auto save, exclude `**/obj and **/bin` folders in user settings, disable compact folders, add code command to path
- **Extensions:** [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) , [C# Extensions](https://marketplace.visualstudio.com/items?itemName=kreativ-software.csharpextensions) , [NuGet Gallery](https://marketplace.visualstudio.com/items?itemName=patcx.vscode-nuget-gallery) , [SQLite](https://marketplace.visualstudio.com/items?itemName=alexcvzz.vscode-sqlite) , [MSBuild project tools](https://marketplace.visualstudio.com/items?itemName=tintoy.msbuild-project-tools) , [vscode-solution-explorer](https://marketplace.visualstudio.com/items?itemName=fernandoescolar.vscode-solution-explorer)
- from command palette type generate assets and click it. (appsettings.json and appsettings.Development.json files should be created after this operation)
- troubleshooting debugging problems: downgrade c# extension, in settings, "omnisharp.useGlobalMono": "always", "omnisharp.path": "latest"

### API Project Files

```bash
cd API
dotnet run
```

- access in https://localhost:5001/Controller ,
- after running the project dotnet looks for Main method (in Program.cs) and the main method calls CreateHostBuilder. We could do some configuration in this method. CreateHostBuilder uses our Startup.cs file as startup. And we could also make configurations there.
- in appsettings.Development.json change `"Microsoft": "Warning"` to `"Microsoft": "Information"` to see more info data on terminal output

- Startup.cs
  - **ConfigureServices:** Dependency injection container. If we want to make a class or a service available to other areas of our application, we can add them inside ConfigureServices method. _(order does not matter)_
  - **Configure:** Used to configure http request pipeline. Our requests goes through a series of middleware in this method. _(order matters)_
    - `env.IsDevelopment > app.UseDeveloperExceptinPage()` : if we are in development mode, when application encounters a problem then we use the developer exception page.
    - `app.HttpsRedirection()`: Redirects HTTP to HTTPS
    - `app.UseRouting()`: Routing our actions to access them
    - `app.UseAuthorization()`: Authorization for our actions
    - `app.UseEndpoints(endpoints=>{endpoints.MapControllers();})`: Maps actions in controllers in controllers folder
- **Properties/launchSettings.json**: configure launch options of project

### Getting to work

- Create **Entities** folder and create an Entity

- **Entity Framework:** An Object Relational Mapper that translates our code into SQL and update our tables in the database. EF features are Querying, Change Tracking, Saving, Concurrency, Caching, Built-in conventions, Configurations, Migrations

- Use NuGet Gallery to install **Microsoft.EntityFrameworkCore** (F1: >Open NuGet Gallery)

- Create **Data** folder and inside Data Create Context file that implements **DbContext**

- add to services in **ConfigureServices** >

  ```cs
  services.AddDbContext<DataContext>(options=>{   options.UseSqlite("Connection String");});
  ```

- add connection string to appsettings.Development.json

  ```cs
     "ConnectionStrings": {
        "DefaultConnection":"Data source=datingapp.db"
     },
  ```

- use it in Startup.cs

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

- **install dotnet-ef in cli **=>

  ```bash
  dotnet tool install --global dotnet-ef # you can specify a version by --version
  ```

- install **EntityFrameworkCore.Design** to API project with NuGet gallery

- Create a migration

  ```bash
  dotnet ef migrations add InitialCreate -o Data/Migrations # -o specifies directory
  ```

- Update database

```bash
 dotnet ef database update
```

- Use sqlite extension to take a look at the sqlite db with F1: SQLite open database
- right click and click new query insert to add some data to db

```sqlite
INSERT INTO AppUsers (Id, UserName) VALUES (1,"Bob");
```

### Adding an API Controller

- in Controller folder create your controller like **UsersController.cs**
- inherit it from ControllerBase and add **[ApiController]** and **[Route("api/[controller]")]** annotations
- Generate Constructor and initialize DataContext field from parameter

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

- syntax: `async Task<YOURRETURNTYPE> YourAction` and return await ....ToListAsync(),FindAsync....

- example:

  ```cs
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
  {
      return await _context.AppUsers.ToListAsync();
  }
  ```

## 2 - Creating the Angular Client (FE)

### angular-cli

- install with `npm install -g @angular/cli`

```bash
######################################################
#### some of the most useful angular cli commands ####
######################################################
ng new <APPNAME> # create an angular app
ng serve # compiles and serves the app on machine
ng add <npp_packages> # installs npm package and configures the project

ng g <schematics> # generate a schematics 
ng g -h # get a list of available schematics
# common ones
ng g c # component
ng g s # service 
ng g p # pipe
```

### Required VS Code Extensions for Angular Development

[Angular Language Service](https://marketplace.visualstudio.com/items?itemName=Angular.ng-template) , [Angular Snippets](https://marketplace.visualstudio.com/items?itemName=johnpapa.Angular2) , [Bracket Pair Colorizer 2](https://marketplace.visualstudio.com/items?itemName=CoenraadS.bracket-pair-colorizer-2)

### HTTP Requests in Angular (FE+BE)

- declare HttpClientModule import in **app.module.ts**

```typescript
import { HttpClientModule } from '@angular/common/http'; //import httpclientmodule
//.....
@NgModule({
    //...
      imports: [
	//...
    HttpClientModule //add to imports
  ],
})
```

- inject in constructor of **app.component.ts** and call in a function

```ts
export class AppComponent implements OnInit {
  // implemented OnInit
  title = "The Dating App";
  users: any; // created a variable to hold the request response

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.getUsers();
  }

  getUsers() {
    this.http
      .get("https://localhost:5001/api/users") // make request
      .subscribe(
        // observe
        (response) => {
          this.users = response;
        }, // paramaters for return situations of request
        (error) => {
          console.log(error);
        }
      );
  }
}
```

- Allow CORS in Backend **Startup.cs > ConfigureServices and Configure**

```cs
public void ConfigureServices(IServiceCollection services)
{
    //...
    services.AddCors();
}
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    //..
   // After UseRouting()
    app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200")); // angular app
    //..
}
```

- Display in **app.component.html**

```html
<ul>
  <li *ngFor="let user of users">{{ user.id }} - {{ user.userName }}</li>
</ul>
```

### Adding bootstrap and font-awesome

```bash
ng add ngx-bootstrap # install with configurations

# or

npm install ngx-bootstrap
npm install bootstrap
# and add into angular.json styles array,
# add into imports in app.module.ts
```

- angular.json

```json
"styles": [
    "src/styles.css",
    "./node_modules/bootstrap/dist/css/bootstrap.min.css",
    "./node_modules/ngx-bootstrap/datepicker/bs-datepicker.css"
],
```

- app.module.ts

```ts
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
//...
  imports: [
//...
    BrowserAnimationsModule
  ],
```

### HTTPS in Angular

- generate ssl keys with openssl

  ```bash
  openssl req -newkey rsa:2048 -nodes -keyout server.key -x509 -days 365 -out server.crt
  ```

- put crt and key files into folder named ssl inside angular client folder.

- add to angular.json serve>options

  ```json
  "options": {
      "sslCert": "./ssl/server.crt",
      "sslKey": "./ssl/server.key",
      "ssl": true
  },
  ```

## 3 - Authentication Basics (BE)

### Account Controller with a Register Endpoint

```cs
public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    public AccountController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AppUser>> Register(string username, string password)
    {
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            PasswordSalt = hmac.Key
        };

        _context.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
```

- current controller only accepts requests with query string and not in body. because it cannot map parameters

### Using Debugger

- After generating assets, set run and debug to attach and add a breakpoint
- press F5 and select API to debug the app

### DTOs

- Created a DTO for registration with only the data we need for registration

```cs
public class RegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

- In **AccountController > Register**

```cs
[HttpPost("register")]
public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
{
    if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

    using var hmac = new HMACSHA512();

    var user = new AppUser
    {
        UserName = registerDto.Username.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        PasswordSalt = hmac.Key
    };

    _context.Add(user);
    await _context.SaveChangesAsync();

    return user;
}
```

- Also lets add a userexists method to check if username is taken or not

```cs
private async Task<bool> UserExists(string username)
{
    return await _context.AppUsers.AnyAsync(x => x.UserName == username.ToLower());
}
```

### Adding Validation

- It is a good starting point to add validation in our DTOs

```cs
[Required]
public string Username { get; set; }
[Required]
public string Password { get; set; }
```

### Adding a login endpoint

```cs
        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            // check if this user exists. if we have user with that username then assign it to user, if not assign null to user
            var user = await _context.AppUsers
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            // if null return 401
            if (user == null) return Unauthorized("Invalid Username");

            // if user is valid then calculate salt and hash to compare with db salt and hash
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return user;
        }
```

### Adding a Token Service - JWT

- Add ITokenService interface into folder name Interfaces and implement it in services folder

  ```cs
  namespace API.Interfaces
  {
      public interface ITokenService
      {
          string CreateToken(AppUser user);
      }
  }
  ```

  ```cs
  namespace API.Services
  {
      public class TokenService : ITokenService
      {
          public string CreateToken(AppUser user)
          {
              throw new System.NotImplementedException();
          }
      }
  }
  ```

  - Add in **Startup>ConfigureServices**

```cs
   services.AddScoped<ITokenService, TokenService>();
```

### Adding Create Token Logic

- Install NuGet package System.IdentityModel.Tokens.Jwt
- Implement logic

```cs
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])); // from appsettings
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
```

### Creating a User DTO and returning the token

- Inject Token Service in Controller

```cs
private readonly ITokenService _tokenService;
public AccountController(DataContext context, ITokenService tokenService)
{
    _tokenService = tokenService;
    _context = context;
}
```

- Create a UserDto for returning after registrations and login operations

```cs
public class UserDto
{
    public string Username { get; set; }
    public string Token { get; set; }
}
```

- Update Register and Login Actions' return type

```cs
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    //....
    return new UserDto
    {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user)
    };
}

public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
{
    //....
    return new UserDto
    {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user)
    };
}
```

- In appsettings.Development.json assign a key into TokenKey

```json
//..
"TokenKey":"super secret unguessable key",
//..
```

### Adding authentication middleware

- Install NuGet package **Microsoft.AspNetCore.Authentication.JwtBearer**

- In Startup add to **Configure** and **ConfigureServices**,

```cs
        public void ConfigureServices(IServiceCollection services)
        {
            //...
 services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        }
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
      //...
      // After Cors
app.UseAuthentication();
      // Before Authorization
      //...
}
```

### Adding extension methods

- Move **Application Services** from **Startup** into **ApplicationServiceExtensions** extension in **Extensions** folder and reference it in **Startup**

```cs
namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
```

- Move **Identity Services** from **Startup** into **IdentityServiceExtensions** extension in **Extensions** folder and reference it in **Startup**

```cs
namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            return services;
        }
    }
}
```

- Usage of extensions in **Startup.cs**

```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddApplicationServices(_config);
    //...
    services.AddIdentityServices(_config);
}
```

## 4 - Client Login and Register

### Creating a navbar

* Create navbar with angular cli

```bash
cd src/app
ng g c nav --skip-tests
```

* After executing these commands NavComponent is added into declarations and imported. Also empty files created in a folder.
* Fill html and add into app.component.html with `<app-nav>` tag.

### Angular Template Forms

* import **FormsModule** in **app.module.ts**
* Getting data from template with Two way binding. create a empty model in **nav.component.ts** and a login function to test it.

```typescript
export class NavComponent implements OnInit {
  model: any = {}
  constructor() {}
  ngOnInit(): void {}
  login() {
    console.log(this.model);
  }
}
```

- Set up the form. 

- (**ngSubmit)** executes the function given after clicking button that has submit as its type. 

- **#loginForm="ngForm"** marks form as an angular form. 

- **[(ngModel)]** binds the data to model we created in **nav.component.ts** (banana notation).

  ```html
      <form
        #loginForm="ngForm"
        (ngSubmit)="login()"
        class="d-flex"
        autocomplete="off"
      >
        <input
          name="username"
          [(ngModel)]="model.username"
          class="form-control me-2"
          type="text"
          placeholder="Username"
        />
        <input
          name="password"
          [(ngModel)]="model.password"
          class="form-control me-2"
          type="password"
          placeholder="Password"
        />
        <button class="btn btn-outline-success" type="submit">Login</button>
      </form>
  ```

  ### Angular Services

- Create a services for for keeping things organized

```bash
cd src/app
mkdir _services
```

* Generate the service with angular-cli

```bash
cd _services
ng g s account --skip-tests
```

* Create a function for login operation

```bash
export class AccountService {
  baseUrl = 'https://localhost:5001/api';
  constructor(private http: HttpClient) { }
  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model);
  }
}
```

### Injecting services into components

* Inject service in constructor of component like this

```typescript
export class NavComponent implements OnInit {
//...  
constructor(private accountService: AccountService) { }
//..
}
```

- Implement login function

```typescript
  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
      this.loggedIn = true;
    },error => {console.error(error);})}
```

### Adding and removing content using conditionals

* Disable the options for **loggedIn==false**
* Show the options for **loggedIn==true**
* Disable the login form and change it with welcome text for **loggedIn==true**

```html
<nav class="navbar navbar-expand-md navbar-dark fixed-top bg-dark">
  <div class="container container-fluid">
    <a class="navbar-brand" href="#">Dating App</a>
    <ul *ngIf="loggedIn" class="navbar-nav me-auto mb-2 mb-md-0">
      <li class="nav-item"><a class="nav-link" aria-current="page" href="#">Matches</a></li>
      <li class="nav-item"><a class="nav-link" href="#">Lists</a></li>
      <li class="nav-item"><a class="nav-link" href="#">Messages</a></li>
      <li class="nav-item"><a class="nav-link" (click)="logout()" href="#">Logout</a></li>
    </ul>
    <div class="dropdown" *ngIf="loggedIn">
      <a class="dropdown-toggle text-light">Welcome {{ model.username }}</a>
      <div class="dropdown-menu">
        <a class="dropdown-item">Edit Profile</a><a class="dropdown-item (click)=logout()">Logout</a>
      </div>
    </div>
    <form *ngIf="!loggedIn" #loginForm="ngForm" (ngSubmit)="login()" class="d-flex" autocomplete="off">
      <input name="username" [(ngModel)]="model.username" class="form-control me-2" type="text" placeholder="Username"/>
      <input name="password" [(ngModel)]="model.password" class="form-control me-2" type="password" placeholder="Password"/>
      <button class="btn btn-outline-success" type="submit">Login</button>
    </form>
  </div>
</nav>
```

### Using ngx-bootstrap components - dropdown

* In **app.module.ts**

```ts
import { BsDropdownModule } from "ngx-bootstrap/dropdown";
//..
  imports: [//..
    BsDropdownModule.forRoot(),
  ],
```

* In **nav.component.html** use dropdown, dropdownToggle, *dropdownMenu. for more info check ngx-bootstrap docs

```html
    <div class="dropdown" *ngIf="loggedIn" dropdown>
      <a class="dropdown-toggle text-light" dropdownToggle>Welcome {{ model.username }}</a>
      <div class="dropdown-menu" *dropdownMenu>
        <a class="dropdown-item">Edit Profile</a>
        <div class="dropdown-divider"></div>
        <a class="dropdown-item" (click)="logout()">Logout</a>
      </div>
    </div>
```

