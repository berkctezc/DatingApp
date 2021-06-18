# NET 5.0 & Angular Project

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
INSERT INTO Users (Id, UserName) VALUES (1,"Bob");
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
    return _context.Users.ToList();
}

// api/users/1
[HttpGet("{id}")]
public ActionResult<AppUser> GetUsers(int id)
{
    return _context.Users.Find(id);
}
```

### Making our code Async

- syntax: `async Task<YOURRETURNTYPE> YourAction` and return await ....ToListAsync(),FindAsync....

- example:

  ```cs
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
  {
      return await _context.Users.ToListAsync();
  }
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

* Use in Controller

## 8 - Building the User Interface

- Created interfaces for member and photo

* Create member service in _services **`ng g s members --skip-tests`**

```ts
const httpOptions = { // header config for carrying token to access authorized get requests
  headers: new HttpHeaders({Authorization: 'Bearer' + JSON.parse(localStorage.getItem('user')).token})}

@Injectable({providedIn: 'root'})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users', httpOptions);
  }
  getMember(username: string) {
    return this.http.get<Member[]>(this.baseUrl + 'users/' + username, httpOptions);
  }
}
```

