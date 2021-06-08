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
```

for our project we are going to run

```bash
dotnet new sln # gets its name from directory
dotnet new webapi -o API # creates a webapi project inside API folder
dotnet sln add API # adds project to the solution we created
```

### Required VS Code Extensions & Tips

**F1:** command palette, **Ctrl+":** terminal 
disable auto save, exclude obj and bin folders in user settings, disable compact folders
**Name:** [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) 
from command palette type generate assets and click it.

**Name:** [C# Extensions](https://marketplace.visualstudio.com/items?itemName=kreativ-software.csharpextensions) 

