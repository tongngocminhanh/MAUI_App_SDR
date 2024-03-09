# ML22-23-8 Implement the SDR representation in the MAUI application

 This project requires the implementation of a .NET Multi-platform App User Interface (.NET MAUI) app for visualizing the SDR representations. The app is compatible in Windows, Android, and IOS devices. This includes generating a UI for parameters setting and file input with XAML, binding data for application and logic with Model-View-ViewModel (MVVM) and XAML, using of Maui.Graphics for generating a new SDR drawing library.  

## Table of contents
*   [Introduction](#introduction)
*   [Getting Started](#getting-started)
*   [Implementation of MAUI App](#implementation-of-maui-app)
    * [UI implementation](#ui-implementation)
    * [SDR drawing library implementation](#sdr-drawing-library-implementation)
*   [Testing and evaluation](#testing-and-evaluation)
*   [License](#license)
*   [References](#references)

## Introduction
.NET MAUI is a cross-platform framework, dealing with real-time problems. The foundation of MAUI is to simplify multiplatform app development. Within a single project, users can add the platform-specific source code and resources if needed. The scope of the project is to show the hardware and software requirements, how to implement the MAUI app, evaluate the test results, and provide possible improvements. In general, the project involves
* Create a User Interface (UI) in the MAUI app so users can define required parameters, and input a file from local devices. The parameters for visual control of the output screen, while the input file contains data used for representation. The main idea in this part is to use XAML to set the method for inputting and Data binding to handle the data within the program.
* Generate and implement an SDR drawing library to visualize the input data based on requirements. The library contains multiple classes and functions used to clarify the content of the output. Therefore, for future improvements, developers can handle the library better. 

Based on the previous reference of the SDR representation generating file - [draw_figure.py](./Python/ColumnActivityDiagram/draw_figure.py), the requirements are to create a MAUI app that could draw the similar SDR representation as the mentioned file. The result is directly loaded on the app UI, not in another HTML file. The app input can handle specific formats for files: CSV and TXT, and types for parameters: string, and numbers. Evaluations are made on "Windows machine" because of the bigger, better view and simple operation. 

## Getting started
The project integrates the latest update; therefore, the suggestion is to install at least the following version of IDE, text editor, and MAUI.
* Microsoft Visual Studio Community 2022 - v17.8.6 - https://learn.microsoft.com/en-us/visualstudio/releases/2022/release-notes-v17.8
* .NET MAUI 8.0 - when installing Visual Studio, choose .NET MAUI to integrate along with the installation. From the version of v17.8.6, .NET MAUI 8.0 will be automatically supported.

Detailed information con be found on [MySEProject](https://github.com/tongngocminhanh/MAUI_App_SDR/tree/master/MySEProject).

## Implementation of MAUI App
### UI implementation
### SDR drawing library implementaion
## Testing and evaluation

In this project, we have successfully implemented the MAUI app for visualizing SDR representation. Users can set the wanted parameters for the configuration of the graph. They also can input the file with the extension "CSV, TXT" containing data of Active Cells Columns, for visualization. For further improvement, the draw function in this App could be generated into one MAUI library and applied to other purposes.

## License
[MIT](https://choosealicense.com/licenses/mit/)

## References

[Documentaion on MAUI creation and configuration ](https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-8.0)
[Example videos of building the first MAUI app ](https://www.youtube.com/playlist?list=PLdo4fOcmZ0oUBAdL2NwBpDs32zwGqb9DY)