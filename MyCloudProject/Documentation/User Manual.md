# User manual for AppSDR v2.0
This document provides information on AppSDR v2.0 's installation and Cloud usage. AppSDR v1.0 can visualize the SDR locally without using Cloud, while in v2.0, the app inherits the v1.0 functions and implements new Cloud operating functions. In [Experiment Specification](Experiment%20Specification%20-%20Anh%20Tong%20Ngoc%20Minh%20-%20Son%20Pham%20Tien.md), there are four possible testing cases of two methods for SDR visualization using Cloud: the *Storage Account 1* and *Storage Account 2*. The two methods are manual and automatic. The corresponding table illustrates the components of the components of the possible testing cases.

|Test          |Inputs               |Operation steps                          |
|--------------|---------------------|-----------------------------------------|
|Case 1|Connection String, Storage Name of *Storage Account 1*, two Blobs, one Table container name,  eight parameters, multiple existing SDR files (.txt, .csv)|Input parameters, Input information for *Storage Account 1*, Connect to *Storage Account 1*, Input Blobs and Table container name, Upload parameters, Choose and upload files, Generate and upload outputs|
|Case 2|Connection String, Storage Name of *Storage Account 1*, two Blobs, one Table container name,  eight parameters, entered SDR values|Input parameters, Enter SDR values in Text Editor and Save that into a .txt file, Input information for *Storage Account 1*, Connect to *Storage Account 1*, Input Blobs and Table container name, Upload parameters, Upload the saved file, Generate and upload outputs|
|Case 3|Connection String, Storage Name of *Storage Account 1*, two Blobs, one Table container name|Input information for *Storage Account 1*, Connect to *Storage Account 1*, Input Blobs and Table container name, Generate and upload outputs|
|Case 4|Connection String of *Storage Account 2*, Queue Container name| Input information for MESSAGE operation, Upload MESSAGE, Start Listening mode|

The structure of this User Manual is:

* Prerequisite: shows the standard version of the application, and some modifications to the local Windows machine.
* Installation: shows how to clone the Cloud Project and enable the project to be operable on the local device (device modification and necessary Nuget Packages). 
* How to run: describes the step-by-step instructions on interactions with the AppSDR v2.0. There are some repeated steps within the four testing cases in the table; therefore, for each test, if the operation step is not mentioned, a clear description is provided, otherwise, only the operation step name is shown. Users can check the above information. The outputs are the same for all testing cases, so this part shows the process, and the next part provides all the output content.
* Output representation: shows the visualizations on AppSDR, the Storage Containers content before and after the experiment, and local device storage.

## Table of contents
* [Prequisite](#prequisite)
* [Installation](#installation)
* [How to run](#how-to-run)
  * [Case 1](#case-1)
  * [case 2](#case-2)
  * [Case 3](#case-3)
  * [Case 4](#case-4)
* [Output representation](#output-representation)

## Prerequisite
It is advised to install the most recent versions of the IDE, text editor, and MAUI as the project incorporates the most recent update. 
* Microsoft Visual Studio Community 2022 - v17.8.6 - https://learn.microsoft.com/en-us/visualstudio/releases/2022/release-notes-v17.8 
* .NET MAUI 8.0 - select .NET MAUI interface like the image below during Visual Studio installation.

<div style="text-align:center">
  <img src="./Figures/Prerequisite.jpg" title="prequisite" width=80%></img>
</div><br>

* If the system requires to update Visual Studio to the newest version, please follow that.

## Installation
Before running the project, installation is made. To install and open the project, follow the following steps.
* First, clone the project into a local device for use and testing
```
git clone https://github.com/tongngocminhanh/MAUI_App_SDR.git
```
* Second, navigate the solution directory of the app. The solution is located on *\local directory\MAUI_App_SDR\MyCloudProject*. 
* Open *MyCloudProject.sln* with Visual Studio.
* To run or build the app with Visual Studio on a Windows device, create a folder named *Properties* under *\local directory\MAUI_App_SDR\MyCloudProject\AppSDR* and include the *launchSettings.json* file, content as follows.

```json
{
    "profiles": {
      "Windows Machine": {
        "commandName": "MsixPackage",
        "nativeDebugging": false
      }
    }
}
```

* The result should looks like the image. This step enables the Windows mode, so users can operate AppSDR v2.0 on Windows local device.

<div style="text-align:center">
  <img src="./Figures/Installation.png" title="installation" width=80%></img>
</div><br>

* There might be some Nuget packages on your device not up-to-date. Therefore, open the manager by right-clicking the *AppSDR* on Visual Studio, and choose *Manage the NuGet Package*. Then update all the installed packages. 

<div style="text-align:center">
  <img src="./Figures/UpdateNuGet.png" title="update-nuget-packages" width=80%></img>
</div><br>

* Users should check the Developer mode, ensure that it is turned on.

<div style="text-align:center">
  <img src="./Figures/developer.png" title="developer-mode" width=80%></img>
</div><br>

Now, the app is ready to run.

## How to run
As mentioned, there are four cases described in this document. All cases start with the same opening step. When clicking the *Windows Machine* button, the AppSDR is loaded and the *Main Page* is shown.

<div style="text-align:center">
  <img src="./Figures/Debug.png" title="start-debugging" width=80%></img>
</div><br>

When clicking the *Windows Machine* button, the AppSDR is loaded and the *Main Page* is shown. For Cloud-relevant functions, care from the label *Manually add SDR values* downward.

<div style="text-align:center">
  <img src="./Figures/MainPage.jpg" title="main-page-representation" width=80%></img>
</div><br>

We can see from the *Main Page*:
* The *Table Entry* has eight blank spaces, corresponding to the eight parameters we need for our operation: Graph Name, Max Cycles, Highlight Touch, X-Axis Title, Y-Axis Title, Min Range, and Max Range. We should be careful that Max Cycles, Highlight Touch, Min Range, and Max Range contain numbers only, while the rest can be in number or text form.

* The *Add Text* button is invalid as the parameters table above is empty. make sure to enter ALL the entities to enable this button.

* The *Cloud configuration* button is clickable whether the parameters are given or not, we can see the differences in Case 1 and Case 3.

* The *Message Configuration* has three text editor spaces to handle text inputs, applied in Case 4. Only when the three blocks are not empty, can we click the *Upload Message* button.

The outputs of all cases have the same form, so they will be separately discussed in [Output Representation](#output-representation), not in the case sections.

### Case 1
**Input parameters**: First, we enter the table entities, the following is the example for the parameters. For the number type entities, try to apply the low value for *Min range*, and high values for *Max cycles, Max range*, so most Input SDR values can fit in the constraint. 

```
Graph name: Sample graph
Max cycles: 1500
Highlight touch: 3
X axis title: SDR columns
Y axis title: SDR values
Min range of cell: 0
Max range of cell: 10000
Saved figure name: SavedSDR
```

Then we click the *Cloud configuration* button and move to *Upload Page*.

<div style="text-align:center">
  <img src="./Figures/AfterParameterCloud.png" title="after-enter-parameters-to-cloud" width=80%></img>
</div><br>

Now, we are on the *Upload Page*. We can see, there are two parts: the left side and the right side. The left side has blank editors and buttons, while the right side has buttons only. The grey buttons are not accessible as their operating conditions are missed. The yellow buttons are clickable.

<div style="text-align:center">
  <img src="./Figures/UploadPageNoPara.jpg" title="upload-page-no-parameters" width=80%></img>
</div><br>

Then, the next operation steps are illustrated.

**Input information for Storage Account 1**:  *Configure Storage* part has two editors to hold the Connection String and the Storage Name. We enter the two editors with the content only, without blank space or Enter key.The sample is 

```
Storage account 1
- Connection String: DefaultEndpointsProtocol=https;AccountName=mauiprojectcloud;AccountKey=gDYct5X+8L0wUco6yIYFSvfdh/1UbwYmAAashjpETQ1czbYjS/1dtdgdhW0pjOlQoqmWqbAbXslb+AStiMasTw==;BlobEndpoint=https://mauiprojectcloud.blob.core.windows.net/;QueueEndpoint=https://mauiprojectcloud.queue.core.windows.net/;TableEndpoint=https://mauiprojectcloud.table.core.windows.net/;FileEndpoint=https://mauiprojectcloud.file.core.windows.net/;

- Storage Account Name: mauiprojectcloud-1
```

**Connect to Storage Account 1**: we can connect to *Storage Account 1* by clicking *Connect*. We will see on the left side, there is a red status showing that AppSDR has connected to *Storage Account 1*, and the *Upload defined parameters* button turns yellow. While on the right side, the upper three buttons turn yellow. 

<div style="text-align:center">
  <img src="./Figures/ConnectToStorage.png" title="connect-to-storage" width=90%></img>
</div><br>

**Input Blobs and Table container name**: next, we enter the *Configure Blob Storage* editors with values:

```
SDR values Blob Name: sdrfiles
Outputs Blob Name: saveoutput
Table Storage Name: parameters
```
**Upload parameters**: by clicking *Upload defined parameters*, the status updates with the successful notification. The eight parameters are uploaded to the Table Container.

<div style="text-align:center">
  <img src="./Figures/ParaConnect.png" title="upload-parameters" width=80%></img>
</div><br>

**Choose and upload files**: then, we choose the input files by clicking the "Select and Upload File" button. The AppSDR will access the local device memory, pop up the choosing screen, and allow the user to choose multiple files. The file extension ".txt, .csv" is supported, so only .txt and .csv files are shown on the screen. This experiment takes the sample files from [TestSamples](../Documentation/TestSamples/), having two .txt files - [samplerandom.txt](../Documentation/TestSamples/samplerandom.txt) and [sampleZero](../Documentation/TestSamples/sampleZero.txt), and a .csv file - [sampleSheet](../Documentation/TestSamples/sampleSheet.csv). The content contains numbers and rows of numbers. Each number presents the SDR values separated by "," for .txt files, or column for .csv files. Each row presents an SDR column, a collection of SDR values. Here is a simple example of the .txt file content.

```
1541, 6, 12, 2573, 1044, 1557, 3608, 1049, 3503, 3619, 3590, 2605, 3630, 2609
2560, 2565, 2568, 524, 2077, 1568, 2088, 1066, 1287, 49, 51, 3126, 2618, 3138
```

After selecting the needed files, we click *Open* on the pop-up screen, it is automatically closed. The left status updates with successful notification.

<div style="text-align:center">
  <img src="./Figures/ChooseFiles.jpg" title="choose-files" width=80%></img>
</div><br>

**Generate and upload outputs**: finally, the "Generate and upload Output Files" button is clicked. We will wait for the output generation. Check [Output Representation](#output-representation) for details.

<div style="text-align:center">
  <img src="./Figures/Generate.png" title="generate-output" width=80%></img>
</div><br>

### Case 2
This test case is triggered when all the table entities are full. Then the "Add Text" button is accessible.
* Click the "Add Text" button and move to *Text Editor Page*.
<div style="text-align:center">
  <img src="./Figures/MainPageText.jpg" title="main-page-text-editor" width=80%></img>
</div><br>

* In this page, instruction is given on the top, follow that and enter the SDR values. The click "Generate an image with Cloud"and move to *Upload Page*. 

* The entered text is saved as a text file on Desktop, and when "Select and Upload File" is clicked among the following steps, the app automatically chooses the saved file to upload.

<div style="text-align:center">
  <img src="./Figures/TextEditor.jpg" title="text-editor-page" width=80%></img>
</div><br>

* When user is on *Upload Page*, the next steps are the same as the *Upload Page* ones in [Parameters and multiple SDR files](#parameters-and-multiple-sdr-files).

### No uploaded parameters and files
If it is verified that SDR file Blob and Parameters Table Container have values, users can run the operation after connecting to the Atorage Account and specified the containers. 

* Directly click *Cloud Configuration* on *Main Page*, and move to *Upload Page*. No parameter is defined.
* When on *Upload Page*, enter the "Configure Storage' and connect to the Storage Account by clicking "Connect".
* Enter the other container names in "Configure Blob Storage". Because no parameters are defined, the *Upload defined parameters* is not clickable.

<div style="text-align:center">
  <img src="./Figures/StorageConnect.jpg" title="upload-page-storage-connect" width=80%></img>
</div><br>

* Click *Generate and upload Output Files* and wait for the visualization.

### Mesage generation 
AppSDR can give output with MESSAGE trigger. User uploads a MESSAGE to a Queue Container, being different or the same with the working Storage Account. 
* The three *Message Configuration* spaces are filled, then the *Upload Message* button is valid.
* After clicking *Upload Message*, a pop-up appears with the successful message. Click "OK", and choose the *Cloud Configuration* button next.

<div style="text-align:center">
  <img src="./Figures/MessageConnect.jpg" title="message-main-page" width=60%></img>
</div><br>

* Only when the message is uploaded, is the *Start Listening* button valid. 

<div style="text-align:center">
  <img src="./Figures/UploadPageMessage.jpg" title="message-upload-page" width=80%></img>
</div><br>

* Click *Start Listening* and wait for the outputs. No extra inputs are required.

## Output representation
All of the cases generate the same form of output. 
* If there is only one SDR file, the output is the *Page 1* visualization. *Page 1* appears on top of the *Upload Page*.

<div style="text-align:center">
  <img src="./Figures/OneOutput.jpg" title="one-output-representation" width=80%></img>
</div><br>

* If there are multiple files in the Blob Containers or several parameter definitions in the Table Container, AppSDR draws all the possible combinations with *Page 1*. Each output will be over one other.

<div style="text-align:center">
  <img src="./Figures/Output.jpg" title="multiple-outputs-representation" width=120%></img>
</div><br>

* When click "OK", visualization is uploaded in the Output Blob Container as an image file. Each output will have its own "OK" pop-up message.
* Navigation back to *Main Page* is possible with the *BACK TO MAIN PAGE* button on the final output. If it is not visible, scroll down the page a little bit.

Further implementation information can be found [here](./Experiment%20Specification%20-%20Anh%20Tong%20Ngoc%20Minh%20-%20Son%20Pham%20Tien.md).