# Project Report
This file contains a report of our IoT project. It is divided in three sections where we talk about the design of the system, what has changed from the original design and what we couldn't accomplish, and finally how the work was divided between the members of the group.

## System design
Our system is divided in three main components, you can see an image of the architecture in [system-design.jpg](./system-design.jpg). These components are the IoT component, with the raspberry PI and the actuators, the cloud component running in Azure, and the mobile component, an Android application.

### IoT Component
Our IoT component is formed by a Raspberry PI, a camera and a door actuator. (the LEDs are in the PI right). The main functionality is, receive information from the camera, send the images to the cloud, and based on the response open or not open the door lock. The IoT component also interfaces directly with the mobile application. In case a guest arrives that does not match with the home owners, the images can be streamed to the mobile application and the administrator can decide to explicitly open the door or not for that person.

The functionality to operate the camera and door actuator, communicate with the cloud and with the mobile application has all been implemented in Python. The communication with the cloud is quite simple, from the PI we make a get request with an image and the authorization key. The response is a true/false so we operate the door based on it. **(include details about camera and door actuator)**. The communication with the mobile application is the following. The PI is running a small Flask server, with two endpoints. The first one sends an image stream that comes directly from the camera, the second one exposes the ability to operate the lock. The mobile application can read the images from the stream and show them to the user and can make requests to operate the lock. 

### Cloud Component
Our cloud component is an ASP.NET API using C# and .NET Core 3.1 and running on Azure Cloud Services. This server application handles the list of users that have been registered with their images, and contains the logic to determine when the door should or should not be opened. It exposes several endpoints with functionality such as listing the registered users with their pictures, adding a new person, deleting a person, or getting a face match from an image. The data is protected and only requests with a predefined authorization key have access to the information. Both the IoT and the mobile components interface with this application. The mobile component behaves as an administrator, in can add or delete people and list the people registered. The IoT component simply sends the images taken from the door and receives a match=true/false. 

Our server application also makes use of [Azure Face API](https://azure.microsoft.com/en-us/services/cognitive-services/face/). This service from Azure does the machine learning from us. When a new person is added, our application sends the images to the Face API and an algorithm is trained. Once we have registered the users, an image that comes from the IoT component is sent to this same service and compared with the preloaded ones. The API service compares the incoming image with all the preloaded ones and returns a confidence parameter for each comparison. Our server application loops through these results and if one of them passes a threshold, we determine there is a match and this is sent back to the IoT component. 

### Mobile Component
Our mobile component is a flutter application and it behaves as the administrator of the system. The goal is to have an access point to the API endpoints the Cloud and the IoT components are exposing. For development purposes, we have tested our applications using Postman to check that all the endpoints were working, but that is not a user-friendly interface so our mobile application fulfills this goal. The owner or people living in the house would have this application installed in their phones. The mobile application has the following functionality
* Add and delete people to the system
* List the registered people and one of their images
* Receive images from the camera
* Actuate the lock

## What has changed and what was not accomplished
Our team has been able to kept working in the same project without changes after COVID-19. In our checkpoint one, after a conversation with Gabe, we decided that including a mobile application to increase the operability of the system would be a great idea, that was the major change and we have been able to stick to the plan. 
We believe that the only point that has been affected is the demo presentation. It would have been nice to be in class and quickly place the camera and the door actuator and show case how when one of us arrives to the door the first time it does not open, and how after including that person as a registered user from our phone application and approaching the door again, the lock would open.

## Tasks Division
From day one we created a project in GitHub to keep track of our issues, you can have access to it in this same repo. We had some initial meetings to decide the main direction of the project, we created issues for the different functionalities we wanted and we included bullet points inside some of them to explain the detail. After that, we assigned the tasks to the different team members and we have been working on them, mainly asynchronously, although whenever it was necessary we have had video calls to clarify some points. Our main communication tool has been Slack, we have been posting updates on a weekly basis about the progress made, where we had problems, and where we needed some help/collaboration.

Sam has done the work on the IoT component and the mobile application. He has....

Alvaro has done the work on the Cloud component. He made the research about the Face API and learn how to use it, he created the server application to handle the business logic, interface with the Face API, and create another API to provide services to both the IoT component and the mobile component. 

Both of us have worked on brainstorming ideas, thinking of the system architecture, write documentation, and kept a great working (remote) environment and collaboration atmosphere.
