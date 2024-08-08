# ML22-23-8 Implement the SDR representation in the MAUI application - Azure Cloud Implementation

This project requires the implementation of the SE Project topic "ML22-23-8 Implement the SDR representation in the MAUI application" onto the cloud computing platform. Microsoft Azure is the core platform for constructing and building the web App SDR application. Azure provides the data storage, letting the app store inputs and outputs in a container or queue, and show how the app could utilize the Azure's services.

## Table of contents
1. [Introduction](#introduction)
2. [Important Project Links](#important-project-links)
    * [SE Project links](#se-project-links)
    * [Cloud Project links](#cloud-project-links)
3. [Goal of Cloud Project](#goal-of-cloud-project)
4. [Overview of the cloud architecture](#overview-of-the-cloud-architecture)
5. [Implementation of new properties in AppSDR](#implementation-of-new-properties-in-appsdr)
    * [UI implementation](#ui-implementation)
    * [Logic implementation](#logic-implementation)
6. [Experiment and evaluation](#experiment-and-evaluation)
    * [How to run experiment](#how-to-run-experiment)
    * [Evaluation](#evaluation)
7. [References](#references)

## Introduction
AppSDR is the .NET MAUI app visualizing the Sparse Distribution Representations (SDR) with the user's SDR values and drawing specifications. The map's primary functions are taking the parameters for graph visualization, and the SDR values with local files or manually set. For Cloud implementation on the AppSDR, the following are created.

* Modify the original pages with new variables and properties to navigate to a new page.
* Add a new *UploadPage* to handle the cloud properties, with new UI elements to specify the pointed Azure data objects (Storage Account, Blob, Queue, and Table Storage).
* Add two new classes to handle the *Message operation* functions, working as the automatic generation when users want outputs with the Azure platform.

The new version of AppSDR can be operated manually and automatically, depending on users' purpose. This project mainly works on Azure Storage Account. Dockers and Azure Services are not included in this implementation.

## Important Project Links
### SE Project links
1. SE Project Documentation: [PDF](../../MySEProject/Documentation/ML22-23-8-Implement%20the%20SDR%20representation%20in%20the%20MAUI%20application_MAUI_App_SDR-Paper.pdf)<br/>
2. README files: [Description](../../MySEProject/Documentation/Readme.md), [User Manual](../../MySEProject/Documentation/UserManual.md)
3. Implemented classes: [MainViewModel()](../../MySEProject/AppSDR/ViewModel/MainViewModel.cs), [Page1ViewModel()](../../MySEProject/AppSDR/ViewModel/Page1ViewModel.cs), [MainPage()](../../MySEProject/AppSDR/MainPage.xaml.cs), [TextEditorPage()](../../MySEProject/AppSDR/TextEditorPage.xaml.cs), [Page1()](../../MySEProject/AppSDR/Page1.xaml.cs)<br/>
4. SDR drawing library: [SdrDrawerable()](../../MySEProject/AppSDR/SdrDrawerLib/SdrDrawable.cs)<br/>
5. Sample SDR inputs: [Folder](../../MySEProject/Documentation/TestSamples/)

### Cloud Project links
1. Cloud Project Documentation: [Experiment Specification](./Experiment%20Specification%20-%20Anh%20Tong%20Ngoc%20Minh%20-%20Son%20Pham%20Tien.md)
2. Other README file: [User Manual](User%20Manual.md)
3. UI implemenation of Cloud Configuration Page: [UploadPage.cs](../AppSDR/UploadPage.xaml)
4. Logic implementation class of Cloud Configuration Page: [UploadPage()](../AppSDR/UploadPage.xaml.cs), [UploadViewModel()](../AppSDR/ViewModel/UploadViewModel.cs)
5. Message handle class: [ExperimentRequestMessage()](../AppSDR/ExperimentRequestMessage.cs), [QueueMessageListener()](../AppSDR/QueueMessageListener.cs)
6. Project solution: [MyCloudProject.sln](../MyCloudProject.sln)

## Goal of Cloud Project
Microsoft Azure is an open and flexible cloud-computing platform. The scope of this project is to apply Azure Cloud to the Software Project of .NET MAUI (AppSDR), using Azure storage containers for storing inputs and outputs of the app. In general, the project involves:

* Modifying the current .NET MAUI app to access the Azure storage account. The main idea is to create the *BlobStorageService* class, holding the input storage account information, and which type of storage is used. 
* Creating a new page with new functions to interact with the Azure storage account, including uploading and downloading files. Users on this page specify the storage information calling the *BlobStorageService* to access the provided storage account.
* Adding a button for *Listening Mode* which waits for messages indicating that CSV files are ready for processing in a specified container. Users can manually upload messages, and when ready, a message can be sent programmatically. The app initiates file processing on-demand without waiting for the message, ensuring flexibility in operation.

AppSDR is operated locally, not dockerized, to receive the user's input interaction. The app accesses the specific cloud storage and saves the user's input. The SDR representation is saved on the storage and can be downloaded to the local machine.

## Overview of the Cloud Architecture
The Cloud Architecture describes the relationship among the object-based Cloud storages. The figure has green components illustrating the manual generation, and the orange ones for automatic generation.

<div style="background-color: #ffffff; text-align:center">
  <img src="./Figures/CloudArchitecture.png" title="general-cloud-architecture" width=70%></img>
</div><br>

This project involves Azure Containers carrying the uploaded data and retrieving it using the corresponding feature from AppSDR. The storage specification for each generating method must follow:

* Users can only use the manual method when *Storage Account 1* is connected. Then users' SDR parameters inputs are uploaded and stored in the *Table Container*, and the SDR files are in the *Blob Storage*. When generation is started in AppSDR, the app retrieves the most recent entity from the *Table* and the values of all files in *Blob* to draw the outputs. The outputs are stored in the *Output Blob*.

* For automatic method, users provide information to connect *Storage Account 2*, and upload the *MESSAGE* to a defined *Queue Container*. This MESSAGE* contains the Connection String of *Storage Account 2*, along with its attending containers. When AppSDR listens to the MESSAGE in Queue, the output generation starts. The process includes: taking parameters, taking files, drawing outputs, and uploading outputs to Blob. 

* *Storage Account 2* can be different from 1, or user can make use of *Storage Account 1* to store Queue MESSAGE.

## Implementation of new properties in AppSDR
The primary AppSDR's architecture from the SE Project remains, adding the Cloud-configuration *Upload Page*, and additional functions in existing pages to handle Azure Cloud components. The visualization for the new application described in the figure below has black and blue components representing the original AppSDR and red components for the Cloud Project implementation.

<div style="background-color: #ffffff; text-align:center">
  <img src="./Figures/NewAppSDRStructure.png" title="general-architecture-of-app-sdr" width=90%></img>
</div><br>

The new properties are implemented with the following specifications.

* Besides the specified inputs in the SE project to *Main Page*, a new handling place is created to take the *Message Configuration* used for automatic SDR visualization generation. This input is optional, so when users want to proceed with the operation manually, they can ignore this input area.
* A New *Upload Page* is created to take *Cloud Configuration* for Cloud accession. *Upload Page* is navigable from *Main Page, Text Editor Page*, and points to *Page 1*. This page calls additional classes to proceed with the *Message* from *Main Page*.

The *xaml* code for the addition is based on the original AppSDR, reviewed on the previous [Readme](../../MySEProject/Documentation/Readme.md). Details on UI and Logic implementation new configuration and steps are explained in the next subsections. 

### UI implementation
AppSDR remains the primary UI foundation, adding a new *Upload Page* defined as the following diagrams. For the initial pages, new arrangements are applied with new old and additional components.

<div style="background-color: #ffffff; text-align:center">
  <img src="./Figures/NewContentAndNavigation.png" title="new-content-and-navigation" width=70%></img>
</div><br>

The content and navigation of the UI elements of *Page 1* remains as in SE Project. While the existing *Main Page* and * Text Editor Page* have added components, following the configurations below.
* In *Main Page*, SDR picture and parameters inputs table stay the same. *Labels* are added to specify the button functions, corresponding to the same-line buttons. 
* A block of Message Configuration is added for users to provide *Storage Account* information and *MESSAGE*, accessing and uploading to the *Queue Container*. A new button is added to handle this block.
* Instead of *Entry*, AppSDR uses *Editor to carry this block inputs, as the content might be too long just for the *Entry* visualization.
* In *Text Editor Page*, one button is added, carrying the navigation to *Upload Page*.

The new *Upload page* contains two parts: the left one for Cloud configuration and the right parts for the actual operating functions, described in the next points: 
* Configure Storage: This section likely provides controls (buttons, text fields) to configure storage settings, such as connection strings or storage account details for Azure.
* Configure Blob & Table: Similar to storage configuration, but specifically for configuring Azure Blob Storage and Table Storage. This might involve selecting or creating containers and tables.
* Status: Displays the current status of operations, such as storage configuration, upload status, or connection status.
* Manual Generation: Controls to manually trigger the generation of messages or data that will be uploaded or processed.
* Message Generation: Controls related to automating or managing the generation of messages that the system will process or upload.
* Message Status: A display area showing the status of the messages being handled by the system, possibly indicating success, failure, or progress.

*Upload Page* has two panels for two applications, so the *Layout* must be combined. The left panel components fit in the preview size of the AppSDR, so the *StackLayout* is used. For the right panel, the components may exist in the area, so the *ScrollView* is used, with the *StackLayout* inside. Both of the panels are on the *Grid* to restrain the UI elements and are better to modify.

```xaml
    <Grid RowDefinitions="Auto,*" ColumnDefinitions="250,*,*,*,*,*">
        <!-- Left Panel with Storage Configuration Options -->
        <StackLayout Grid.Row="0" Grid.Column="0" Grid.RowSpan="2"
                     BackgroundColor="{StaticResource LeftPanelBackgroundColor}"
                     Padding="10" Spacing="10">     
        </StackLayout>

        <!-- Right Panel with SDR Visualization -->
        <ScrollView Grid.Row="0" Grid.Column="1" Grid.ColumnSpan="5"
                    BackgroundColor="{StaticResource RightPanelBackgroundColor}"
                    Padding="10">
            <StackLayout Spacing="20">
                <Label Text="GENERATE SDR VISUALIZATION" 
                       FontAttributes="Bold" 
                       TextColor="{StaticResource PrimaryTextColor}" />
            </StackLayout>
        </ScrollView>
    </Grid>
```
### Logic implementation


## Experiment and evaluation

### How to run experiment

Describe Your Cloud Experiment based on the Input/Output you gave in the Previous Section.
### Evaluation

**Describe your blob container registry:**  
what are the blob containers you used e.g.:  
- 'training_container' : for saving training dataset  
  - the file provided for training:  
  - zip, images, configs, ...  
- 'result_container' : saving output written file  
  - The file inside are result from the experiment, for example:  
  - **file Example** screenshot, file, code  

## References
