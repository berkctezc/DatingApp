# NET 5.0 & Angular Project

This is a project for learning purposes named DatingApp.

[TOC]

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

for our project we are going to use

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
- after running the project, dotnet looks for Main method (in Program.cs) and the main method calls CreateHostBuilder. We could do some configuration in this method. CreateHostBuilder uses our Startup.cs file as startup. And we could also make configurations there.
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
services.AddDbContext<DataContext>(options=>{options.UseSqlite("Connection String");});
```

- add connection string to appsettings.Development.json

```cs
"ConnectionStrings": {"DefaultConnection":"Data source=datingapp.db"},
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
public void ConfigureServices(IServiceCollection services){
            services.AddDbContext<DataContext>(options =>
            {                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });
// .....
```

- **install dotnet-ef in cli **

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
INSERT INTO Users (Id, UserName) VALUES (1,"Bob");
```

### API Controller

- in Controller folder create your controller like **UsersController.cs**
- inherit it from ControllerBase and add **[ApiController]** and **[Route("api/[controller]")]** annotations
- Generate Constructor and initialize DataContext field from parameter

some action examples

```cs
[HttpGet]
public ActionResult<IEnumerable<AppUser>> GetUsers(){ return _context.Users.ToList(); }

// api/users/1
[HttpGet("{id}")]
public ActionResult<AppUser> GetUsers(int id){ return _context.Users.Find(id); }
```

### Making our code Async

- syntax: `async Task<YOURRETURNTYPE> YourAction` and return await ....ToListAsync(),FindAsync....

- example:

  ```cs
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){ return await _context.Users.ToListAsync(); }
  ```

## 2 - Creating the Angular Client (FE)

### angular-cli

- install with `npm i -g @angular/cli`

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
npm i ngx-bootstrap
npm i bootstrap
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
    return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
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
    var user = await _context.Users
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

## 4 - Client Login and Register (FE)

### Creating a navbar

- Create navbar with angular cli

```bash
cd src/app
ng g c nav --skip-tests
```

- After executing these commands NavComponent is added into declarations and imported. Also empty files created in a folder.
- Fill html and add into app.component.html with `<app-nav>` tag.

### Angular Template Forms

- import **FormsModule** in **app.module.ts**
- Getting data from template with Two way binding. create a empty model in **nav.component.ts** and a login function to test it.

```typescript
export class NavComponent implements OnInit {
  model: any = {};
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

- Generate the service with angular-cli

```bash
cd _services
ng g s account --skip-tests
```

- Create a function for login operation

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

- Inject service in constructor of component like this

```typescript
export class NavComponent implements OnInit {
  //...
  constructor(private accountService: AccountService) {}
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

- Disable the options for **loggedIn==false**
- Show the options for **loggedIn==true**
- Disable the login form and change it with welcome text for **loggedIn==true**

```html
<nav class="navbar navbar-expand-md navbar-dark fixed-top bg-dark">
  <div class="container container-fluid">
    <a class="navbar-brand" href="#">Dating App</a>
    <ul *ngIf="loggedIn" class="navbar-nav me-auto mb-2 mb-md-0">
      <li class="nav-item">
        <a class="nav-link" aria-current="page" href="#">Matches</a>
      </li>
      <li class="nav-item"><a class="nav-link" href="#">Lists</a></li>
      <li class="nav-item"><a class="nav-link" href="#">Messages</a></li>
      <li class="nav-item">
        <a class="nav-link" (click)="logout()" href="#">Logout</a>
      </li>
    </ul>
    <div class="dropdown" *ngIf="loggedIn">
      <a class="dropdown-toggle text-light">Welcome {{ model.username }}</a>
      <div class="dropdown-menu">
        <a class="dropdown-item">Edit Profile</a
        ><a class="dropdown-item (click)=logout()">Logout</a>
      </div>
    </div>
    <form
      *ngIf="!loggedIn"
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
  </div>
</nav>
```

### Using ngx-bootstrap components - dropdown

- In **app.module.ts**

```ts
import { BsDropdownModule } from "ngx-bootstrap/dropdown";
//..
  imports: [//..
    BsDropdownModule.forRoot(),
  ],
```

- In **nav.component.html** use **dropdown, dropdownToggle, \*dropdownMenu**. for more info check ngx-bootstrap docs

```html
<div class="dropdown" *ngIf="loggedIn" dropdown>
  <a class="dropdown-toggle text-light" dropdownToggle
    >Welcome {{ model.username }}</a
  >
  <div class="dropdown-menu" *dropdownMenu>
    <a class="dropdown-item">Edit Profile</a>
    <div class="dropdown-divider"></div>
    <a class="dropdown-item" (click)="logout()">Logout</a>
  </div>
</div>
```

### Persistent login

- Create a User **user.ts** model in a new **\_models** folder

```ts
export interface User {
  username: string;
  token: string;
}
```

- Account operations in account.service.ts

```ts
export class AccountService {
  //..
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();
  //..
  login(model: any) {
    return this.http.post(this.baseUrl + "account/login", model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          localStorage.setItem("user", JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }
  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }
  logout() {
    localStorage.removeItem("user");
    this.currentUserSource.next(null);
  }
}
```

- Operations in app.component.ts

```ts
//..
constructor(private http: HttpClient,
    private accountService: AccountService
  ) { }
 ngOnInit() {
//..
    this.setCurrentUser();
  }
  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    this.accountService.setCurrentUser(user);
  }
//..
```

- Operations in nav.component.ts

```ts
 ngOnInit(): void {
    this.getCurrentUser();
  }
  //..
   logout() {
    this.accountService.logout();
    this.loggedIn = false;
  }
  getCurrentUser() {
    this.accountService.currentUser$.subscribe(user => {
      this.loggedIn = !!user;
    },error => {console.log(error);})}
```

### Using the async pipe

- Removed loggedIn variable, getCurrentUser in nav.component.ts, and made accountService injection public to use in html template
- In template instead of **\*ngIf="loggedIn"** we need **\*ngIf="accountService.currentUser$ | async"**. And for ! we need to use === null

### Adding a home page

```bash
ng g c home # create a component named home
```

- Register field for new users in **register.component.ts**

```ts
export class HomeComponent implements OnInit {
  registerMode = false;
  //..
  registerToggle() {
    this.registerMode = !this.registerMode;
  }
}
```

```html
<div *ngIf="!registerMode" class="container mt-5" class="text-center">
  <h1>Find your match</h1>
  <p class="lead">
    Come on in to view your matches.. all you need to do is sign up!
  </p>
  <div class="text-center">
    <button (click)="registerToggle()" class="btn btn-primary btn-lg me-2">
      Register</button
    ><button class="btn btn-info btn-lg">Learn More</button>
  </div>
</div>

<div *ngIf="registerMode" class="container">
  <div class="row justify-content-center">
    <div class="col-4">
      <p>Register Form goes here</p>
    </div>
  </div>
</div>
```

- Add component to **app.component.html** template `<app-home></app-home>`

### Adding a register form

```bash
ng g c register # create a component named register
```

- Created simple methods for testing

```ts
export class RegisterComponent implements OnInit {
  model: any = {};
  //..
  register() {
    console.log(this.model);
  }
  cancel() {
    console.log("cancelled");
  }
}
```

- Form **register.component.html**

```html
<form #registerForm="ngForm" (ngSubmit)="register()" autocomplete="off">
  <h2 class="text-center text-primary">Sign up</h2>
  <hr />
  <div class="form-group">
    <input
      type="text"
      class="form-control"
      name="username"
      [(ngModel)]="model.username"
      placeholder="Username"
    />
    <div class="form-group mt-4 mb-4">
      <input
        type="password"
        class="form-control"
        name="password"
        [(ngModel)]="model.password"
        placeholder="Password"
      />
    </div>
    <div class="form-group text-center">
      <button class="btn btn-success me-4" type="submit">Register</button>
      <button class="btn btn-default me-4" (click)="cancel()" type="button">
        Cancel
      </button>
    </div>
  </div>
</form>
```

- **home.component.html** => `<app-register></app-register>`

### Parent to child communication

- In **home.component.ts** make get request to api to getUsers

```ts
export class HomeComponent implements OnInit {
  //..
  users: any;
  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.getUsers();
  }
  getUsers() {
    this.http
      .get("https://localhost:5001/api/users")
      .subscribe((users) => (this.users = users));
  }
}
```

- In **register.component.ts** (child of home) set a input paramater for parent to child relation

```ts
@Input() usersFromHomeComponent: any;
```

- Pass the parameter in **home.component.html** when calling **register.component.html** (child of home)

```html
<app-register [usersFromHomeComponent]="users"></app-register>
```

- In **register.component.html** (child of home) use values like this

```html
  </div>
  <div class="form-group">
    <label>Who is your favourite user?</label>
    <select class="form-control">
      <option value="" *ngFor="let user of usersFromHomeComponent" [value]="user.userName">
        {{ user.userName }}
      </option>
    </select>
  </div>
```

### Child to parent communication

- In **register.component.ts**

```ts
 @Output() cancelRegister = new EventEmitter() // import from '@angular/core'
```

- and use like this in a function

```ts
  cancel() {
    this.cancelRegister.emit(false);
  }
```

- In **home.component.ts** (parent of register)

```ts
  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }
```

- and lastly in **home.component.html** (parent of register)

```html
<app-register
  [usersFromHomeComponent]="users"
  (cancelRegister)="cancelRegisterMode($event)"
></app-register>
```

### Hooking up the register method to the service

- Implement register method in **account.service.ts**

```ts
  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        return user;
      })
    )
  }
```

- In **register.component.ts** inject **account.service.ts** and use it in a function

```ts
export class RegisterComponent implements OnInit {
//..
  constructor(private accountService: AccountService) { }
register() {
    this.accountService.register(this.model)
      .subscribe(
        response => {
          console.log(response);
          this.cancel();
        }, error => { console.log(error); })}}
//..
}
```

## 5 - Routing in Angular (FE)

### Creating some more components

- Create member-list, member-detail, lists, messages components

```bash
cd src/app
mkdir members
cd members
ng g c member-list --skip-tests
ng g c member-detail --skip-tests
cd ..
ng g c lists --skip-tests
ng g c messages --skip-tests
```

- In **app-routing.module.ts** add routes

```bash
const routes: Routes = [
	{ path: '', component: HomeComponent },
	{ path: 'members', component: MemberListComponent },
	{ path: 'members/:id', component: MemberDetailComponent },
	{ path: 'lists', component: ListsComponent },
	{ path: 'messages', component: MessagesComponent },
	{ path: '**', component: HomeComponent, pathMatch: 'full' },
];
```

- In **app.component.html** use `<router-outlet></router-outlet>`instead of `<app-home></app-home>`

### Adding the nav links

- In nav.component.html add the route to `<a>` tags with `routerLink=''`
- And with `routerLinkActive=''` you can add a css class when route is active

### Routing in code

- Inject Router in component.ts
- In functions call router like this `this.router.navigateByUrl('/members');`

### Adding toastr service for notifications

- `npm install ngx-toastr`
- In **angular.json** add `"./node_modules/font-awesome/css/font-awesome.css","./node_modules/ngx-toastr/toastr.css"` in styles array
- In **app.module.ts** add **ToastrModule** to imports array and configure like this `ToastrModule.forRoot({positionClass: 'toast-bottom-right'})`
- Inject in **component.ts** and use it in functions like `this.toastr.error(error)` or `this.toastr.success('message')` (also warning and info is available)

### Angular Route Guard

```bash
cd src/app
mkdir _guards
cd _guards
ng g guard auth --skip-tests
```

- add a constructor, simply guard and implement

```ts
export class AuthGuard implements CanActivate {
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map((user) => {
        if (user) return true;
        this.toastr.error("You shall not pass!");
      })
    );
  }
}
```

- add to routing module like

```ts
  { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] },
```

### Adding a dummy route

- In app.routing.module applied guard on all the paths

```
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] },
      { path: 'members/:id', component: MemberDetailComponent },
      { path: 'lists', component: ListsComponent },
      { path: 'messages', component: MessagesComponent },
      { path: '**', component: HomeComponent, pathMatch: 'full' },
    ]
  },
```

### Adding a new theme

```bash
npm install bootswatch
```

- angular.json, styles array `"./node_modules/bootswatch/dist/morph/bootstrap.css",`

### Shared module

```bash
cd src/app
mkdir _modules
cd _modules
ng g m shared --flat
```

- Move some modules into shared.module.ts and import in in app.module.ts

```ts
// app.module.ts
@NgModule({
  declarations: [//..
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    SharedModule // here
  ],
//..
})
// shared.module.ts
@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    })
  ],
  exports: [
    BsDropdownModule,
    ToastrModule
  ]
})
```

## 6 - Error Handling (BE+FE)

### Error Controller (BE)

Created a Controller named BuggyController and tested http responses

```cs
public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;

        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);

            if (thing == null) return NotFound();

            return Ok(thing);
        }
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1);

            var thingToReturn = thing.ToString();

            return thingToReturn;
        }
        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was not a good request");
        }
    }
```

### Exception Handling Middleware (BE)

- Created an errors entity named **ApiException** in **Errors** folder.

```cs
namespace API.Errors
{
    public class ApiException
    {
        public ApiException(int statusCode, string message = null, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
```

- Created **ExceptionMiddleware** in **Middleware** folder.

```cs
namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly IHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
```

- In **Startup.cs** replaced `app.UseDeveloperExceptionPage()` with `app.UseMiddleware<ExceptionMiddleware>()`

### Testing Errors in the Client (FE)

- Create test errors component

```bash
cd src/app
mkdir errors
cd errors
ng g c test-errors --skip-tests
```

- Add to routing `{ path: 'errors', component: TestErrorsComponent }`
- Make requests to the api buggy controller in **test-errors.component.ts**

```ts
export class TestErrorsComponent implements OnInit {
  baseUrl = "https://localhost:5001/api/";

  constructor(private http: HttpClient) {}

  ngOnInit(): void {}

  get404Error() {
    this.http.get(this.baseUrl + "buggy/not-found").subscribe(
      (response) => {
        console.log(response);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  get400Error() {
    this.http.get(this.baseUrl + "buggy/bad-request").subscribe(
      (response) => {
        console.log(response);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  get500Error() {
    this.http.get(this.baseUrl + "buggy/server-error").subscribe(
      (response) => {
        console.log(response);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  get401Error() {
    this.http.get(this.baseUrl + "buggy/auth").subscribe(
      (response) => {
        console.log(response);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + "account/register", {}).subscribe(
      (response) => {
        console.log(response);
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
```

- Add buttons for accessing in view

```html
<ng-container>
  <button (click)="get500Error()" class="btn btn-outline-primary mr3">
    Test 500 Error
  </button>
  <button (click)="get400Error()" class="btn btn-outline-primary mr3">
    Test 400 Error
  </button>
  <button (click)="get401Error()" class="btn btn-outline-primary mr3">
    Test 401 Error
  </button>
  <button (click)="get404Error()" class="btn btn-outline-primary mr3">
    Test 404 Error
  </button>
  <button (click)="get400ValidationError()" class="btn btn-outline-primary mr3">
    Test 400 Validation Error
  </button>
</ng-container>
```

### Adding an Error Interceptor (FE)

- Create an interceptor

```bash
cd /src/app
mkdir _interceptors
cd _interceptors
ng g interceptor error --skip-tests
```

- Satisy http errors in error interceptor

```ts
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              } else {
                this.toastr.error(error.statusText, error.status);
              }
              break;
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            case 404:
              this.router.navigateByUrl("/not-found");
              break;
            case 500:
              const navigationExtras: NavigationExtras = {
                state: { error: error.error },
              };
              this.router.navigateByUrl("/server-error", navigationExtras);
              break;
            default:
              this.toastr.error("Something unexpected went wrong");
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    );
  }
}
```

- Add to providers in app.module.ts

  ```ts
    providers: [
      { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
    ],
  ```

### Validation Errors (FE)

- test-errors.component

```ts
  validationErrors: string[] = [];

//..
  get400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
      this.validationErrors = error; // add to errorList
    })
  }
```

- Display in view

```html
<div class="row mt-4" *ngIf="validationErrors.length > 0">
  <ul class="text-danger">
    <li *ngFor="let error of validationErrors">{{ error }}</li>
  </ul>
</div>
```

### Handling not found (FE)

```bash
cd src/app/errors
ng g c not-found --skip-tests
```

- Create a not-found component and add it to route

### Server Error Page (FE)

```bash
cd src/app/errors
ng g c not-found --skip-tests
```

- server-error.component.ts

```ts
export class ServerErrorComponent implements OnInit {
  error: any;

  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    this.error = navigation?.extras?.state?.error;
  }
  ngOnInit(): void {}
}
```

- Some informative text on **server-error.component.html**

```html
<h4>
  Internal Server Error - Refreshing the page will make the error disappear!
</h4>

<ng-container *ngIf="error">
  <h5 class="text-danger">Error:{{ error.message }}</h5>
  <p class="font-weight-bold">Note: Not an Angular error</p>
  <p>What to do next?</p>
  <ol>
    <li>Open the chrome dev tools</li>
    <li>Inspect the network tab</li>
    <li>Check the failing request</li>
    <li>Examine the request URL - make sure it is correct</li>
    <li>
      Reproduce the error in Postman - if we see the same response, then the
      issue is not with angular
    </li>
  </ol>
  <p>
    Following is the stack trace - this is where your investigation should
    start!
  </p>
  <code class="mt-5" style="background-color: whitesmoke"
    >{{ error.details }}</code
  >
</ng-container>
```

- Add to routing

## 7 - Extending the API (BE)

### Extending the user entity

- added **DateOfBirth, KnownAs, Created, LastActive, Gender, Introduction, LookingFor, Interests, City, Country, Photos** (ICollection of Photo) props to **AppUser.cs**

### DateTime extension to calculate age

- Extensions/**DateTimeExtensions.cs**

```cs
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
```

- AppUser.cs

```cs
        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }
```

### Entity Framework Relationships

- To define one to many relationship between users and photos we are not going to add photos to dbcontext but we will Users relations to the photo entity

```cs
    [Table("Photos")] // table name annotation
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        // these fields define one to many in MANY(PHOTOS)
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }

    public class AppUser
    {
        //..
        // this field define one to many in ONE(USER)
         public ICollection<Photo> Photos { get; set; }
        //....
    }
```

### Generating Seed Data & Seeding Data

- Use [json-generator](https://www.json-generator.com/)
- Create a **Seed** class in Data

```cs
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            string SeedDataPath = "Data/UserSeedData.json";

            var userData = await System.IO.File.ReadAllTextAsync(SeedDataPath);
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);
            }
        }
    }
```

- Add functionality to Program.cs to seed everything and apply migrations at every startup.

```cs
 public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<DataContext>();
                await context.Database.MigrateAsync();
                await Seed.SeedUsers(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }

            await host.RunAsync();
        }
```

- Dropped database `dotnet ef database drop`
- Run app and db will be seeded.

### Applying Repository Pattern

- Create Interface for User in Interfaces

```cs
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUsernameAsync(string username);
    }
```

- Create Repository for User in Data

```cs
public class UserRepository : IUserRepository
    {
        public DataContext _context { get; }
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
```

- Add `services.AddScoped<IUserRepository, UserRepository>();` to **ApplicationServiceExtensions**
- Update **UsersController**

```cs
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _userRepository.GetUsersAsync());
        }

        // api/users/1
        [HttpGet("{username}")]
        public async Task<ActionResult<AppUser>> GetUser(string username)
        {
            return await _userRepository.GetUserByUsernameAsync(username);
        }
```

- This controllers returns photo as null, lets fix that by adding a DTO for Members and Photos and in the next part lets use automapper

```cs
  public class MemberDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }
    public class PhotoDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
    }
```

### AutoMapper

- Install **`AutoMapper.Extensions.Microsoft.DependencyInjection`** with NuGet
- Create Helpers Folder and Create **AutoMapperProfiles**

```cs
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>();
            CreateMap<Photo, PhotoDto>();
        }
    }
```

- Add to Extensions **`services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);`**

* Update Controller

```cs
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await _userRepository.GetUsersAsync();

            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            return Ok(usersToReturn);
        }

        // api/users/1
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);

            return _mapper.Map<MemberDto>(user);
        }
    }
```

- Configuring AutoMapper to get photoUrl from photos array:

```cs
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl // destination, our target
                           , opt => opt.MapFrom(src =>src.Photos.FirstOrDefault(x => x.IsMain).Url)) // map from there to our target
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge())); // from calculateAge method to age field of dto
```

### AutoMapper Queryable Extensions

- A more optimized way to get relational objects
- Add Extra methods to interface and implement them in UserRepository

```cs
    public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
```

- Use in Controller

## 8 - Building the User Interface (FE)

- Created interfaces for member and photo
- Moved **apiUrl** to environment files
- Create member service in **\_services** **`ng g s members --skip-tests`**

```ts
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + "users");
  }

  getMember(username: string) {
    return this.http.get<Member>(this.baseUrl + "users/" + username);
  }
}
```

### Retrieving the list of members

- In **member-list.component.ts**

```ts
export class MemberListComponent implements OnInit {
  members: Member[];

  constructor(private memberService: MembersService) {}

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers().subscribe((members) => {
      this.members = members;
    });
  }
}
```

- In **member-list.component.html**

```html
<div class="row">
  <div class="col-2">
    <p *ngFor="let member of members">{{ member.knownAs }}</p>
  </div>
</div>
```

### Creating member cards

- Create member-card component

```bash
cd src/app/members
ng g c member-car --skip-tests
```

- In **member-card.component.ts** add input field.

```ts
@Input() member: Member;
```

- In **member-card.component.html**

```html
<div class="card mb-4">
  <div class="card-img-wrapper">
    <img
      src="{{member.photoUrl || './assets/user.png'}}"
      alt="{{member.knownAs}}"
      class="card-img-top"
    />
    <ul class="list-inline member-icons animate text-center">
      <li class="list-inline-item">
        <button class="btn btn-primary">
          <i class="fa fa-user"></i>
        </button>
      </li>
      <li class="list-inline-item">
        <button class="btn btn-primary">
          <i class="fa fa-heart"></i>
        </button>
      </li>
      <li class="list-inline-item">
        <button class="btn btn-primary">
          <i class="fa fa-envelope"></i>
        </button>
      </li>
    </ul>
  </div>
  <div class="card-body p-1">
    <h6 class="card-title text-center mb-1">
      <i class="fa fa-user mr-2"></i>
      {{member.knownAs}}
    </h6>
    <p class="card-text text-muted text-center">{{member.city}}</p>
  </div>
</div>
```

- Add style to **member-card.component.css**

```css
.card:hover img {
  transform: scale(1.2, 1.2);
  transition-duration: 500ms;
  transition-timing-function: ease-out;
  opacity: 0.7;
}
.card img {
  transform: scale(1, 1);
  transition-duration: 500ms;
  transition-timing-function: ease-out;
}
.card-img-wrapper {
  overflow: hidden;
  position: relative;
}
.member-icons {
  position: absolute;
  bottom: -30%;
  left: 0;
  right: 0;
  margin-right: auto;
  margin-left: auto;
  opacity: 0;
}
.card-img-wrapper:hover .member-icons {
  bottom: 0;
  opacity: 1;
}
.animate {
  transition: all 0.3s ease-in-out;
}
```

### Using an interceptor to send the token

```shell
cd src/app/_interceptors
ng g interceptor jwt --skip-tests
```

- **jwt.interceptor.ts**

```ts
export class JwtInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    let currentUser: User;
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe((user) => (currentUser = user));
    if (currentUser) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`,
        },
      });
    }
    return next.handle(request);
  }
}
```

- add to **app.module.ts** `{provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true}`

### Routing to Detail page

- **member-detail.component.ts**

```ts
export class MemberDetailComponent implements OnInit {
  member: Member;

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    this.memberService
      .getMember(this.route.snapshot.paramMap.get("username"))
      .subscribe((member) => {
        this.member = member;
      });
  }
}
```

- **member-detail.component.html**

```html
<h1 *ngIf="member">{{member.knownAs}}</h1>
```

- Update **app-routing.module.ts** `members/:id` to `members/:username`
- Add routerLink to **member-card.component.html** `routerLink='/members/{{member.username}}'`

### Styling the member detailed page

- **member-detail.component.html**

```html
<div class="row" *ngIf="member">
  <div class="col-4">
    <div class="card">
      <img
        src="{{member.photoUrl || './assets/user.png'}}"
        alt="{{member.knownAs}}"
        class="card-img-top img-thumbnail"
      />
      <div class="card-body">
        <div>
          <strong>Location:</strong>
          <p>{{member.city}}, {{member.country}}</p>
        </div>
        <div>
          <strong>Age:</strong>
          <p>{{member.age}}</p>
        </div>
        <div>
          <strong>Last Active:</strong>
          <p>{{member.lastActive}}</p>
        </div>
        <div>
          <strong>Member since:</strong>
          <p>{{member.created}}</p>
        </div>
      </div>
      <div class="card-footer">
        <div class="btn-group d-flex">
          <button class="btn btn-primary">Like</button>
          <button class="btn btn-success">Message</button>
        </div>
      </div>
    </div>
  </div>
</div>
```

- **member-detail.component.css**

```css
.img-thumbnail {
  margin: 25px;
  width: 85%;
  height: 85%;
}
.card-body {
  padding: 0 25px;
}
.card-footer {
  padding: 10px 15px;
  background-color: #fff;
  border-top: none;
}
```

- Import and Export Angular-Bootstrap Tabs Module in **shared.module.ts**

- Added tabset to **member-detail.component.html**

```html
<div class="col-8">
  <tabset class="member-tabset">
    <tab heading="About {{member.knownAs}}">
      <h4>Description</h4>
      <p>{{member.introduction}}</p>
      <h4>Looking for</h4>
      <p>{{member.lookingFor}}</p>
    </tab>
    <tab heading="Interests">
      <h4>Interests</h4>
      <p>{{member.interests}}</p>
    </tab>
    <tab heading="Photos">
      <p>Photos will go here</p>
    </tab>
    <tab heading="Messages">
      <p>Messages will go here</p>
    </tab>
  </tabset>
</div>
```

- In global **styles.css**

```css
.tab-panel {
  border: 1px solid #ddd;
  padding: 10px;
  border-radius: 4px;
}
.nav-tabs > li.open,
.member-tabset > .nav-tabs > li:hover {
  border-bottom: 4px solid #fbcdcf;
}
.member-tabset > .nav-tabs > li.open > a,
.member-tabset > .nav-tabs > li:hover > a {
  border: 0;
  background: none !important;
  color: #333333;
}
.member-tabset > .nav-tabs > li.open > a > i,
.member-tabset > .nav-tabs > li:hover > a > i {
  color: #a6a6a6;
}
.member-tabset > .nav-tabs > li.open .dropdown-menu,
.member-tabset > .nav-tabs > li:hover .dropdown-menu {
  margin-top: 0px;
}
.member-tabset > .nav-tabs > li.active {
  border-bottom: 4px solid #e95420;
  position: relative;
}
.member-tabset > .nav-tabs > li.active > a {
  border: 0 !important;
  color: #333333;
}
.member-tabset > .nav-tabs > li.active > a > i {
  color: #404040;
}
.member-tabset > .tab-content {
  margin-top: -3px;
  background-color: #fff;
  border: 0;
  border-top: 1px solid #eee;
  padding: 15px 0;
}
```

### Adding a photo gallery

- `npm install @kolkov/ngx-gallery`
- Import (**shared.module.ts**)

```ts
@NgModule({
  declarations: [],
  imports: [
//..
    NgxGalleryModule
  ],
  exports: [
//..
    NgxGalleryModule
  ]
})
```

- Use in **member-detail.component.ts**. Final form of file

```ts
export class MemberDetailComponent implements OnInit {
  member: Member;
  galleryOptions: NgxGalleryOptions[]; //set these
  galleryImages: NgxGalleryImage[];
  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) {}
  ngOnInit(): void {
    this.loadMember();

    this.galleryOptions = [
      {
        width: "500px",
        height: "500px",
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];
  }
  // get images from db
  getImages(): NgxGalleryImage[] {
    const imageUrls = [];
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      });
    }
    return imageUrls;
  }
  loadMember() {
    this.memberService
      .getMember(this.route.snapshot.paramMap.get("username"))
      .subscribe((member) => {
        this.member = member;
        this.galleryImages = this.getImages();
      });
  }
}
```

- Insert this into html

```html
<tab heading="Photos">
  <ngx-gallery
    [options]="galleryOptions"
    [images]="galleryImages"
    style="display: inline-block; margin-bottom: 20px;"
  ></ngx-gallery>
</tab>
```

- Add a placeholder for user. Place an placeholder image on assets
- In html use it like this (if member.photoUrl is null set to placeholder)

```html
<img
  src="{{member.photoUrl || './assets/user.png'}}"
  alt="{{member.knownAs}}"
  class="card-img-top img-thumbnail"
/>
```

## 9 - Updating Resources

### Member edit component

```bash
cd src/app/members
ng g c member-edit --skip-tests
```

- add to routing and add routerLink to `<a>` in html
- **member.edit.component.ts**

```ts
export class MemberEditComponent implements OnInit {
  member: Member;
  user: User;
  constructor(
    private accountService: AccountService,
    private memberService: MembersService
  ) {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));
  }
  ngOnInit(): void {
    this.loadMember();
  }
  loadMember() {
    this.memberService.getMember(this.user.username).subscribe((member) => {
      this.member = member;
    });
  }
}
```

- Access in html

```html
<h1 *ngIf="member">{{ member.username }}</h1>
```

### Edit Template Form

- Copied and modified detail component for our needs

```html
<div class="row" *ngIf="member">
  <div class="col-4">
    <h1>Your Profile</h1>
  </div>
  <div class="col-8">
    <div class="alert alert-info">
      <p>
        <strong>Information:</strong> You have made changes. Any unsaved changes
        will be lost
      </p>
    </div>
  </div>
  <div class="col-4">
    <div class="card">
      <img
        src="{{ member.photoUrl || './assets/user.png' }}"
        alt="{{ member.knownAs }}"
        class="card-img-top img-thumbnail"
      />
      <div class="card-body">
        <div>
          <strong>Location:</strong>
          <p>{{ member.city }}, {{ member.country }}</p>
        </div>
        <div>
          <strong>Age:</strong>
          <p>{{ member.age }}</p>
        </div>
        <div>
          <strong>Last Active:</strong>
          <p>{{ member.lastActive }}</p>
        </div>
        <div>
          <strong>Member since:</strong>
          <p>{{ member.created }}</p>
        </div>
      </div>
      <div class="card-footer">
        <button class="btn btn-success btn-block">Save Changes</button>
      </div>
    </div>
  </div>

  <div class="col-8">
    <tabset class="member-tabset">
      <tab heading="About {{ member.knownAs }}">
        <form>
          <h4>Description</h4>
          <textarea
            class="form-control"
            [(ngModel)]="member.introduction"
            name="introduction"
            rows="6"
          ></textarea>
          <h4 class="mt-2">Looking for</h4>
          <textarea
            class="form-control"
            [(ngModel)]="member.lookingFor"
            name="lookingFor"
            rows="6"
          ></textarea>
          <h4 class="mt-2">Interests</h4>
          <textarea
            class="form-control"
            [(ngModel)]="member.interests"
            name="interests"
            rows="6"
          ></textarea>
          <h4 class="mt-2">Location Details:</h4>
          <div class="form-inline">
            <label for="city">City</label>
            <input
              class="form-control"
              [(ngModel)]="member.city"
              type="text"
              name="city"
              class="form-control mx-2"
            />
            <label for="country">Country</label>
            <input
              class="form-control"
              [(ngModel)]="member.country"
              type="text"
              name="country"
              class="form-control mx-2"
            />
          </div>
        </form>
      </tab>
      <tab heading="Edit photos">
        <p>Photo edit will go here</p>
      </tab>
    </tabset>
  </div>
</div>
```

- Also copied css

### Adding the update functionality

- Make the form an angular form by adding `#editForm="ngForm" (ngSubmit)="updateMember() to <form>`.
- Hook the button to form by adding `form="editForm"` to button and `id="editForm"` to form.
- Disable info when not made any changes with adding \*`ngIf=`"editForm.dirty".
- Get access to form from component `@ViewChild('editForm') editForm: NgForm;` to preserve data after an action.
- Implement pseudo **updateMember** function.

```ts
  updateMember() {
    console.log(this.member);
    this.toastr.success('Profile updated')
    this.editForm.reset(this.member);
  }
```

### Can Deactivate Route Guard

```bash
cd src/app/_guards
ng g g prevent-unsaved-changes --skip-tests
(*) Can Deactivate
```

- **prevent-unsaved-changes.guard.ts**

```ts
canDeactivate(
    component: MemberEditComponent): boolean {
    if (component.editForm.dirty) {return confirm('Are you sure you want to continue? Any unsaved changes will be lost!')}
    return true;}}
```

- add guard to **app-routing.module.ts**

```ts
{ path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard] },
```

- To prevent users from going to another site or closing the tab add this to component, path `canDeactivate: [PreventUnsavedChangesGuard]`

```ts
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty) {$event.returnValue=true;}}
```

### Persisting the changes in API (BE)

- Create a **MemberUpdateDto** with only the props that we wanna show in update view.
- Map in **AutoMapperProfiles** `CreateMap<MemberUpdateDto, AppUser>();`
- Create Action on **UsersController**

```cs
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to update user");
        }
```

### Updating user in the client app

- Make request **updateMember** in **members.service**

```ts
updateMember(member: Member) { return this.http.put(this.baseUrl + 'users', member);}}
```

- Configure **updateMember** in **member-edit.component.ts**

```ts
  updateMember() {
    this.memberService.updateMember(this.member).subscribe(()=>{
      this.toastr.success('Profile updated')
      this.editForm.reset(this.member);
    })
```

### Loading Indicators

```bash
npm install ngx-spinner
```

- Add to **shared.module.ts**

```ts
 imports: [
// ...
    NgxSpinnerModule
  ],  exports: [
//..
    NgxSpinnerModule
  ]
```

- Create busy service

```bash
cd src/app/_services
ng g s busy --skip-tests
```

```ts
export class BusyService {
  busyRequestCount = 0;
  constructor(private spinnerService: NgxSpinnerService) {}
  busy() {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: "line-scale-party",
      bdColor: "rgba(255,255,255,0)",
      color: "#333333",
    });
  }
  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
```

- Create loading interceptor

```bash
ng g interceptor loading --skip-tests
```

```ts
export class LoadingInterceptor implements HttpInterceptor {
  constructor(private busyService: BusyService) {}
  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    this.busyService.busy();
    return next.handle(request).pipe(
      delay(100),
      finalize(() => {
        this.busyService.idle();
      })
    );
  }
}
```

- Add to **app.module.ts** providers `{ provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },`
- Add to app.component.html

```html
<ngx-spinner>
  <h3>Loading...</h3>
</ngx-spinner>
```

### Using service to store state

- Inside members service

```ts
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  constructor(private http: HttpClient) {}
  getMembers() {
    if (this.members.length > 0) return of(this.members);
    return this.http.get<Member[]>(this.baseUrl + "users").pipe(
      map((members) => {
        this.members = members;
        return members;
      })
    );
  }
  getMember(username: string) {
    const member = this.members.find((x) => x.username === username);
    if (member !== undefined) return of(member);
    return this.http.get<Member>(this.baseUrl + "users/" + username);
  }
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + "users", member).pipe(
      map(() => {
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }
}
```

- Inside member-list.component.ts

```ts
export class MemberListComponent implements OnInit {
  members$: Observable<Member[]>;
  constructor(private memberService: MembersService) {}
  ngOnInit(): void {
    this.members$ = this.memberService.getMembers();
  }
}
```

- And in html of member-list

```html
<div class="row">
  <div *ngFor="let member of members$ | async" class="col-2">
    <app-member-card [member]="member"></app-member-card>
  </div>
</div>
```

## 10 - Photo Upload Functionality

### Configuring Cloudinary in the API (BE)

- Install CloudinaryDotNet NuGet Package
- Insert keys into appsettings.json

```json
  "CloudinarySettings": {
    "CloudName": "aaaa",
    "ApiKey": "1111",
    "ApiSecret":"2222"
  }
```

- Create CloudinarySettings in Helpers folder

```cs
namespace API.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
```

- Add to ApplicationServiceExtensions AddApplicationServices `services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));`

### Adding a photo service (BE)

- Create a IPhotoService Interface in Interfaces

```cs
public interface IPhotoService
{
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}
```

- Implement Service in Service Folder

```cs
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var acc = new Account(config.Value.CloudName,config.Value.ApiKey,config.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }
        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
```

- Add to services `services.AddScoped<IPhotoService, PhotoService>();`

### Updating the users controller (BE)

- Cleanup on Getting user name. Moved function to a new extension name **ClaimsPrincipleExtensions**

```cs
namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
```

- **AddPhoto** Action in **UsersController**

```cs
 [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0) photo.IsMain = true;
            user.Photos.Add(photo);
            if (await _userRepository.SaveAllAsync())
                return _mapper.Map<PhotoDto>(photo);
            return BadRequest("Problem adding photo");
        }
```

### Using the CreatedAtRoute method (BE)

- Give GetUser action a name to access in other actions

```cs
[HttpGet("{username}", Name = "GetUser")]
public async Task<ActionResult<MemberDto>> GetUser(string username){
    //...
}
```

- In AddPhoto action's save method return CreatedAtRoute

```cs
if (await _userRepository.SaveAllAsync())
{
    return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
}
```

### Adding a Photo Editor Component (FE)

- Generate photo editor component

```bash
cd src/app
ng g c photo-editor --skip-tests
```

- Get input from parent (member-edit) `@Input() member: Member;`
- In **member-edit.component.html** (member is populated from earlier)

```html
<app-photo-editor [member]="member"></app-photo-editor>
```

### Photo Uploader (FE)

- Install **ng2-file-upload** package

```bash
npm install ng2-file-upload
```

- Add to **shared.module.ts**

```ts
  imports: [
//..
    FileUploadModule
  ],
  exports: [
//..
    FileUploadModule
  ]
```

- Add file upload functionality to **photo-editor-component.ts**

```ts
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;

  constructor(private accountService: AccountService) {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + "users/add-photo",
      authToken: "Bearer" + this.user.token,
      isHTML5: true,
      allowedFileType: ["image"],
      removeAfterUpload: true,
      autoUpload: true,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    };
  }
}
```

- In template

```html
<div class="row mt-3">
  <div class="col-md-3">
    <h3>Add Photos</h3>

    <div
      ng2FileDrop
      [ngClass]="{ 'nv-file-over': hasBaseDropZoneOver }"
      (fileOver)="fileOverBase($event)"
      [uploader]="uploader"
      class="card bg-faded p-3 text-center mb-3 my-drop-zone"
    >
      <i class="fa fa-upload fa-3x"></i>
      Drop photos here
    </div>
    Multiple
    <input type="file" ng2FileSelect [uploader]="uploader" multiple /><br />
    Single
    <input type="file" ng2FileSelect [uploader]="uploader" />
  </div>
  <div
    class="col-md-9"
    style="margin-bottom: 40px"
    *ngIf="uploader?.queue?.length"
  >
    <h3>Upload queue</h3>
    <p>Queue length: {{ uploader?.queue?.length }}</p>
    <table class="table">
      <thead>
        <tr>
          <th width="50%">Name</th>
          <th>Size</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let item of uploader.queue">
          <td>
            <strong>{{ item?.file?.name }}</strong>
          </td>
        </tr>
      </tbody>
    </table>
    <div>
      <div>
        Queue progress:
        <div class="progress" style="">
          <div
            class="progress-bar"
            role="progressbar"
            [ngStyle]="{ width: uploader.progress + '%' }"
          ></div>
        </div>
      </div>
      <button
        type="button"
        class="btn btn-success btn-s"
        (click)="uploader.uploadAll()"
        [disabled]="!uploader.getNotUploadedItems().length"
      >
        <span class="fa fa-upload"></span> Upload all
      </button>
      <button
        type="button"
        class="btn btn-warning btn-s"
        (click)="uploader.cancelAll()"
        [disabled]="!uploader.isUploading"
      >
        <span class="fa fa-ban"></span> Cancel all
      </button>
      <button
        type="button"
        class="btn btn-danger btn-s"
        (click)="uploader.clearQueue()"
        [disabled]="!uploader.queue.length"
      >
        <span class="fa fa-trash"></span> Remove all
      </button>
    </div>
  </div>
</div>
```

- and a bit styling in css

```css
img.img-thumbnail {
  height: 100px;
  min-width: 100px !important;
  margin-bottom: 2px;
}
.nv-file-over {
  border: dotted 3px red;
}
input[type="file"] {
  color: transparent;
}
```

### Setting the main photo in the API (BE)

- **UsersController.cs**

```ts
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.IsMain) return BadRequest("This is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }
```

### Adding the main photo to the navbar (FE)

- Add **PhotoUrl** field to **UserDto** in dotnet api and **user.ts** in angular client
- In **AccountController's Login** method include photos table and set **PhotoUrl** when logging in

```cs
var user = await _context.Users
    .Include(p => p.Photos)
    .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);
//..
return new UserDto
{Username=user.UserName,Token =_tokenService.CreateToken(user),PhotoUrl=user.Photos.FirstOrDefault(x => x.IsMain)?.Url};
```

- tidied up **account.service.ts**
- **nav.component.html** `<img src="{{ user.photoUrl }}" alt="{{ user.photoUrl }}" />`
- **nav.component.css**

```css
img {
  max-height: 40px;
  border: 2px solid #fff;
  display: inline;
  margin-right: 2em;
}
```

### Setting the main photo in the client (FE)

- Call put method of api in **members.service.ts**

```ts
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {})
  }
```

- fill request in **photo-editor.component.ts**

```ts
  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe(() => {
      this.user.photoUrl = photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl = photo.url;
      this.member.photos.forEach(p => {
        if (p.isMain) p.isMain = false;
        if (p.id === photo.id) p.isMain = true;
      })
    })
  }
```

- Update the template

```html
<button
  [disabled]="photo.isMain"
  (click)="setMainPhoto(photo)"
  [ngClass]="photo.isMain ? 'btn-success active' : 'btn-outline-success'"
  class="btn btn-sm"
>
  Main
</button>
```

### Deleting Photos API (BE+FE)

- On API UsersController

```cs
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You cannot delete your main photo");
            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await _userRepository.SaveAllAsync()) return Ok();
            return BadRequest("Failed to Delete the Photo");
        }
```

- In Angular **members.service.ts**

```ts
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }
```

- In **photo-editor.component.ts**

```ts
  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe(() => {
      this.member.photos = this.member.photos.filter(x => x.id !== photoId);
    })
  }
```

- In template add disabled and click to delete button

```html
<button
  [disabled]="photo.isMain"
  (click)="deletePhoto(photo.id)"
  class="btn btn-sm btn-danger"
>
  <i class="fa fa-trash"></i>
</button>
```

## 11 - Reactive Forms

- Import ReactiveForms in **app.module.ts**
- Setup reactive form in **register.component.ts**

```ts
export class RegisterComponent implements OnInit {
 //..
  registerForm: FormGroup;
//..
  ngOnInit(): void {
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl(),
      password: new FormControl(),
      confirmPassword: new FormControl()
    })
  }
```

- In View remove template reference variables, ng models and name fields. Add formGroup and formControlNames

```html
<form [formGroup]="registerForm" (ngSubmit)="register()" autocomplete="off">
  <h2 class="text-center text-primary">Sign up</h2>
  <hr />
  <div class="form-group">
    <input
      type="text"
      class="form-control"
      formControlName="username"
      placeholder="Username"
    />
  </div>
  <div class="form-group">
    <input
      type="password"
      class="form-control"
      formControlName="password"
      placeholder="Password"
    />
  </div>
  <div class="form-group">
    <input
      type="password"
      class="form-control"
      formControlName="confirmPassword"
      placeholder="Password again"
    />
  </div>
  <div class="form-group text-center">
    <button class="btn btn-success mr-2" type="submit">Register</button>
    <button class="btn btn-default mr-2" (click)="cancel()" type="button">
      Cancel
    </button>
  </div>
</form>

<p>Form value: {{ registerForm.value | json }}</p>
<p>Form value: {{ registerForm.status | json }}</p>
```

### Client side validation

- Validators

```ts
   initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('Username', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', Validators.required)
    })
  }
```

### Custom validators

- A Validator for pass and pass confirm matching

```ts
  initializeForm() {
    this.registerForm = new FormGroup({
      username: new FormControl('Username', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      //confirmpass, custom validator
      confirmPassword: new FormControl('', [Validators.required, this.matchValues('password')])
    });
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching: true }
    }
  }
```

### Validation Feedback

- In html files use it like this and add messages with div

```html
<input />
<input
  [class.is-invalid]="registerForm.get('username').errors && registerForm.get('username').touched"
  type="text"
  class="form-control"
  formControlName="username"
  placeholder="Username"
/>
<div class="invalid-feedback">Please enter a username</div>
<!-- -->
<input
  [class.is-invalid]="registerForm.get('password').errors && registerForm.get('password').touched"
  type="password"
  class="form-control"
  formControlName="password"
  placeholder="Password"
/>
<div
  *ngIf="registerForm.get('password').hasError('required')"
  class="invalid-feedback"
>
  Please enter a password
</div>
<div
  *ngIf="registerForm.get('password').hasError('minlength')"
  class="invalid-feedback"
>
  Password must be at least 4 characters
</div>
<div
  *ngIf="registerForm.get('password').hasError('maxlength')"
  class="invalid-feedback"
>
  Password must be at most 8 characters
</div>
<!-- -->
<input
  [class.is-invalid]=" registerForm.get('confirmPassword').errors && registerForm.get('confirmPassword').touched"
  type="password"
  class="form-control"
  formControlName="confirmPassword"
  placeholder="Password again"
/>
<div
  *ngIf="registerForm.get('confirmPassword').hasError('required')"
  class="invalid-feedback"
>
  Please enter a confirm password
</div>
<div
  *ngIf="registerForm.get('confirmPassword').hasError('isMatching')"
  class="invalid-feedback"
>
  Passwords must match
</div>
```

### Creating a reusable text input

- Created a **\_forms** folder and generate a component named **text-input** `ng g c text-input --skip-tests`
- Implent ControlValueAccessor in text-input.component instead of OnInit
- Empty not implemented exceptions and add input, inject control

```ts
export class TextInputComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() type = "text";
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
  }
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
}
```

- In html use it like this

```html
<div class="form-group">
  <input
    [class.is-invalid]="ngControl.touched && ngControl.invalid"
    type="{{ type }}"
    class="form-control"
    [formControl]="ngControl.control"
    placeholder="{{ label }}"
  />
  <div *ngIf="ngControl.control.errors?.required" class="invalid-feedback">
    Please enter a {{ label }}
  </div>
  <div *ngIf="ngControl.control.errors?.minlength" class="invalid-feedback">
    {{ label }} must be at least {{
    ngControl.control.errors.minlength["requiredLength"] }}
  </div>
  <div *ngIf="ngControl.control.errors?.maxlength" class="invalid-feedback">
    {{ label }} must be at most {{
    ngControl.control.errors.maxlength["requiredLength"] }}
  </div>
  <div *ngIf="ngControl.control.errors?.isMatching" class="invalid-feedback">
    Passwords do not match
  </div>
</div>
```

- Now that we made a template for all of the input we could use it at our register component

```html
<form [formGroup]="registerForm" (ngSubmit)="register()" autocomplete="off">
  <h2 class="text-center text-primary">Sign up</h2>
  <hr />
  <app-text-input
    [formControl]="registerForm.controls['username']"
    [label]="'Username'"
  >
  </app-text-input>
  <app-text-input
    [formControl]="registerForm.controls['password']"
    [label]="'Password'"
    [type]='"password"'
  >
  </app-text-input>
  <app-text-input
    [formControl]="registerForm.controls['confirmPassword']"
    [label]="'Confirm Password'"
    [type]='"password"'
  >
  </app-text-input>
  <div class="form-group text-center">
    <button class="btn btn-success mr-2" type="submit">Register</button>
    <button class="btn btn-default mr-2" (click)="cancel()" type="button">
      Cancel
    </button>
  </div>
</form>
```

### Using the form builder service

- Inject FormBuilder in register.component.ts. Swap new form group with array brackets

```ts
  constructor(  //...
      private fb: FormBuilder) { }
    //..
  initializeForm() {this.registerForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });}
```

### Expanding the register form

- Add new fields to initializeForm in **register-component.ts**

```ts
  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    });
  }
```

- Add these fields to html template

```html
<form [formGroup]="registerForm" (ngSubmit)="register()" autocomplete="off">
  <h2 class="text-center text-primary">Sign up</h2>
  <hr />
  <div class="form-group">
    <label class="control-label" style="margin-right:10px;">I am a:</label>
    <label class="radio-inline">
      <input type="radio" value="male" formControlName="gender" />Male
    </label>
    <label class="radio-inline">
      <input type="radio" value="female" formControlName="gender" />Female
    </label>
  </div>
  <app-text-input
    [formControl]="registerForm.controls['username']"
    [label]="'Username'"
  >
  </app-text-input
  ><app-text-input
    [formControl]="registerForm.controls['knownAs']"
    [label]="'knownAs'"
  >
  </app-text-input
  ><app-text-input
    [formControl]="registerForm.controls['dateOfBirth']"
    [label]="'dateOfBirth'"
  >
  </app-text-input
  ><app-text-input
    [formControl]="registerForm.controls['city']"
    [label]="'city'"
  >
  </app-text-input
  ><app-text-input
    [formControl]="registerForm.controls['country']"
    [label]="'country'"
  >
  </app-text-input>
  <app-text-input
    [formControl]="registerForm.controls['password']"
    [label]="'Password'"
    [type]='"password"'
  >
  </app-text-input
  ><app-text-input
    [formControl]="registerForm.controls['confirmPassword']"
    [label]="'Confirm Password'"
    [type]='"password"'
  >
  </app-text-input>
  <div class="form-group text-center">
    <button class="btn btn-success mr-2" type="submit">Register</button>
    <button class="btn btn-default mr-2" (click)="cancel()" type="button">
      Cancel
    </button>
  </div>
</form>
```

### Adding Reusable date input

- Add datepicker modlue in shared.module.ts

```ts
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
//...
@NgModule({
  declarations: [],
  imports: [
//...
    BsDatepickerModule.forRoot()
  ],
  exports: [
//...
    BsDatepickerModule
  ]
})
```

- Create dateInput form for reusability

```bash
cd src/app/_forms
ng g c text-input --skip-tests
```

- Date Input Component

```ts
export class DateInputComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() maxDate: Date;
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: "theme-red",
      dateInputFormat: "DD MMMM YYYY",
    };
  }
  writeValue(obj: any): void {}
  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
}
```

- Date Input Template

```html
<div class="form-group">
  <input
    type="text"
    class="form-control"
    [class.is-invalid]="ngControl.touched && ngControl.invalid"
    [formControl]="ngControl.control"
    placeholder="{{ label }}"
    bsDatepicker
    [bsConfig]="bsConfig"
    [maxDate]="maxDate"
  />
  <div *ngIf="ngControl.control.errors?.required" class="invalid-feedback">
    {{ label }} is required
  </div>
</div>
```

- In main register template change **app-text-input** to **app-date-input**
- For constraints (ex: age restrictions) go to **register-component.ts**

```ts
  maxDate: Date;
  //...
  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }
```

- Pass date to template (**register.component.html**)

```html
<app-date-input
  [formControl]="registerForm.controls['dateOfBirth']"
  [label]="'dateOfBirth'"
  [maxDate]="maxDate"
>
</app-date-input>
```

### Updating the API register method (BE)

- Add new fields to RegisterDto

```cs
namespace API.DTOs
{
    public class RegisterDto
    {
        [Required] public string Username { get; set; }
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }
        [Required] [StringLength(8, MinimumLength = 4)] public string Password { get; set; }
    }
}
```

- Add new mapping to AutoMapperProfiles `CreateMap<RegisterDto, AppUser>();`
- Update register action in **AccountController**

```cs
[HttpPost("register")]
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

    var user = _mapper.Map<AppUser>(registerDto);

    using var hmac = new HMACSHA512();

    user.UserName = registerDto.Username.ToLower();
    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
    user.PasswordSalt = hmac.Key;

    _context.Add(user);
    await _context.SaveChangesAsync();

    return new UserDto
    {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        KnownAs = user.KnownAs // add this new field to UserDto too!!!
    };
}
```

### Client side registration

- In register component, update register method

```ts
export class RegisterComponent implements OnInit {
    //...
    validationErrors: string[] = [];

    constructor(
    //...
    private router: Router) { }
    //...
    register() {
        this.accountService.register(this.registerForm.value).subscribe(response => {
            this.router.navigateByUrl('/members');
        }, error => {
            this.validationErrors = error;
        })
    }
```

- Add if data is valid condition to register form template also get validations from api

```html
<form
  [formGroup]="registerForm"
  (ngSubmit)="registerForm.valid && register()"
  autocomplete="off"
>
  <!-FORM FIELDS HERE-->
  <div class="row" *ngIf="validationErrors.length > 0">
    <ul class="text-danger">
      <li *ngFor="let error of validationErrors">{{ error }}</li>
    </ul>
  </div>

  <div class="form-group text-center">
    <button
      [disabled]="!registerForm.valid"
      class="btn btn-success mr-2"
      type="submit"
    >
      Register
    </button>
    <button class="btn btn-default mr-2" (click)="cancel()" type="button">
      Cancel
    </button>
  </div>
</form>
```

- Some enhancements like update navbar photo when user uploads new photo **nav.component.html**

```html
<img
  src="{{ user.photoUrl || './assets/user.png' }}"
  alt="{{ user.photoUrl }}"
/>
```

- Set new main in everywhere. In **photo-editor.component**

```ts
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo: Photo = JSON.parse(response);
        this.member.photos.push(photo);
        if (photo.isMain) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }}}}
```

## 12 - Paging, Sorting, Filtering

### Paged List Class

```cs
namespace API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
```

### Adding helper classes for pagination

- **PaginationHeader.cs** Helper method

```cs
namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
```

- **HttpExtensions.cs** Extension method

```cs
namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
```

- **UserParams.cs** Helper method

```cs
namespace API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        public int _pageSize { get; set; } = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
```

### Using the pagination classes

- Updated **GetMembersAsync** method in **IUserRepository.cs** interface to return **PagedList** of MemberDto Task with user parameters  
  `Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);`
- Updated implementation in **UserRepository.cs**

```cs
public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
{
    var query = _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();
    return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
}
```

- Updated **UserController** to utilize pagination

```cs
[HttpGet]
public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
{
    var users = await _userRepository.GetMembersAsync(userParams);
    Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
    return Ok(users);
}
```

- Configure HttpExtensions to return JSON as camel case

```cs
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
```

### Pagination in client

- Create a pagination model in \_models

```ts
export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}
export class PaginatedResult<T> {
  result: T;
  pagination: Pagination;
}
```

- Update **member.service.ts** getMembers()

```ts
 export class MembersService {
 //...
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
 //...
 getMembers(page?: number, itemsPerPage?: number) {
    let params = new HttpParams();
    if (page !== null && itemsPerPage !== null) {
      params = params.append('pageNumber', page.toString());
      params = params.append('pageSize', itemsPerPage.toString());
    }
    return this.http.get<Member[]>(this.baseUrl + 'users', { observe: 'response', params }).pipe(
      map(response => {
        this.paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          this.paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));}
        return this.paginatedResult;}))}
  //...
```

- Update member-list.component

```ts
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  pageNumber = 1;
  pageSize = 5;
  constructor(private memberService: MembersService) {}
  ngOnInit(): void {
    this.loadMembers();
  }
  loadMembers() {
    this.memberService
      .getMembers(this.pageNumber, this.pageSize)
      .subscribe((response) => {
        this.members = response.result;
        this.pagination = response.pagination;
      });
  }
}
```

- Update member-list html

```html
<div class="row">
  <div *ngFor="let member of members" class="col-2">
    <app-member-card [member]="member"></app-member-card>
  </div>
</div>
```

### Using the angular bootstrap pagination component

- In shared modules import PaginationModule from ngx-bootstrap/pagination

```ts
//...
import { PaginationModule } from "ngx-bootstrap/pagination";

@NgModule({
  declarations: [],
  imports: [//...
    PaginationModule.forRoot()
  ],
  exports: [//...
    PaginationModule
  ]
})
```

- Add **pageChanged** event function in **member-list.component**

```ts
pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadMembers();
}
```

- In template use it like this

```html
<div class="d-flex justify-content-center" *ngIf="pagination">
  <pagination
    [boundaryLinks]="true"
    [totalItems]="pagination.totalItems"
    [itemsPerPage]="pagination.itemsPerPage"
    [(ngModel)]="pagination.currentPage"
    (pageChanged)="pageChanged($event)"
    previousText="&lsaquo;"
    nextText="&rsaquo;"
    firstText="&laquo;"
    lastText="&raquo;"
  >
  </pagination>
</div>
```

### Filtering in API

- We only want to show opposite gender and not themselves on the members list. First add **CurrentUsername** and **Gender** fields to **UserParams.cs**
- Update **UsersController GetUsers** Action

```cs
[HttpGet]
public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
{
    var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());
    userParams.CurrentUsername = user.UserName;
    if (string.IsNullOrEmpty(userParams.Gender)) userParams.Gender = user.Gender == "male" ? "female" : "male";
    var users = await _userRepository.GetMembersAsync(userParams);
    Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
    return Ok(users);
}
```

- Update **UserRepository GetMembersAsync** query

```cs
public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
{
    var query = _context.Users.AsQueryable();

    query = query.Where(u => u.UserName != userParams.CurrentUsername);
    query = query.Where(u => u.Gender == userParams.Gender);

    return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper
                                                                             .ConfigurationProvider).AsNoTracking(),
                                                  userParams.PageNumber, userParams.PageSize);
}
```

- Now we get filtered results but query like this we could get the same gender `/api/users?gender=female`

### Adding Additional Parameters

- Lets add min and max age to UserParams `public int MinAge { get; set; } = 18;` `public int MaxAge { get; set; } = 150;`
- Add new parameters to GetMembersAsync on **UserRepository**

```cs
public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
{
    var query = _context.Users.AsQueryable();
    query = query.Where(u => u.UserName != userParams.CurrentUsername);
    query = query.Where(u => u.Gender == userParams.Gender);
    var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
    var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
    query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
    return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
                                                  userParams.PageNumber, userParams.PageSize);
}
```

### Cleaning up the member service

- Update **UserDto**

```cs
public class UserDto
{
    public string Username { get; set; }
    public string Token { get; set; }
    public string PhotoUrl { get; set; }
    public string KnownAs { get; set; }
    public string Gender { get; set; }
}
```

- Add **Gender to AccountController Login and Register**

```cs
[HttpPost("register")]
public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
{
    //...
    return new UserDto
    {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        KnownAs = user.KnownAs,
        Gender = user.Gender
    };
}

[HttpPost("login")]
public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
{
    //...

    return new UserDto
    {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.Gender
    };
}
```

- Add knownAs and gender to **user model** in client, change **nav.components.html** welcome text to **knownAs**
- Create **userParams** model in client

```ts
import { User } from "./user";

export class UserParams {
  gender: string;
  minAge = 18;
  maxAge = 99;
  pageNumber = 1;
  pageSize = 5;
  orderBy = "lastActive";

  constructor(user: User) {
    this.gender = user.gender === "female" ? "male" : "female";
  }
}
```

- members.service overhaul

```ts
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
  }
  //...
  getMembers(userParams: UserParams) {
    let params = this.getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    params = params.append("minAge", userParams.minAge.toString());
    params = params.append("maxAge", userParams.maxAge.toString());
    params = params.append("gender", userParams.gender);

    return this.getPaginatedResult<Member[]>(this.baseUrl + "users", params);
  }
  //...
  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: "response", params }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get("Pagination") !== null) {
          paginatedResult.pagination = JSON.parse(
            response.headers.get("Pagination")
          );
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append("pageNumber", pageNumber.toString());
    params = params.append("pageSize", pageSize.toString());
    return params;
  }
}
```

- member-list.component

```ts
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  constructor(
    private memberService: MembersService,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
  }
  ngOnInit(): void {
    this.loadMembers();
  }
  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe((response) => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }
  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.loadMembers();
  }
}
```

### Filter Buttons on client

- member-list.component add genderList with values and add resetFilter Function

```ts
export class MemberListComponent implements OnInit {
  //...
  genderList = [
    { value: "male", display: "Males" },
    { value: "female", display: "Females" },
  ];
  //...
  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }
}
```

- member-list html add buttons and selectors

```html
<div class="text-center mt-3">
  <h2>Your matches - {{ pagination?.totalItems }} found</h2>
</div>
<div class="container mt-3">
  <form
    #form="ngForm"
    class="form-inline mb-3"
    (ngSubmit)="loadMembers()"
    autocomplete="off"
  >
    <div class="form-group">
      <label>Age from: </label>
      <input
        type="number"
        class="form-control ml-1"
        style="width: 70px"
        name="minAge"
        [(ngModel)]="userParams.minAge"
      />
    </div>

    <div class="form-group px-2">
      <label>Age to: </label>
      <input
        type="number"
        class="form-control ml-1"
        style="width: 70px"
        name="maxAge"
        [(ngModel)]="userParams.maxAge"
      />
    </div>

    <div class="form-group px-2">
      <label>Show: </label>
      <select
        name="gender"
        style="width: 130px"
        [(ngModel)]="userParams.gender"
        class="form-control ml-1"
      >
        <option *ngFor="let gender of genderList" [value]="gender.value">
          {{ gender.display }}
        </option>
      </select>
    </div>
    <button class="btn btn-primary ml-1" type="submit">Apply Filters</button>
    <button (click)="resetFilters()" class="btn btn-info ml-1" type="submit">
      Reset Filters
    </button>
  </form>
</div>
```

### Sort functionality

- Add OrderBy field to UserParams and set initial value "LastActive" in API
- Add OrderBy functionality to UserRepository

```cs
query = userParams.OrderBy switch
{
        "created" => query.OrderByDescending(u => u.Created),
        _ => query.OrderByDescending(u => u.LastActive)
};
```

### Adding an action filter (LogUserActivity)

- Create **LogUserActivity** helper method

```cs
namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var username = resultContext.HttpContext.User.GetUsername();
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByUsernameAsync(username);
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}
```

- Add to ApplicationServiceExtensions `services.AddScoped<LogUserActivity>();`
- Filter through adding to BaseApiController `[ServiceFilter(typeof(LogUserActivity))]`

### Making the LastActive action filter more optimal

- In **TokenService** change setting claim method in create token function

```cs
public string CreateToken(AppUser user)
{
    var claims = new List<Claim>
		{
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
        };
//...
}
```

- Add **GetUserId** method to **ClaimsPrincipleExtensions**

```cs
public static int GetUserId(this ClaimsPrincipal user)
{
    return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
}
```

- In **LogUserActivity** change username to userId  
  `var userId = resultContext.HttpContext.User.GetUserId();`
  and change GetUserByUsernameAsync to ById
  `var user = await repo.GetUserByIdAsync(userId);`

### Sorting on client

- Import **ButtonsModule** from ngx-bootstrap in **shared.module.ts**
- Add **orderBy** field to **userParams.ts** and add it to get members `params = params.append('gender', userParams.gender);`
- Add sorting to view member-list

```html
<div class="col">
  <div class="btn-group float-right">
    <button
      type="button"
      class="btn btn-primary"
      name="orderBy"
      (click)="loadMembers()"
      btnRadio="lastActive"
      [(ngModel)]="userParams.orderBy"
    >
      Last Active
    </button>
    <button
      type="button"
      class="btn btn-primary"
      name="orderBy"
      (click)="loadMembers()"
      btnRadio="created"
      [(ngModel)]="userParams.orderBy"
    >
      Newest Members
    </button>
  </div>
</div>
```

### Formatting Dates

- Install **ngx-timeago** with npm and import, export in **shared.module.ts**
- member-detail view

```html
<div>
  <strong>Last Active:</strong>
  <p>{{ member.lastActive | timeago }}</p>
</div>
<div>
  <strong>Member since:</strong>
  <p>{{ member.created | date: "dd MMM yyyy" }}</p>
</div>
```

### Restoring the caching for members

- Create a **memberCache** map in **members.service** and implement caching in getMembers

```ts
export class MembersService {
//...
  userParams: UserParams;
//...
  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) { return of(response); }

    let params = this.getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params)
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))
  }
```

### Restoring caching for member detailed

- Update **getMember** method in **members.service**

```ts
getMember(username: string) {
    const member = [...this.memberCache.values()]
    .reduce((arr, elem) => arr.concat(elem.result), [])
    .find((member: Member) => member.username === username);

    if (member) {
        return of(member);
    }
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
}
```

### Remembering the filters for a user in the service

- Inject **accountService** into **members.service** and move the functionality of the member-list constructor to the members.service's

```ts
export class MembersService {
    //...
    user: User;
    userParams: UserParams;

    constructor(private http: HttpClient, private accountService: AccountService) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
            this.user = user;
            this.userParams = new UserParams(user);
        })
    }
    getUserParams() {
        return this.userParams;
    }
    setUserParams(params: UserParams) {
        this.userParams = params;
    }
    resetUserParams() {
        this.userParams = new UserParams(this.user);
        return this.userParams;
    }
```

- Update **member-list.component.ts** accordingly

```ts
export class MemberListComponent implements OnInit {
  members: Member[];
  pagination: Pagination;
  userParams: UserParams;
  user: User;
  genderList = [
    { value: "male", display: "Males" },
    { value: "female", display: "Females" },
  ];
  constructor(private memberService: MembersService) {
    this.userParams = this.memberService.getUserParams();
  }
  ngOnInit(): void {
    this.loadMembers();
  }
  loadMembers() {
    this.memberService.setUserParams(this.userParams);
    this.memberService.getMembers(this.userParams).subscribe((response) => {
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }
  resetFilters() {
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }
  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams);
    this.loadMembers();
  }
}
```

## 13 - Like User Function

### Creating Likes Entity

- Create a join entity **UserLike.cs** for joining User to User

```cs
namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}
```

- Add relational fields to AppUser `public ICollection<UserLike> LikedByUsers { get; set; }` `public ICollection<UserLike> LikedUsers { get; set; }`
- Add to DataContext and set relations in there

```cs
namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
```

- Run a migration `dotnet ef migrations add LikedEntityAdded`

### Creating Likes Repository

- Dto

```cs
namespace API.DTOs
{
    public class LikeDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public string PhotoUrl { get; set; }
        public string City { get; set; }
    }
}
```

- Interface

```cs
namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUsersWithLikes(int userId);
        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
    }
}
```

- Add to ApplicationServiceExtensions `services.AddScoped<ILikesRepository, LikesRepository>();`

### Implementing Likes Repository

- Implement LikesRepository

```cs

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }
        public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();
            if (predicate == "liked")
            {
                likes = likes.Where(likes => likes.SourceUserId == userId);
                users = likes.Select(likes => likes.LikedUser);
            }
            if (predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);
            }
            return await users.Select(user => new LikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
```

### Controller methods for the likes feature

```cs
namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        {
            var users = await _likesRepository.GetUserLikes(predicate, User.GetUserId());

            return Ok(users);
        }
    }
}
```

### Likes function on angular app (client)

- **members.service.ts**

```ts
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {})
  }
  getLikes(predicate: string) {
    return this.http.get<Partial<Member[]>>(this.baseUrl + 'likes?predicate=' + predicate);
  }
```

- **member-card.component.ts**

```ts
export class MemberCardComponent implements OnInit {
  @Input() member: Member;

  constructor(
    private memberService: MembersService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}
  addLike(member: Member) {
    this.memberService.addLike(member.username).subscribe(() => {
      this.toastr.success("You have liked " + member.knownAs);
    });
  }
}
```

- **member-card.component.html**

```ht
<li class="list-inline-item">
    <button (click)="addLike(member)" class="btn btn-primary">
        <i class="fa fa-heart"></i>
    </button>
</li>
```

- Get error message from api to show in toastr. **in error.interceptor.ts**

```ts
case 400:
              if (error.error.errors) {
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key])
                  }
                }
                throw modalStateErrors.flat();
              } else if (typeof (error.error) === 'object') { // here
                this.toastr.error(error.statusText, error.status);
              } else {									// and here
                this.toastr.error(error.error, error.status)
              }
//....
```

<<<<<<< HEAD

### Adding the likes component

- **list.component.ts**

```ts
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = "liked";
  constructor(private memberService: MembersService) {}
  ngOnInit(): void {
    this.loadLikes();
  }
  loadLikes() {
    this.memberService.getLikes(this.predicate).subscribe((response) => {
      this.members = response;
    });
  }
}
```

- View

```html
<div class="text-center mt-3">
  <h2>
    {{ predicate === "liked" ? "Members I like" : "Members who like me" }}
  </h2>
</div>

<div class="container mt-3">
  <div class="row">
    <div class="btn-group" name="predicate">
      <button
        class="btn btn-primary"
        btnRadio="liked"
        [(ngModel)]="predicate"
        (click)="loadLikes()"
      >
        Members I like
      </button>
      <button
        class="btn btn-primary"
        btnRadio="likedBy"
        [(ngModel)]="predicate"
        (click)="loadLikes()"
      >
        Members who like me
      </button>
    </div>
  </div>
  <div class="row mt-3">
    <div class="col-2" *ngFor="let member of members">
      <app-member-card [member]="member"></app-member-card>
    </div>
  </div>
</div>
```

### Adding pagination for the likes (BE)

- Moved paginationParams from users to reusable **PaginationParams.cs** and inherit in **UserParams.cs**

```cs
namespace API.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
```

- **LikesParams.cs**

```cs
namespace API.Helpers
{
    public class LikesParams : PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}
```

- Update **Likes Repository interface GetUserLikes** method to return `Task<PagedList<LikeDto>>` .
  `Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);`
- Change **LikesRepository** GetUserLikes method

```cs
        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            IQueryable<AppUser> users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            IQueryable<UserLike> likes = _context.Likes.AsQueryable();

            if (likesParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }

            if (likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likedUsers = users.Select(user => new LikeDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            });

            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
        }
```

- Update **LikesController GetUserLikes**

```cs
[HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(users);
        }
```

### Pagination on client

- **members.service.ts**

```ts
  getLikes(predicate: string, pageNumber, pageSize) {
    let params = this.getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return this.getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params);
  }
```

- **list.component.ts**

```ts
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination;

  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadLikes(this.predicate);
  }

  loadLikes(s: string) {
    this.memberService.getLikes(s, this.pageNumber, this.pageSize).subscribe(response => {
      this.members = response.result;
      this.pagination = response.pagination;
    })
  }

  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadLikes(this.predicate);
  }
}
```

- **list.component.html**

```html
<div class="text-center mt-3">
  <h2>
    {{ predicate === "liked" ? "Members I like" : "Members who like me" }}
  </h2>
</div>

<div class="container mt-3">
  <div class="row">
    <div class="btn-group" name="predicate">
      <button
        class="btn btn-primary"
        btnRadio="liked"
        [(ngModel)]="predicate"
        (click)="loadLikes('liked')"
      >
        Members I like
      </button>
      <button
        class="btn btn-primary"
        btnRadio="likedBy"
        [(ngModel)]="predicate"
        (click)="loadLikes('likedBy')"
      >
        Members who like me
      </button>
    </div>
  </div>

  <div class="row mt-3">
    <div class="col-2" *ngFor="let member of members">
      <app-member-card [member]="member"></app-member-card>
    </div>
  </div>
</div>

<div class="d-flex justify-content-center" *ngIf="pagination">
  <pagination
    [boundaryLinks]="true"
    [totalItems]="pagination.totalItems"
    [itemsPerPage]="pagination.itemsPerPage"
    [(ngModel)]="pagination.currentPage"
    (pageChanged)="pageChanged($event)"
    previousText="&lsaquo;"
    nextText="&rsaquo;"
    firstText="&laquo;"
    lastText="&raquo;"
  >
  </pagination>
</div>
```

## 14 - Messaging Feature

### Setting up the entitites for messaging

- Create message entity

```cs
namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.Now;
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
    }
}
```

- Add message fields to AppUser

```cs
public ICollection<Message> MessagesSent { get; set; }
public ICollection<Message> MessagesReceived { get; set; }
```

- Add to context

```cs
   public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //...
                        builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
```

- Add migration `dotnet ef migrations add MessageEntityAdded`

### Setting up message repository

- Create Dto

```cs
namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}
```

- Create interface and implement it

```cs
namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser();
        Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId);
        Task<bool> SaveAllAsync();
    }
}
```

```cs
namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        public MessageRepository(DataContext context)
        {
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public Task<PagedList<MessageDto>> GetMessagesForUser(){throw new System.NotImplementedException();}
        public Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId){throw new System.NotImplementedException();}

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
```

- Add to ApplicationServiceExtensions  `services.AddScoped<IMessageRepository, MessageRepository>();`

### Setting up automapper profiles

- **AutoMapperProfiles.cs**

```cs
 CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl, memberOptions => memberOptions.MapFrom(src =>
                    src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, memberOptions => memberOptions.MapFrom(src =>
                    src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
```

- Created **CreateMessageDTO**

```cs
namespace API.DTOs
{
    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
    }
}
```

### Creating MessageController

```cs
namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }
    }
}
```

### Message Controller

```cs
namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }
    }
}
```

### Getting the messages from the repo

- Created **MessageParams**

```cs
namespace API.Helpers
{
    public class MessageParams : PaginationParams
    {
        public string Username { get; set; }
        public string Container { get; set; } = "Undefined";
    }
}
```

- Updated **GetMessagesForUser in Message Repository Interface**  `Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);`
- Implementation in **MessageRepository**

```cs
public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
{
    var query = _context.Messages
        .OrderByDescending(m => m.MessageSent)
        .AsQueryable();
    query = messageParams.Container switch
    {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username),
            _ => query.Where(u => u.Recipient.UserName == messageParams.Username && u.DateRead == null)
    };
    var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
    return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
}
```

- Endpoint in controller

```cs
[HttpGet]
public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
{
    messageParams.Username = User.GetUsername();
    var messages = await _messageRepository.GetMessagesForUser(messageParams);
    Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);
    return messages;
}
```

### Getting the message thread between two users

- In interface `Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);`
- Implementation 

```cs
 public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.Recipient.UserName == currentUsername
                            && m.Sender.UserName == recipientUsername
                            || m.Recipient.UserName == recipientUsername
                            && m.Sender.UserName == currentUsername
                            )
                            .OrderBy(m => m.MessageSent)
                            .ToListAsync();
            var unreadMessages = messages
                .Where(m => m.DateRead == null
                    && m.Recipient.UserName == currentUsername).ToList();
            if (unreadMessages.Any()){
                foreach (var message in unreadMessages){
                    message.DateRead = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
```

- Controller Endpoint

```cs
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }
```

### Setting up the Angular app for messaging

- create message model

```ts
export interface Message {
    id: number;
    senderId: number;
    senderUsername: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientUsername: string;
    recipientPhotoUrl: string;
    content: string;
    dateRead: Date;
    messageSent: Date;
}
```

- generate message service in _services

```ts
export class MessageService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMessages(pageNumber, pageSize, container) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http);
  }
}
```

- moved and configured common functions from members service to **paginationHelper.ts** file to use in message service also.

```ts
export function getPaginatedResult<T>(url, params, http: HttpClient) {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  return http.get<T>(url, { observe: 'response', params }).pipe(
    map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') !== null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}

export function getPaginationHeaders(pageNumber: number, pageSize: number) {
  let params = new HttpParams();

  params = params.append('pageNumber', pageNumber.toString());
  params = params.append('pageSize', pageSize.toString());

  return params;
}
```

- members service updates

```ts
export class MembersService {
 //...
  getMembers(userParams: UserParams) {
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize);

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http)
      .pipe(map(response => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      }))
  }
//...
  getLikes(predicate: string, pageNumber, pageSize) {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResult<Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);
  }
}
```

- Configured **messages.component.ts**

```ts
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
    
  constructor(private messageService: MessageService) { }
  
    ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages() {
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
    })
  }
  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadMessages();
  }
}
```

- And finally in view `<p *ngFor="let message of messages"><b>Message: </b>{{ message?.content }}</p>`

### Designing the inbox

```html
<div class="row mb-4">
  <div class="btn-group" name="container">
    <button
      class="btn btn-primary"
      btnRadio="Unread"
      [(ngModel)]="container"
      (click)="loadMessages()"
    >
      <i class="fa fa-envelope"></i> Unread
    </button>
    <button
      class="btn btn-primary"
      btnRadio="Inbox"
      [(ngModel)]="container"
      (click)="loadMessages()"
    >
      <i class="fa fa-envelope-open"></i> Inbox
    </button>
    <button
      class="btn btn-primary"
      btnRadio="Outbox"
      [(ngModel)]="container"
      (click)="loadMessages()"
    >
      <i class="fa fa-paper-plane"></i> Outbox
    </button>
  </div>
</div>

<div class="row" *ngIf="messages?.length === 0">
  <h3>No messages</h3>
</div>

<div class="row" *ngIf="messages?.length > 0">
  <table class="table table-hover" style="cursor: pointer">
    <tr>
      <th style="width: 40%">Message</th>
      <th style="width: 20%">From / To</th>
      <th style="width: 20%">Sent / Received</th>
      <th style="width: 20%"></th>
    </tr>
    <tr
      [hidden]="loading"
      *ngFor="let message of messages"
      routerLink="{{
        container === 'Outbox'
          ? '/members/' + message.recipientUsername
          : '/members/' + message.senderUsername
      }}"
    >
      <td>{{ message.content }}</td>
      <td>
        <div *ngIf="container === 'Outbox'">
          <img
            class="img-circle rounded-circle mr-2"
            src="{{ message.recipientPhotoUrl || './assets.user.png' }}"
            alt="{{ message.recipientUsername }}"
          />
          <strong>{{ message.recipientUsername | titlecase }}</strong>
        </div>
        <div *ngIf="container !== 'Outbox'">
          <img
            class="img-circle rounded-circle mr-2"
            src="{{ message.senderPhotoUrl || './assets.user.png' }}"
            alt="{{ message.senderUsername }}"
          />
          <strong>{{ message.senderUsername | titlecase }}</strong>
        </div>
      </td>
      <td>{{ message.messageSent | timeago }}</td>
      <td>
        <button
          class="btn btn-danger"
        >
          Delete
        </button>
      </td>
    </tr>
  </table>
</div>

<div class="d-flex justify-content-center" *ngIf="pagination && !loading">
  <pagination
    [boundaryLinks]="true"
    [totalItems]="pagination.totalItems"
    [itemsPerPage]="pagination.itemsPerPage"
    [(ngModel)]="pagination.currentPage"
    (pageChanged)="pageChanged($event)"
    previousText="&lsaquo;"
    nextText="&rsaquo;"
    firstText="&laquo;"
    lastText="&raquo;"
  >
  </pagination>
</div>
```

### Adding the message thread in the client

- In **message.service.ts** implement **getMessageThread** to make a get request to the API

```ts
  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }
```

- Generate **member-messages** component in /members

```ts
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[];
  @Input() username: string;
  messageContent: string;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {  }

  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(message => {
      this.messages.push(message);
      this.messageForm.reset();
    })
  }
}
```

```html
<p *ngFor="let message of messages">{{message?.content}}</p>
```

- And in member-detail component pass username parameter 

```html
 <app-member-messages [username]="member.username"></app-member-messages>
```

### Styling the message thread

- **member-messages.component.html**

```html
<div class="card">
  <div class="card-body">
    <div *ngIf="messages?.length === 0">
      No messages yet... say hi by using the message box below
    </div>
    <ul *ngIf="messages?.length > 0" class="chat">
      <li *ngFor="let message of messages">
        <div>
          <span class="chat-img float-right">
            <img class="rounded-circle" src="{{ message.senderPhotoUrl || './assets/user.png' }}"
              alt="{{ message.senderUsername }}" />
          </span>
          <div class="chat-body">
            <div class="header">
              <small class="text-muted">
                <span class="fa fa-clock-o">{{
                  message.messageSent | timeago
                }}</span>
                <span class="text-danger" *ngIf="
                    !message.dateRead && message.senderUsername !== username
                  ">
                  (unread)
                </span>
                <span class="text-success" *ngIf="
                    message.dateRead && message.senderUsername !== username
                  ">
                  (read {{ message.dateRead | timeago }})
                </span>
              </small>
            </div>
            <p>{{ message.content }}</p>
          </div>
        </div>
      </li>
    </ul>
  </div>

  <div class="card-footer">
    <form #messageForm="ngForm" (ngSubmit)="sendMessage()" autocomplete="off">
      <div class="input-group">
        <input name="messageContent" required [(ngModel)]="messageContent" type="text" class="form-control input-sm"
          placeholder="Send a private message" />
        <div class="input-group-append">
          <button [disabled]="!messageForm.valid" class="btn btn-primary" type="submit">
            Send
          </button>
        </div>
      </div>
    </form>
  </div>
</div>

```

- css

```css
.card {
  border: none;
}
.chat {
  list-style: none;
  margin: 0;
  padding: 0;
}
.chat li {
  margin-bottom: 10px;
  padding-bottom: 10px;
  border-bottom: 1px dotted #B3A9A9;
}
.rounded-circle {
  max-height: 50px;
}
```

### Activating the message tab

- add template reference variable to **member-detail.component** html and ts

```html
  <div class="col-8">
    <tabset class="member-tabset" #memberTabs>
      <!---///...-->
```

```ts
export class MemberDetailComponent implements OnInit {
    //...
    @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
    //...
    activeTab: TabDirective;
    messages:Message[]=[]; // also add this initialization to messages.component

    //...
      loadMessages() {
    this.messageService.getMessageThread(this.member.username).subscribe(messages => {
      this.messages = messages;
    })
  }   
    onTabActivated(data: TabDirective) {
        this.activeTab = data;
        if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
            this.loadMessages();
        }
    }
}
```

- cleanup **member-messages.component** and pass message input

```ts
export class MemberMessagesComponent implements OnInit {
  @Input() messages: Message[];

  constructor() { }

  ngOnInit(): void { }
}
```

- pass messages as input in **member-detail.component.html**

```html
<tab heading="Messages">
    <app-member-messages [messages]="messages"></app-member-messages>
</tab>
```

- Add (selectTab)="onTabActivated($event)" to tab headings like  **`<tab heading="Interests" (selectTab)="onTabActivated($event)">`**

### Using query params

- Add selectTab method to **member-detail.component** to access message tab through other places

```cs
selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
}
```

- Add click event to **member-detail** message button

```html
<button (click)="selectTab(3)" class="btn btn-success">
    Message
</button>
```

- Access through member-list or other places. Add router link to **member-card.component.html**

```html
<button
        routerLink="/members/{{ member.username }}"
        [queryParams]="{ tab: 3 }"
        class="btn btn-primary"
        >
    <i class="fa fa-envelope"></i>
</button>
```

- Add queryParams to **messages.component.html** make redirection from inbox to messages tab too

```html
    <tr
      [hidden]="loading"
      *ngFor="let message of messages"
      routerLink="{{
        container === 'Outbox'
          ? '/members/' + message.recipientUsername
          : '/members/' + message.senderUsername
      }}"
      [queryParams]="{ tab: 3 }"
    >
```

- **member-detail.component.ts**

```ts
ngOnInit(): void {
    //...
    this.route.queryParams.subscribe(params => {
    params.tab ? this.selectTab(params.tab) : this.selectTab(0);
})
//...
}
```

- But it doesn't work right away because these tabs are in a ngIf conditional and undefined in this state.
- We need to make ViewChild membertabs static

```ts
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
```

- Still not working. only if we **remove ngIf** it becomes usable but it creates whole kinds of other problems. Lets solve it via using route resolvers in the next chapter.

### Using route resolvers

- Create a _resolvers directory and create a resolver named **member-detailed.resolver.ts**

```ts
@Injectable({
    providedIn: 'root'
})
export class MemberDetailedResolver implements Resolve<Member> {

    constructor(private memberService: MembersService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<Member> {
        return this.memberService.getMember(route.paramMap.get('username'));
    }
}
```

- Update **app-routing.module**

```ts
 { path: 'members/:username', component: MemberDetailComponent, resolve: { member: MemberDetailedResolver } },
```

- In member-detail change loading members to getting from root. remove loadMember from member-detail

```ts
ngOnInit(): void {
    this.route.data.subscribe(data => {
    this.member = data.member;
})
//...
this.galleryImages = this.getImages();
}
```

### Sending messages

- Add **sendMessage** to **message.service**

```ts
  sendMessage(username: string, content: string) {
    return this.http.post<Message>(this.baseUrl + 'messages', { recipientUsername: username, content })
  }
```

- Add **sendMessage** to **member-messages.component**

```ts
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm;
  @Input() messages: Message[];
  @Input() username: string;
  messageContent: string;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void { }

  sendMessage() {
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(message => {
      this.messages.push(message);
      this.messageForm.reset();
    })
  }
}
```

- pass input username in **member-detail** view 

```html
<app-member-messages [messages]="messages" [username]="member.username"></app-member-messages>
```

- **member-message.component** messaging form

```html
<div class="card-footer">
    <form #messageForm="ngForm" (ngSubmit)="sendMessage()" autocomplete="off">
      <div class="input-group">
        <input
          name="messageContent"
          required
          [(ngModel)]="messageContent"
          type="text"
          class="form-control input-sm"
          placeholder="Send a private message"
        />
        <div class="input-group-append">
          <button
            [disabled]="!messageForm.valid"
            class="btn btn-primary"
            type="submit"
          >Send
          </button>
        </div>
      </div>
    </form>
  </div>
```

### Fixing the photo weirdness

- **messages.component.ts**

```ts
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading = false;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    this.loading = true;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
      this.loading = false;
    })
  }
//...
}
```

- **messages.component.html** add `[hidden]="loading"` to messages for loop

### Deleting messages on the API

- Create endpoint in **MessagesController.cs**

```cs
[HttpDelete("{id}")]
public async Task<ActionResult> DeleteMessage(int id)
{
    var username = User.GetUsername();

    var message = await _messageRepository.GetMessage(id);

    if (message.Sender.UserName != username && message.Recipient.UserName != username)
        return Unauthorized();

    if (message.Sender.UserName == username) message.SenderDeleted = true;

    if (message.Recipient.UserName == username) message.RecipientDeleted = true;

    if (message.SenderDeleted && message.RecipientDeleted)
        _messageRepository.DeleteMessage(message);

    if (await _messageRepository.SaveAllAsync()) return Ok();

    return BadRequest("Problem deleting the message");
}
```

- Update **MessageRepository.cs** to not display messages if both sender and receiever deleted the message

```cs
public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
{
    var query = _context.Messages
        .OrderByDescending(m => m.MessageSent)
        .AsQueryable();

    query = messageParams.Container switch
    {
        "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username
       		 && u.RecipientDeleted == false),
        "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username
       		 && u.SenderDeleted == false),
        _ => query.Where(u => u.Recipient.UserName ==
        	messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
    };

    var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

    return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
}
//...
public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername,
                                                            string recipientUsername)
{
    var messages = await _context.Messages
        .Include(u => u.Sender).ThenInclude(p => p.Photos)
        .Include(u => u.Recipient).ThenInclude(p => p.Photos)
        .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
               && m.Sender.UserName == recipientUsername
               || m.Recipient.UserName == recipientUsername
               && m.Sender.UserName == currentUsername && m.SenderDeleted == false
              )
        .OrderBy(m => m.MessageSent)
        .ToListAsync();

    var unreadMessages = messages.Where(m => m.DateRead == null
                                        && m.Recipient.UserName == currentUsername).ToList();

    if (unreadMessages.Any())
    {
        foreach (var message in unreadMessages)
        {
            message.DateRead = DateTime.Now;
        }

        await _context.SaveChangesAsync();
    }

    return _mapper.Map<IEnumerable<MessageDto>>(messages);
}
```

### Deleting messages on the client

- **deleteMessage** on **message.service**

```ts
deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
}
```

- **deleteMessage** on **messages.component**

```cs
deleteMessage(id: number) {
    this.messageService.deleteMessage(id).subscribe(() => {
        this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
    })
}
```

- Add click event for the delete button

```html
<button
        (click)="$event.stopPropagation()"
        (click)="deleteMessage(message.id)"
        class="btn btn-danger"
        >  
    Delete
</button>
```

- Update MessageRepository GetMessage to include recipient and recipient

```cs
public async Task<Message> GetMessage(int id)
{
    return await _context.Messages
        .Include(u => u.Sender)
        .Include(u => u.Recipient)
        .SingleOrDefaultAsync(x => x.Id == id);
}
```

## 15 - Identity and Role Management

### Setting up the entities

- Make **AppUser** inherit from `IdentityUser<int>` and remove id, pass salt and pass hash because from now on they are coming from **IdentityUser** of .Net
- Created **AppRole** and **AppUserRole** classes for relations

```cs
namespace API.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
```

### Configuring the DbContext

- Change DbContext to IdentityDbContext on DataContext of entity framework and install Microsoft.AspNetCore.Identity.EntityFrameworkCore package from nuget
- DataContext implements 

```cs
IdentityDbContext<AppUser, AppRole, int,
     IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
      IdentityRoleClaim<int>, IdentityUserToken<int>>
```

- Removed users DbSet because Identity got it
- Relations of AppUser and AppRole

```cs
builder.Entity<AppUser>()
    .HasMany(ur => ur.UserRoles)
    .WithOne(u => u.User)
    .HasForeignKey(ur => ur.UserId)
    .IsRequired();

builder.Entity<AppRole>()
    .HasMany(ur => ur.UserRoles)
    .WithOne(u => u.Role)
    .HasForeignKey(ur => ur.RoleId)
    .IsRequired();
```

- Configuring the Startup class
- In IdentityServiceExtensions

```cs
services.AddIdentityCore<AppUser>(opt =>{opt.Password.RequireNonAlphanumeric = false;}).AddRoles<AppRole>().AddRoleManager<RoleManager<AppRole>>().AddSignInManager<SignInManager<AppUser>>().AddRoleValidator<RoleValidator<AppRole>>().AddEntityFrameworkStores<DataContext>();
```

### Refactoring and adding new migration

- Added migration `dotnet ef migrations add IdentityAdded`
- Commented seed command on Program.cs because it is outdated and doesn't match with our current database
- run the app and check the migrated results

### Updating the Seed method

- Update Seed.cs

```cs
public static async Task SeedUsers(UserManager<AppUser> userManager)
{
    if (await userManager.Users.AnyAsync()) return;
    var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
    var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
    if (users == null) return;
    foreach (var user in users)
    {
        user.UserName = user.UserName.ToLower();
        await userManager.CreateAsync(user, "Pa$$w0rd");
    }
```

- Update Program.cs

```cs
//...
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        try 
        {
            var context = services.GetRequiredService<DataContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            await context.Database.MigrateAsync();
            await Seed.SeedUsers(userManager);
        }
        catch (Exception ex)
            //...
            //...
```

- Drop db
- dotnet run to apply changes and migration

### Updating the Account Controller

- AccountController.cs

```cs
public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IMapper _mapper;
    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.Username.ToLower();

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid username");

        var result = await _signInManager
            .CheckPasswordSignInAsync(user, loginDto.Password, false);

        if (!result.Succeeded) return Unauthorized();

        return new UserDto
        {
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
```

### Adding roles to the app

- Update Seed to have roles

```cs
public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
{
    if (await userManager.Users.AnyAsync()) return;

    var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
    var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
    if (users == null) return;

    var roles = new List<AppRole>
    {
        new AppRole {Name = "Member"},
        new AppRole {Name = "Admin"},
        new AppRole {Name = "Moderator"}
    };

    foreach (var role in roles){await roleManager.CreateAsync(role);}

    foreach (var user in users)
    {
        user.UserName = user.UserName.ToLower();
        await userManager.CreateAsync(user, "Pa$$w0rd");
        await userManager.AddToRoleAsync(user, "Member");
    }

    var admin = new AppUser{UserName = "admin"};

    await userManager.CreateAsync(admin, "Pa$$w0rd");
    await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
}
```

- Create a instance of RoleManager like UserManager and add to seed methods parameters
- Drop db and run seed again

### Adding the roles to the JWT token

- Token Service

```cs
public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SymmetricSecurityKey _key; 
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public async Task<string> CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role=>new Claim(ClaimTypes.Role,role)));
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
```

- Add await to Token part of AccountController and also add roles at register to users

```cs
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
            var user = _mapper.Map<AppUser>(registerDto);
            user.UserName = registerDto.Username.ToLower();
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);
            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());
            if (user == null) return Unauthorized("Invalid username");
            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized();
            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
```

### Adding policy based Authorization

- Create AdminController for demonstration

```cs
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public ActionResult GetUsersWithRoles()=>return Ok("Only admins can see this");
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()=>return Ok("Admins or moderators can see this");
}
```

- Fill Policies in IdentityServiceExtensions by adding AddAuthorization service

```cs
services.AddAuthorization(opt =>
{
    opt.AddPolicy("RequireAdminRole",policy=>policy.RequireRole("Admin"));
    opt.AddPolicy("ModeratePhotoRole",policy=>policy.RequireRole("Admin","Moderator"));
});
```

- We get the expected results with the roles we set

### Getting the users with roles

- AdminController

```cs
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
 
    public AdminController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
 
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .Include(r => r.UserRoles)
            .ThenInclude(r => r.Role)
            .OrderBy(u => u.UserName)
            .Select(u=>new
            {
                u.Id,
                Username=u.UserName,
                Roles=u.UserRoles.Select(r=>r.Role.Name).ToList()
            })
            .ToListAsync();
        
        return Ok("Only admins can see this");
    }    
    
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}
```

### Editing User Roles

- Added functionality to AdminController

```cs
[Route("api/[controller]")]
  [ApiController]
  public class AdminController : ControllerBase
  {
      private readonly UserManager<AppUser> _userManager;
      public AdminController(UserManager<AppUser> userManager)
      {
          _userManager = userManager;
      }
      [Authorize(Policy = "RequireAdminRole")]
      [HttpGet("users-with-roles")]
      public async Task<ActionResult> GetUsersWithRoles()
      {
          var users = await _userManager.Users
              .Include(r => r.UserRoles)
              .ThenInclude(r => r.Role)
              .OrderBy(u => u.UserName)
              .Select(u=>new
              {
                  u.Id,
                  Username=u.UserName,
                  Roles=u.UserRoles.Select(r=>r.Role.Name).ToList()
              })
              .ToListAsync();
          return Ok(users);
      }
      [HttpPost("edit-roles/{username}")]
      public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
      {
          var selectedRoles = roles.Split(",").ToArray();
          var user = await _userManager.FindByNameAsync(username);
          if (user == null) return NotFound("Could not find user");
          var userRoles = await _userManager.GetRolesAsync(user);
          var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
          if (!result.Succeeded) return BadRequest("Failed to add to roles");
          result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
          if (!result.Succeeded) return BadRequest("Failed to remove from roles");
          return Ok(await _userManager.GetRolesAsync(user));
      }
      //...
```

### Adding an admin component (FE)

- Created Admin component in src/app/admin
- Add to routing    `{ path: 'admin', component: AdminPanelComponent },`
- Create link in nav.html

### Adding an admin guard

- Created guard in **src/app/_guards** => `ng g g admin --skip-tests`

```ts
export class AdminGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr: ToastrService) {

  }

  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user.roles.includes('Admin') || user.roles.includes('Moderator')) {
          return true;
        }
        this.toastr.error('You cannot enter this page');
      }))
  }
}
```

- Add  `roles: string[];` to **user.ts** model
- Add guard to routing of admin    `{ path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard] },`
- Create getDecodedToken function and update setCurrentUser function

```ts
//...
  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }
//...
  getDecodedToken(token) {
    return JSON.parse(atob(token.split('.')[1]));
  }
```

### Adding a custom directive

```bash
cd src/app;
mkdir _directives; cd _directives
ng g d has-role --skip-tests
```

- **has-role.directive.ts**

```ts
export class HasRoleDirective implements OnInit {
    @Input() appHasRole: string[];
    user: User;
    constructor(private viewContainerRef: ViewContainerRef,
                 private templateRef: TemplateRef<any>,
                 private accountService: AccountService) {
        this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
            this.user = user;
        })
    }
    ngOnInit(): void {
        if (!this.user?.roles || this.user == null) {
            this.viewContainerRef.clear();
        return;
    }
    if (this.user?.roles.some(r => this.appHasRole.includes(r))) {
        this.viewContainerRef.createEmbeddedView(this.templateRef);} else {this.viewContainerRef.clear();}}}
```

- Navbar admin button

```html
<li *appHasRole="['Admin', 'Moderator']" class="nav-item">
    <a class="nav-link" routerLink="/admin" routerLinkActive="active">Admin</a>
</li>
```

### Adding the edit roles component

- Create user-management component in /admin
- Add Admin panel features photo management and role management but photo only for admin

```html
<h2>Admin Panel</h2>
<div class="tab-panel">
  <tabset class="member-tabset">
    <tab heading="User management" *appHasRole="['Admin']">
      <div class="container">
        <app-user-management></app-user-management>
      </div>
    </tab>
    <tab heading="Photo management" *appHasRole="['Admin', 'Moderator']">
      <div class="container">
        <app-photo-management></app-photo-management>
      </div>
    </tab>
  </tabset>
</div>
```

- Create admin service in /_services

```ts
export class AdminService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }
  getUsersWithRoles() {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles')
  }
}
```

- user-management component

```ts
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;

  constructor(private adminService: AdminService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users
    });
  }
}
```

- User management template

```html
<div class="row">
  <table class="table">
    <tr>
      <th>Username</th>
      <th>Active Roles</th>
      <th></th>
    </tr>
    <tr *ngFor="let user of users">
      <td>{{ user.username }}</td>
      <td>{{ user.roles }}</td>
      <td><button class="btn btn-info">Edit Roles</button></td>
    </tr>
  </table>
</div>
```

### Setting up modals

- Import Modal Module from ngx-bootstrap in shared module

```ts
import { ModalModule } from 'ngx-bootstrap/modal';
@NgModule({
  declarations: [],
  imports: [// ...
    ModalModule.forRoot()
  ],
  exports: [// ...
    ModalModule
  ]
})
```

- `mkdir modals; ng g c roles-modal --skip-tests` 
- User management component

```ts
export class UserManagementComponent implements OnInit {
    //..
    bsModalRef: BsModalRef;

    constructor(private adminService: AdminService, private modalService: BsModalService) { }
    //...
    openRolesModal() {
        const initialState = {
            list: [
                'Open a modal with component',
                "Pass your data",
                "Do something else",
                "..."
            ],
            title: 'Modal with component'
        };
        this.bsModalRef = this.modalService.show(RolesModalComponent, { initialState });
        this.bsModalRef.content.closeBtnName = 'Close';
    }
}
```

- User management template

```html
<div class="row">
  <table class="table">
    <tr>
      <th>Username</th>
      <th>Active Roles</th>
      <th></th>
    </tr>
    <tr *ngFor="let user of users">
      <td>{{ user.username }}</td>
      <td>{{ user.roles }}</td>
      <td>
        <button (click)="openRolesModal()" class="btn btn-info">
          Edit Roles
        </button>
      </td>
    </tr>
  </table>
</div>
```

- Roles modal component

```ts
import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  title: string;
  list: any[] = [];
  closeBtnName: string
  constructor(public bsModalRef: BsModalRef) { }
  ngOnInit(): void { }
}
```

- Roles modal template

```html
<div class="modal-header">
  <h4 class="modal-title pull-left">{{ title }}</h4>
  <button type="button" class="close pull-right" aria-label="Close" (click)="bsModalRef.hide()">
    <span aria-hidden="true">&times;</span>
  </button>
</div>
<div class="modal-body">
  <ul *ngIf="list.length">
    <li *ngFor="let item of list">{{ item }}</li>
  </ul>
</div>
<div class="modal-footer">
  <button type="button" class="btn btn-default" (click)="bsModalRef.hide()">
    {{ closeBtnName }}
  </button>
</div>
```

### Editing roles in the client

- Roles modal component

```ts
export class RolesModalComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter();
  user: User;
  roles: any[];

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  updateRoles() {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}
```

- Roles modal template

```html
<div class="modal-header">
  <h4 class="modal-title pull-left">Edit roles for {{ user.username }}</h4>
  <button
    type="button"
    class="close pull-right"
    aria-label="Close"
    (click)="bsModalRef.hide()"
  >
    <span aria-hidden="true">&times;</span>
  </button>
</div>
<div class="modal-body">
  <form #rolesForm="ngForm" id="rolesForm">
    <div class="form-check" *ngFor="let role of roles">
      <input
        type="checkbox"
        class="form-check-input"
        [checked]="role.checked"
        value="role.name"
        (change)="role.checked = !role.checked"
        [disabled]="role.name === 'Admin' && user.username === 'admin'"
      />
      <label>{{ role.name }}</label>
    </div>
  </form>
</div>
<div class="modal-footer">
  <button type="button" class="btn btn-default" (click)="bsModalRef.hide()">
    Cancel
  </button>
  <button type="button" class="btn btn-success" (click)="updateRoles()">
    Submit
  </button>
</div>
```

- Admin service

```ts
 export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<Partial<User[]>>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {});
  }
}
```

* User management component

```ts
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  bsModalRef: BsModalRef;

  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit(): void {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      this.users = users;
    })
  }

  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    }
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    this.bsModalRef.content.updateSelectedRoles.subscribe(values => {
      const rolesToUpdate = {
        roles: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => {
          user.roles = [...rolesToUpdate.roles]
        })
      }
    })
  }

  private getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Member' }
    ];

    availableRoles.forEach(role => {
      let isMatch = false;
      for (const userRole of userRoles) {
        if (role.name === userRole) {
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      if (!isMatch) {
        role.checked = false;
        roles.push(role);
      }
    })
    return roles;
  }
}
```

- User management template

```html
<div class="row">
  <table class="table">
    <tr>
      <th style="width: 30%">Username</th>
      <th style="width: 40%">Active roles</th>
      <th style="width: 30%"></th>
    </tr>
    <tr *ngFor="let user of users">
      <td>{{ user.username }}</td>
      <td>{{ user.roles }}</td>
      <td>
        <button (click)="openRolesModal(user)" class="btn btn-info">
          Edit Roles
        </button>
      </td>
    </tr>
  </table>
</div>
```

## 16 - SignalR

### Adding a presence hub (BE)

- Create new folder in API SignalR and create a class called PresenceHub

```cs
namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            await base.OnDisconnectedAsync(exception);
        }
    }
}
```

- Add SignalR service at startup configureServices and add enpoints mapHub to configure

```cs
public void ConfigureServices(IServiceCollection services)
{
	// ...
    services.AddSignalR();
}
public void Configure(IApplicationBuilder app)
{
    // ...
    app.UseEndpoints(endpoints =>
                     {
                         endpoints.MapControllers();
                         endpoints.MapHub<PresenceHub>("hubs/presence");
                     });
}
```

### Authentication to SignalR (BE)

- Add [**Authorize**] to **PresenceHub**
- Add event option to services in **IdentityServiceExtensions**

```cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>{options.TokenValidationParameters = new TokenValidationParameters{ValidateIssuerSigningKey = true,IssuerSigningKey = new(Encoding.UTF8.GetBytes(config["TokenKey"])), ValidateIssuer = false,ValidateAudience = false,};
 					// here
                      options.Events = new JwtBearerEvents {OnMessageReceived = context =>{
                              var accessToken = context.Request.Query["access_token"];
                              var path = context.HttpContext.Request.Path;
                              if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) context.Token = accessToken;
                              return Task.CompletedTask;
                          }};});
```

- Add **AllowCredentials()** to **UseCors** in **Startup** **Configure**

```cs
public void Configure(IApplicationBuilder app)
{
    // ...
    app.UseCors(x => x
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins("https://localhost:4200"));
    // ...
}
```

### Client Side SignalR (FE)

- Install SignalR via npm `npm install @microsoft/signalr` 
- Add **hubUrl** to **environment** and **environment.prod**
- generate **presence.service** in **_services** `ng g s presence --skip-tests`

```ts
export class PresenceService {
    hubUrl = environment.hubUrl;
    private hubConnection: HubConnection;
    constructor(private toastr: ToastrService) { }
    createHubConnection(user: User) {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl(this.hubUrl + 'presence', {
            accessTokenFactory: () => user.token
        })
            .withAutomaticReconnect()
            .build()
        this.hubConnection
            .start()
            .catch(error => console.log(error));
        this.hubConnection.on('UserIsOnline', username => {
            this.toastr.info(username + 'has connected');
        })
        this.hubConnection.on('UserIsOffline', username => {
            this.toastr.warning(username + 'has disconnected');
        })
    }
    stopHubConnection() {
        this.hubConnection.stop().catch(error => console.log(error));
    }
}
```

- Use service in **app.component.ts**

```ts
constructor(private accountService: AccountService, private presence: PresenceService) { }
ngOnInit() {this.setCurrentUser();}
setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user) {
        this.accountService.setCurrentUser(user);
        this.presence.createHubConnection(user); // signalr hub connection
    }}}
```

- Add create and stop hub connection functions to **account.service** 's login, register, logout methods

```ts
  constructor(private http: HttpClient, private presence: PresenceService) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);  // here
        }}))}
  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);  // here
        }}))}
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection(); // here
  }}
```

### Adding a presence tracker (BE)

- Presence Tracker

```cs
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = new();
        public Task UserConnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username)) OnlineUsers[username].Add(connectionId);
                else OnlineUsers.Add(username, new List<string> { connectionId });
            }
            return Task.CompletedTask;
        }
        public Task UserDisconnected(string username, string connectionId)
        {
            lock (OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.CompletedTask;

                OnlineUsers[username].Remove(connectionId);
                if (OnlineUsers[username].Count == 0) OnlineUsers.Remove(username);
            }

            return Task.CompletedTask;
        }
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
```

- Update **PresenceHub**

```cs
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        [Authorize]
        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
```

- Add to **ApplicationServiceExtensions** `services.AddSingleton<PresenceTracker>();`

### Displaying online presence (FE)

- Add BehaviourSubject to **presence.service** and an observable

```ts
export class PresenceService {
//...
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUsersSource.asObservable();
//...
  createHubConnection(user: User) {
//...
    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => {
      this.onlineUsersSource.next(usernames);
    })
  }
//...
}
```

- Inject **PresenceService** as presence in **member-card.component**. And in template add a span with being online condition

```html
<div class="card-body p-1">
    <h6 class="card-title text-center mb-1">
      <span [class.is-online]="(presence.onlineUsers$ | async).includes(member.username)">
        <i class="fa fa-heart mr-2"></i>
      </span>
      {{ member.knownAs }}, {{ member.age }}
    </h6>
    <p class="card-text text-muted text-center">{{ member.city }}</p>
  </div>
```

- Add a blinking effect to heart in CSS

```css
@keyframes fa-blink {
  0% {opacity: 1;}
  100% {opacity: 0.5;}
}
.is-online {
  animation: fa-blink 0.5s infinite;
  color: rgb(200, 20, 40)
}
```

- Also inject **PresenceService** as **presence** to **member-detail.component** and add Online Now info in **template**

```html
<div class="mb-2" *ngIf="(presence.onlineUsers$ | async).includes(member.username)">
    <i class="fa fa-user-circle text-success"></i>Online Now
</div>
```

### Creating a message hub (BE)

- Create **MessageHub** in SignalR folder

```cs
    public class MessageHub : Hub
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        public MessageHub(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }
        public override async Task OnDisconnectedAsync(Exception exception)=>await base.OnDisconnectedAsync(exception);
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
```

- Add Endpoint  `MapHub<MessageHub>("hubs/message")` in **Startup**

### Adding the send message method to the hub

- Copy SendMessage function from MessagesController to MessageHub and make some modifications

```cs
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.UserName, recipient.UserName);
                await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }
```

### Adding the hub connection to the message service (FE)

- Implement **CreateHubConnection** and **StopHubConnection** on **MessageService**

```ts
export class MessageService {
    baseUrl = environment.apiUrl;
    hubUrl = environment.hubUrl;
    private hubConnection: HubConnection;
    private messageThreadSource = new BehaviorSubject<Message[]>([]);
    messageThread$ = this.messageThreadSource.asObservable(); 

    constructor(private http: HttpClient) { }

    createHubConnection(user: User, otherUsername: string) {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl(this.hubUrl + 'message?user=' + otherUsername, { accessTokenFactory: () => user.token })
            .withAutomaticReconnect()
            .build();

        this.hubConnection.start().catch(error => console.log(error));

        this.hubConnection.on('ReceiveMessageThread', messages => {
            this.messageThreadSource.next(messages);
        });
    }

    stopHubConnection() {
        this.hubConnection.stop();
    }
    // ...
}
```

### Refactoring the message components to use the hub

- MessageService

```ts
// stop only if its initialized
stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }
```

- Member Detail Component

```ts
export class MemberDetailComponent implements OnInit, OnDestroy {
// ...
  user: User;
  constructor(   //...
    private accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }
// ...
  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.messages.length === 0) {
      this.messageService.createHubConnection(this.user, this.member.username);
    } else {
      this.messageService.stopHubConnection();
    }
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
```

- Member Detail Template

```html
<tab heading="Messages" (selectTab)="onTabActivated($event)">
    <app-member-messages [username]="member.username"></app-member-messages>
</tab>
```

- Made **messageService** Injection **public** in **member-messages.component.ts**
- Replaced **messages** variable with **(messageService.messageThread$ | async)** because it is an observable

### Sending messages via the hub

- **Created Hub connection** for **New Messages** and updated **send message** method in **message service**

```ts
createHubConnection(user: User, otherUsername: string) {
    //...
    this.hubConnection.on('NewMessage', message => {
        this.messageThread$.pipe(take(1)).subscribe(messages => {
            this.messageThreadSource.next([...messages, message])
        })})
}
async sendMessage(username: string, content: string) {
    return this.hubConnection.invoke('SendMessage', { recipientUsername: username, content })
        .catch(error => console.log(error));
}
```

### Tracking the message groups

- Create Group and Connection entities

```cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class Group
    {
        public Group(){}
        public Group(string name){Name = name;}
        [Key]
        public string Name { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
    public class Connection
    {
        public Connection(){}
        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }
        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}
```

- Added `public DbSet<Group> Groups { get; set; }  public DbSet<Connection> Connections { get; set; }` to DataContext
- Added New Methods to **Message Repository Interface** and implemented them in **MessageRepository**

```cs
public class MessageRepository : IMessageRepository
{
    //...
    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }
    //...
    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }
    //...
    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await _context.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }
    //...
    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
}
```

- Add new migration `dotnet ef migrations add GroupsAdded` and update database by running the API

### Updating the message hub with group tracking

- Update and create non existent **MessageHub**'s **AddToGroup**, **RemoveFromMessageGroup**, **GetGroupName**,**RemoveFromMessageGroup** and **OnConnectedAsync** methods

```cs
public class MessageHub : Hub
{
    //...
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToGroup(groupName);

        var messages = await _messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User.GetUsername();

        if (username == createMessageDto.RecipientUsername.ToLower())
            throw new HubException("You cannot send messages to yourself");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        if (recipient == null) throw new HubException("Not found user");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _messageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {

            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }
    }

    private async Task<bool> AddToGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
            _messageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        return await _messageRepository.SaveAllAsync();
    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await _messageRepository.GetConnection(Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        await _messageRepository.SaveAllAsync();
    }
    //...
}
```

### Dealing with UTC date formats

- Set **`DateTime.Now`** To **`DateTime.UtcNow`** in **`Message.cs`** and **`MessageRepository.cs`**
- In **AutoMapperProfiles** `CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));`

### Notifying users when they receive a message

- Add **GetConnectionsForUser** method to **PresenceTracker**

```cs
       public Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            lock (OnlineUsers)
            {
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }
            return Task.FromResult(connectionIds);
        }
```

- **MessageHub** inject presenceHub and presenceTracker and add an else condition to sendmessage function

```cs
public async Task SendMessage(CreateMessageDto createMessageDto)
{
    var username = Context.User.GetUsername();
    if (username == createMessageDto.RecipientUsername.ToLower()) throw new HubException("You cannot send messages to yourself");
    var sender = await _userRepository.GetUserByUsernameAsync(username);
    var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);
    if (recipient == null) throw new HubException("Not found user");
    var message = new Message
    {
        Sender = sender,
        Recipient = recipient,
        SenderUsername = sender.UserName,
        RecipientUsername = recipient.UserName,
        Content = createMessageDto.Content
    };
    var groupName = GetGroupName(sender.UserName, recipient.UserName);
    var group = await _messageRepository.GetMessageGroup(groupName);
    if (group.Connections.Any(x => x.Username == recipient.UserName))
    {
        message.DateRead = DateTime.UtcNow;
    }
    else //here
    {
        var connections = await _tracker.GetConnectionsForUser(recipient.UserName);
        if (connections != null)
        {
            await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = sender.UserName, knownAs = sender.KnownAs });
        }
    }
    _messageRepository.AddMessage(message);
    if (await _messageRepository.SaveAllAsync())
    {
        await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
    }
}
```

- New **hubConnection** on **presence.service** (inject router)

```ts
    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
      this.toastr.info(knownAs + ' has sent you a new message!')
        .onTap
        .pipe(take(1))
        .subscribe(() => this.router.navigateByUrl('/members/' + username + '?tab=3'));
    })
```

- In member-detail component inject router and in constructor add `this.router.routeReuseStrategy.shouldReuseRoute = () => false;`

### Optimizing the presence

- In **PresenceHub**

  - **OnConnectedAsync(...)** change Clients.All.SendAsync(....) to Clients.Caller.SendAsync(...)
  - **OnDisconnectedAsync(...)** remove var currentUsers and await Clients.All.SendAsync(...) lines

- In **PresenceTracker**

  - In **UserConnected**

    - ```cs
      public Task<bool> UserConnected(string username, string connectionId)
      {
          bool isOnline = false;
          lock (OnlineUsers)
          {
              if (OnlineUsers.ContainsKey(username))
              {
                  OnlineUsers[username].Add(connectionId);
              }
              else
              {
                  OnlineUsers.Add(username, new List<string> { connectionId });
                  isOnline = true;
              }
          }
          return Task.FromResult(isOnline);
      }
      ```

  - In **UserDisconnected**

    - ```cs
      public Task<bool> UserDisconnected(string username, string connectionId)
      {
          bool isOffline = false;
          lock (OnlineUsers)
          {
              if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);
      
              OnlineUsers[username].Remove(connectionId);
              if (OnlineUsers[username].Count == 0)
              {
                  OnlineUsers.Remove(username);
                  isOffline = true;
              }
          }
          return Task.FromResult(isOffline);
      }
      ```

- Back to **PresenceHub** to add **isOnline** checks

  - ```cs
    public override async Task OnConnectedAsync()
    {
        var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
        if (isOnline)
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
    
        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
    
        if (isOffline)
            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
    
        await base.OnDisconnectedAsync(exception);
    }
    ```

- **presence.service** change **UserIsOffline** and **UserIsOffline** hubConnections

  - ```ts
    this.hubConnection.on('UserIsOnline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
            this.onlineUsersSource.next([...usernames, username])
        })
    })
    
    this.hubConnection.on('UserIsOffline', username => {
        this.onlineUsers$.pipe(take(1)).subscribe(usernames => {
            this.onlineUsersSource.next([...usernames.filter(x => x !== username)])
        })
    })
    ```

### Optimizing the messages

- **MessageHub**

```cs
public override async Task OnConnectedAsync()
{
    var httpContext = Context.GetHttpContext();
    var otherUser = httpContext.Request.Query["user"].ToString();
    var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    var group = await AddToGroup(groupName);
    await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

    var messages = await _messageRepository.
        GetMessageThread(Context.User.GetUsername(), otherUser);

    await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
}
//...
private async Task<Group> AddToGroup(string groupName)
{
    var group = await _messageRepository.GetMessageGroup(groupName);
    var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
    if (group == null)
    {
        group = new Group(groupName);
        _messageRepository.AddGroup(group);
    }
    group.Connections.Add(connection);
    if (await _messageRepository.SaveAllAsync()) return group;
    throw new HubException("Failed to join group");
}
private async Task<Group> RemoveFromMessageGroup()
{
    var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
    var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
    _messageRepository.RemoveConnection(connection);
    if (await _messageRepository.SaveAllAsync()) return group;
    throw new HubException("Failed to remove from group");
}
```

- **MessageRepository (and Interface of it)**

```cs
//...       
public async Task<Group> GetGroupForConnection(string connectionId)
{
    return await _context.Groups
        .Include(c => c.Connections)
        .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
        .FirstOrDefaultAsync();
}
//...
public async Task<Group> GetMessageGroup(string groupName)
{
    return await _context.Groups
        .Include(x => x.Connections)
        .FirstOrDefaultAsync(x => x.Name == groupName);
}
```

- Create **group.ts** model in **_models** of **client**

```ts
export interface Group {
    name: string;
    connections: Connection[]
}
interface Connection {
    connectionId: string;
    username: string;
}
```

- **message.service** new **hubConnection** in **createHubConnection**

```ts
this.hubConnection.on('UpdatedGroup', (group: Group) => {
    if (group.connections.some(x => x.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe(messages => {
            messages.forEach(message => {
                if (!message.dateRead) {
                    message.dateRead = new Date(Date.now())
                }
            })
            this.messageThreadSource.next([...messages]);
        })
    }
})
```

## 17 - Unit of Work Pattern

### Implementing the unit of work

- Create a new Interface called **IUnitOfWork** and make implementations in **UnitOfWork** in Data folder

```cs
namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context);
        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
```

- In ApplicationServiceExtensions remove repositories and add `services.AddScoped<IUnitOfWork, UnitOfWork>();`
- Remove **SaveAllAsync()** from **MessageRepository** and **UserRepository** (also interfaces of them)

### Refactoring the controllers to use the unit of work

- **LikesController** update constructor and change all `_userRepository`,`_likesRepository` to `_unitOfWork.UserRepository`, `_unitOfWork.LikesRepository`  `_...Repository.SaveAllAsync()` to `_unitOfWork.Complete()`

```cs
public class LikesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    public LikesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    //...............
    // other changes
    //...............
}
```

- In **MessagesController** 
  - we could remove **GetMessageThread** because we got this functionality on our **SignalR** hub
  - Update constructor like we did in LikesController but plus IMapper and same for `_...Repository` to `_unitOfWork....Repository` 

```cs
public class MessagesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    //...............
    // other changes
    //...............
}
```

- Same with **UsersController** but with **photoService**

```cs
public class UsersController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork unitOfWork, IMapper mapper,
                           IPhotoService photoService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _photoService = photoService;
    }   
    //...............
    // other changes
    //...............
}      
```

- same for **MessageHub**

```cs
public class MessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _tracker;
    public MessageHub(IMapper mapper, IUnitOfWork unitOfWork, IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
    {
        _tracker = tracker;
        _presenceHub = presenceHub;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    //...............
    // other changes
    //...............
}
```

- **LogUserActivity**

```cs
public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();
        var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
        var user = await uow.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;
        await uow.Complete();
    }
}
```

- Remove **GetMessageThread's** **SaveChanges** and go to **MessageHub's** **OnConnectedAsync** add  `if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();` before **SendAsync**

### Optimizing queries 1

- **MessageRepository**

```cs
public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername,string recipientUsername)
{
    var messages = await _context.Messages
        .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
               && m.Sender.UserName == recipientUsername
               || m.Recipient.UserName == recipientUsername
               && m.Sender.UserName == currentUsername && m.SenderDeleted == false
              )
        .OrderBy(m => m.MessageSent)
        .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    var unreadMessages = messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();
    if (unreadMessages.Any())
    {
        foreach (var message in unreadMessages)
        {
            message.DateRead = DateTime.UtcNow;
        }
    }
    return messages;
}
//...
//Also added RecipientDeteled, SenderDeleted to MessageDto with [JsonIgnore] annotations
public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
{
    var query = _context.Messages
        .OrderByDescending(m => m.MessageSent)
        .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
        .AsQueryable();
    query = messageParams.Container switch
    {
            "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false),
            "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
            _ => query.Where(u => u.RecipientUsername == messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
    };
    return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
}
```

### Fixing UTC dates again

- Used a prewritten code on GitHub

```cs
namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        //...
        builder.ApplyUtcDateTimeConverter();
    }
}
public static class UtcDateAnnotation
{
    private const String IsUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
        new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
        new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
        builder.HasAnnotation(IsUtcAnnotation, isUtc);

    public static Boolean IsUtc(this IMutableProperty property) =>
        ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsUtc()) continue;
                if (property.ClrType == typeof(DateTime)) property.SetValueConverter(UtcConverter);
                if (property.ClrType == typeof(DateTime?)) property.SetValueConverter(UtcNullableConverter);
            }
        }
    }
}
```

### Optimizing queries 2

- Update **GetUsers** endpoint on **UsersController**

```cs
[HttpGet]
public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
{
    var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
    userParams.CurrentUsername = User.GetUsername();

    if (string.IsNullOrEmpty(userParams.Gender))
        userParams.Gender = gender == "male" ? "female" : "male";

    var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

    Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
                                 users.TotalCount, users.TotalPages);

    return Ok(users);
}
```

- Add **GetUserGender** to **UserRepository** (and interface)

```cs
public async Task<string> GetUserGender(string username)
{
    return await _context.Users
        .Where(x => x.UserName == username)
        .Select(x => x.Gender).FirstOrDefaultAsync();
}
```



### Adding a confirmation service to the angular app

### Getting the data from the confirmation modal

### Tidying up the member message component

### ???

## 18 - Publishing

