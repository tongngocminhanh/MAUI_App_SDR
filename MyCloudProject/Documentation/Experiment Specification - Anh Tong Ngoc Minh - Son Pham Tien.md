# ML22-23-8 Implement the SDR representation in the MAUI application - Azure Cloud Implementation

This project requires the implementation of the SE Project topic "ML22-23-8 Implement the SDR representation in the MAUI application" onto the cloud computing platform. Microsoft Azure is the core platform for constructing and building the web App SDR application. Azure provides the data storage, letting the app store inputs and outputs in a container or queue, and show how the app could utilize the Azure's services.

## Table of contents
1. [Introduction](#introduction)
2. [SE Project links](#se-project-links)
3. [Important Cloud Project links](#important-cloud-project-links)
4. [Project specifications](#project-specifications)
5. [Overview of the cloud architecture](#overview-of-the-cloud-architecture)
    * [Experiment description](#experiment-description)
    * [Azure Container Registry](#azure-container-registry)
6. [Experiment and evaluation](#experiment-and-evaluation)
    * [How to run experiment](#how-to-run-experiment)
    * [Azure deployment](#azure-deployment)
    * [Evaluation](#evaluation)
7. [References](#references)

## Introduction
Microsoft Azure is an open and flexible cloud-computing platform. The scope of this project is to apply Azure Cloud to the Software Project of .NET MAUI (AppSDR), using Azure storage containers for storing inputs and outputs of the app. In general, the project involves:

* Modifying the current .NET MAUI app to access the Azure storage account. The main idea is to create the *BlobStorageService* class, holding the input storage account information, and which type of storage is used. 
* Creating a new page with new functions to interact with the Azure storage account, including uploading and downloading files. Users on this page specify the storage information calling the *BlobStorageService* to access the provided storage account.
* Adding a button for *Listening Mode* which waits for messages indicating that CSV files are ready for processing in a specified container. Users can manually upload messages, and when ready, a message can be sent programmatically. The app initiates file processing on-demand without waiting for the message, ensuring flexibility in operation.

AppSDR is operated locally, not dockerized, to receive the user's input interaction. The app accesses the specific cloud storage and saves the user's input. The SDR representation is saved on the storage and can be downloaded to the local machine.

## SE Project links
1. SE Project Documentation: [PDF](../../../MySEProject/Documentation/ML22-23-8-Implement%20the%20SDR%20representation%20in%20the%20MAUI%20application_MAUI_App_SDR-Paper.pdf)<br/>
2. README files: [Description](../../../MySEProject/Documentation/Readme.md), [User Manual](../../../MySEProject/Documentation/UserManual.md)
3. Implemented classes: [MainViewModel()](../../../MySEProject/AppSDR/ViewModel/MainViewModel.cs), [Page1ViewModel()](../../../MySEProject/AppSDR/ViewModel/Page1ViewModel.cs), [MainPage()](../../../MySEProject/AppSDR/MainPage.xaml.cs), [TextEditorPage()](../../../MySEProject/AppSDR/TextEditorPage.xaml.cs), [Page1.xaml.cs](../../../MySEProject/AppSDR/Page1.xaml.cs)<br/>
4. SDR drawing library: [SdrDrawerable()](../../../MySEProject/AppSDR/SdrDrawerLib/SdrDrawable.cs)<br/>
5. Sample SDR inputs: [Folder](../../../MySEProject/Documentation/TestSamples/)

## Important Cloud Project links

## Project specifications

## Overview of the cloud architecture

### Experiment specifications
* Improvise the App SDR into MVC web application.
* Use Blobs container to store SDR values in Queue storage, and save output Images.
* Create a new class to access the blob containers.
* The app deployment could be in docker or normal.

1. What is the **input**?
- SDR values in Queues
- Parameters to define the graph from keyboards, saved in Table.
- Text files, CSV files stored in containers.

2. What is the **output**?
- Graphs shown on web app
- Saved pictures in containers

3. What your algorithmas does? How ?

### Azure Container Registry


## Experiment and evaluation

### How to run experiment

Describe Your Cloud Experiment based on the Input/Output you gave in the Previous Section.

**_Describe the Queue Json Message you used to trigger the experiment:_**  

~~~json
{
     ExperimentId = "123",
     InputFile = "https://beststudents2.blob.core.windows.net/documents2/daenet.mp4",
     .. // see project sample for more information 
};
~~~

- ExperimentId : Id of the experiment which is run  
- InputFile: The video file used for trainign process  

**_Describe your blob container registry:**  

what are the blob containers you used e.g.:  
- 'training_container' : for saving training dataset  
  - the file provided for training:  
  - zip, images, configs, ...  
- 'result_container' : saving output written file  
  - The file inside are result from the experiment, for example:  
  - **file Example** screenshot, file, code  


**_Describe the Result Table_**

 What is expected ?
 
 How many tables are there ? 
 
 How are they arranged ?
 
 What do the columns of the table mean ?
 
 Include a screenshot of your table from the portal or ASX (Azure Storage Explorer) in case the entity is too long, cut it in half or use another format
 
 - Column1 : explaination
 - Column2 : ...
Some columns are obligatory to the ITableEntities and don't need Explaination e.g. ETag, ...
 
### Azure deployment

### Evaluation
