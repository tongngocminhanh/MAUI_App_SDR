# ML22-23-8 Implement the SDR representation in the MAUI application

 This project requires the implementation of a .NET Multi-platform App User Interface (.NET MAUI) app for visualizing the SDR representations. The app is compatible in Windows, Android, and IOS devices. This includes generating a UI for parameters setting and file input with XAML, binding data for application and logic with Model-View-ViewModel (MVVM) and XAML, using of Maui.Graphics for generating a new SDR drawing library.  

## Table of contents
1. [Introduction](#introduction)
2. [Important Links](#important-links)
2. [Getting Started](#getting-started)
3. [Implementation of MAUI App](#implementation-of-maui-app)
    * [UI implementation](#ui-implementation)
    * [Logic implementation](#logic-implementation)
    * [SDR drawing library implementation](#sdr-drawing-library-implementation)
4. [Testing and evaluation](#testing-and-evaluation)
5. [References](#references)

## Introduction
.NET MAUI is a cross-platform framework, dealing with real-time problems. The foundation of MAUI is to simplify multiplatform app development. Within a single project, users can add the platform-specific source code and resources if needed. The scope of the project is to show the hardware and software requirements, how to implement the MAUI app, evaluate the test results, and provide possible improvements. In general, the project involves
* Create a User Interface (UI) in the MAUI app so users can define required parameters, and input a file from local devices. The parameters for visual control of the output screen, while the input file contains data used for representation. The main idea in this part is to use XAML to set the method for inputting and Data binding to handle the data within the program.
* Modify the logic behind the UI elements, showing how the data binds together and being processed.
* Generate and implement an SDR drawing library to visualize the input data based on requirements. The library contains multiple functions used to clarify the content of the output. Therefore, for future improvements, developers can handle the library better. 

Based on the previous reference of the SDR representation generating file - [draw_figure.py](./Python/ColumnActivityDiagram/draw_figure.py), the requirements are to create a MAUI app that could draw the similar SDR representation as the mentioned file. The result is directly loaded on the app UI, not in another HTML file. The app input can handle specific formats for files: CSV and TXT, and types for parameters: string, and numbers. Evaluations are made on "Windows machine" because of the bigger, better view and simple operation. 

## Important links
1. SE Project Documentation: [PDF](https://github.com/tongngocminhanh/MAUI_App_SDR/tree/master/MySEProject/Documentation)<br/>
2. UI implemenation files of Input Page, Text Input Page and Visualisation Page: [MainPage.xaml](https://github.com/tongngocminhanh/MAUI_App_SDR/blob/master/MySEProject/AppSDR/MainPage.xaml), [TextEditorPage.xaml](https://github.com/tongngocminhanh/MAUI_App_SDR/blob/master/MySEProject/AppSDR/TextEditorPage.xaml), [Page1.xaml](https://github.com/tongngocminhanh/MAUI_App_SDR/blob/master/MySEProject/AppSDR/Page1.xaml)<br/>
3. Logic implementation files of mentioned pages: [MainViewmodel.cs](https://github.com/tongngocminhanh/MAUI_App_SDR/blob/master/MySEProject/AppSDR/ViewModel/MainViewModel.cs), [TextEditorPage.xaml.cs](https://github.com/tongngocminhanh/MAUI_App_SDR/blob/master/MySEProject/AppSDR/TextEditorPage.xaml.cs), [Page1.xaml.cs](https://github.com/tongngocminhanh/MAUI_App_SDR/blob/master/MySEProject/AppSDR/Page1.xaml.cs)<br/>
4. SDR drawing library: [SDRDrawerLib]()<br/>
5. Project solution: [MyProjectSample.sln](https://github.com/tongngocminhanh/MAUI_App_SDR/blob/master/MySEProject/MyProjectSample.sln)

## Getting started
The project integrates the latest update; therefore, the suggestion is to install at least the following version of IDE, text editor, and MAUI.
* Microsoft Visual Studio Community 2022 - v17.8.6 - https://learn.microsoft.com/en-us/visualstudio/releases/2022/release-notes-v17.8
* .NET MAUI 8.0 - when installing Visual Studio, choose .NET MAUI to integrate along with the installation. From the version of v17.8.6, .NET MAUI 8.0 will be automatically supported.

Detailed information can be found on [MySEProject](https://github.com/tongngocminhanh/MAUI_App_SDR/tree/master/MySEProject).

## Implementation of MAUI App
When building a .NET MAUI app, developers could work on one specific platform, normally Windows. To display on other platforms, .NET MAUI auto-generates the compatible code for that platform. For this project, specification on Windows is provided, based on the general architecture below.

<div style="background-color: #ffffff; text-align:center">
  <img src="./Figures/General Architecture of App SDR.jpg" title="general-architecture-of-app-sdr" width=700px></img>
</div><br>

The project's idea is to draw SDR representations from users' SDR data and visualization specifications. Therefore, users must input the program in two orders: 
* Parameters to define the graphical output of the required representation. The program uses *Main Page* to collect the parameters, parsing them to *Text Editor Page* and *Page 1* depending on the user's intention.
* An input text file (.txt) or sheet files (.csv) feeding the SDR data into the program. Another method to input SDR data is directly entering values from keyboards. In both cases, the data is parsed into *Page 1* for visualizing SDR representations with the help of the *SDRDrawerLib* library.

Details on implementation configuration and steps are explained in the next subsections.

### UI implementation
### Logic implementation
### SDR drawing library implementaion
## Testing and evaluation

In this project, we have successfully implemented the MAUI app for visualizing SDR representation. Users can set the wanted parameters for the configuration of the graph. They also can input the file with the extension "CSV, TXT" containing data of Active Cells Columns, for visualization. For further improvement, the draw function in this App could be generated into one MAUI library and applied to other purposes.

## References

[Documentaion on MAUI creation and configuration ](https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-8.0)
[Example videos of building the first MAUI app ](https://www.youtube.com/playlist?list=PLdo4fOcmZ0oUBAdL2NwBpDs32zwGqb9DY)