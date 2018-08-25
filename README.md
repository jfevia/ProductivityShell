#  Productivity Shell

Productivity tools for Visual Studio.

## Features

### Configuration

**Startup Profiles**

![https://imgur.com/Dikiri6](https://i.imgur.com/Dikiri6.png)

A customizable way to configure startup projects with extra debugging options. Productivity Shell allows you to customize the list of startup projects, including advanced extra debugging options. Switching between Startup Profiles is quickly done with a single click. 

The below example provides insight of the Startup Profiles configuration for Productivity Shell. This configuration can be stored as machine-wide, team-shared and user-specific with a layer mechanism for each solution for better modularity.

![https://imgur.com/pgQJ24H](https://i.imgur.com/pgQJ24H.png)

### Refactoring

**Move to Settings**

![https://imgur.com/aVCIJRP](https://i.imgur.com/aVCIJRP.png)

When Productivity Shell finds a value that can be potentially changed, it helps you move it to a settings file as quickly as possible. You can optionally search for identical values and refactor them to use the new settings item.

Depending on your project settings, values that you can move to a settings file may or may not be highlighted with a curly underline. If a value is not highlighted, press Ctrl+R and then Ctrl+S and select the Move to Settings refactoring. If a value is highlighted with a curly underline, you can press Alt+Enter and launch the refactoring directly from the list of quick-fixes.

In addition to values used in C# or VB.NET code, Productivity Shell is able to process values from markup files like XAML.

### Environment/Shell

**Restart**

![https://imgur.com/EG7gGmz](https://i.imgur.com/EG7gGmz.png)

Restarting Visual Studio is one of the most common tasks whenever an extension or the shell itself requires it. Productivity Shell provides an option to restart Visual Studio with normal privileges. If there is unsaved work, it will prompt you in case you wish to save the changes. After restarting, Productivity Shell will load again the solution in a seamless and painless execution.

**Restart as Administrator**

![https://imgur.com/FKyNH5j](https://i.imgur.com/FKyNH5j.png)

More often than not when loading a solution, we realize that certain project requires Administrator privileges to be debugged or executed. Productivity Shell provides an additional option to restart Visual Studio with elevated privileges.

**Path Variables**

![https://imgur.com/58CwqCv](https://i.imgur.com/58CwqCv.png)

Prints out the current variables located in the system's %PATH% for quick access and review.

### Projects

**Open Ouput Folder**

![https://imgur.com/FcnL8N5](https://i.imgur.com/FcnL8N5.png)

Productivity Shell offers a quick command that opens the output folders for all the projects currently selected. This feature is especially useful after one or more projects have been built and you wish to have immediate access to their respective output.

**Show File Ouput in File Explorer**

![https://imgur.com/O7YGstM](https://i.imgur.com/O7YGstM.png)

Shows the file output of the selected project in File Explorer. This action is built-in in Productivity Shell with the purpose of quickly reach the output file from a specific project.

**Reload Project**

![https://imgur.com/5y6cYrf](https://i.imgur.com/5y6cYrf.png)

Productivity Shell simplies the process of unloading and reloading projects in one single action. This action helps to quickly reload all the selected operations in the Solution Explorer.

### Tools

**GUID: Automatic Replacement of Placeholders**

![https://imgur.com/HQiw03O](https://i.imgur.com/HQiw03O.png)

Automatically replaces placeholders such as INSERT-GUID-HERE and PUT-GUID-HERE which are common in projects like those using WiX Toolset across a solution, projects or file ocurrences with a generated GUID value.


**Note:**
Most tools are customizable through Tools > Options > Productivity Shell
