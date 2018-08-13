#  Productivity Shell

Productivity tools for Visual Studio.

## Features

### Refactoring

**Move to Settings**

![https://imgur.com/aVCIJRP](https://i.imgur.com/aVCIJRP.png)

When Productivity Shell finds a value that can be potentially changed, it helps you move it to a settings file as quickly as possible. You can optionally search for identical values and refactor them to use the new settings item.

Depending on your project settings, values that you can move to a settings file may or may not be highlighted with a curly underline. If a value is not highlighted, press Ctrl+R and then Ctrl+S and select the Move to Settings refactoring. If a value is highlighted with a curly underline, you can press Alt+Enter and launch the refactoring directly from the list of quick-fixes.

In addition to values used in C# or VB.NET code, Productivity Shell is able to process values from markup files like XAML.

### Environment/Shell

**Restart**

![https://i.imgur.com/EG7gGmz.png](https://i.imgur.com/EG7gGmz.png)

Restarting Visual Studio is one of the most common tasks whenever an extension or the shell itself requires it. Productivity Shell provides an option to restart Visual Studio with normal privileges. If there is unsaved work, it will prompt you in case you wish to save the changes. After restarting, Productivity Shell will load again the solution in a seamless and painless execution.

**Restart as Administrator**

![https://i.imgur.com/FKyNH5j.png](https://i.imgur.com/FKyNH5j.png)

More often than not when loading a solution, we realize that certain project requires Administrator privileges to be debugged or executed. Productivity Shell provides an additional option to restart Visual Studio with elevated privileges.

**Path Variables**

![https://i.imgur.com/58CwqCv.png](https://i.imgur.com/58CwqCv.png)

Prints out the current variables located in the system's %PATH% for quick access and review.

### Projects

**Open Ouput Folder**

![https://i.imgur.com/FcnL8N5.png](https://i.imgur.com/FcnL8N5.png)

Productivity Shell offers a quick command that opens the output folders for all the projects currently selected. This feature is especially useful after one or more projects have been built and you wish to have immediate access to their respective output.

**Show File Ouput in File Explorer**

![https://i.imgur.com/O7YGstM.png](https://i.imgur.com/O7YGstM.png)

Shows the file output of the selected project in File Explorer. This action is built-in in Productivity Shell with the purpose of quickly reach the output file from a specific project.

**Reload Project**

![https://i.imgur.com/5y6cYrf.png](https://i.imgur.com/5y6cYrf.png)

Productivity Shell simplies the process of unloading and reloading projects in one single action. This action helps to quickly reload all the selected operations in the Solution Explorer.

### Tools

**GUID: Automatic Replacement of Placeholders**

![https://i.imgur.com/HQiw03O.png](https://i.imgur.com/HQiw03O.png)

Automatically replaces placeholders such as INSERT-GUID-HERE and PUT-GUID-HERE which are common in projects like those using WiX Toolset across a solution, projects or file ocurrences with a generated GUID value.


**Note:**
Most tools are customizable through Tools > Options > Productivity Shell