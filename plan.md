# Project work distribution & timeline
Sam Frey (github: samfrey99)

Alvaro Albero (github: albero94)

## Brief project description
Our project consists on creating a smart door system that uses face recognition to determine if the door lock should open or not. 

## Components breakdown
As we have seen in class, every IoT system consists of three main components. Sensors, computing capabilities with a network connection, and actuators.

### Sensor
Our sensor is a *camera*. The role of the camera is to record images of the people approaching the door and send them to the computing unit it is connected.

### Computing capabilities
Our computing capabilities are divided in two places. First we have a simple computer, probably a *Raspberry Pi* located close to the camera. The role of this component is to receive the images of the camera and send them through an internet connection to the cloud. In the cloud, the real processing is done. The images received by the camera are compared to the images previously stored of the people with entrance permission. A response is sent back to the Raspberry Pi allowing or not the entrance. The Raspberri Pi receives this message and if entrance is permitted it will notify the actuator to open the door.

### Actuator
The actuator is a *lock* that is controlled by the Raspberry Pi. The Raspberry Pi based on the information received by the cloud will simple send a signal to open or close the lock.

### Connections
The Raspberry Pi will have access to the internet through Wi-Fi and will be able to communicate with the cloud service.

The camera and Raspberry Pi could communicate via Bluetooth, Wi-Fi (without accessing the internet just LAN), or via cable. We will have a better knowledge once we know which camera we are acquiring.

Similar situation with the lock, probably a Bluetooth connection is available but we need to do some research on which locks are available.

## Interfaces
### Camera and Raspberry Pi
We will use Raspberry Pi's own [Camera Board v2](https://www.adafruit.com/product/3099) for its dedicated connector with the Raspberry Pi. At 8MP, it should be plenty of resolution for the face detection we hope to implement.

### Raspberry Pi and lock
Adafruit offers a motor [specifically designed for actuating a door lock](https://www.adafruit.com/product/3881). The motor only requires 5V to run, so the Raspberry Pi should be able to power the motor when required. The motor should fit most standard door locks, so the lock itself can be aquired from any hardware store.

### Raspberry Pi and cloud
The Raspberry Pi will have access to the internet and will connect to the cloud service through an API, could be a REST API. This API will allow the Raspberry Pi to send the data with the images that are processed in the cloud. The API will also allow to send a response stating if the door should be open or not. 

## Securing the Internet of Things
The article [Securing the Internet of Things](https://github.com/gwu-iot/collaboration/blob/master/papers/dhs16guidelines.pdf) emphasizes the importance of IoT security, to the point that is considered as homeland security, and explains six strategic principles that should be followed. In this section we present which principles we will apply in the project and which ones not, and explain why.

### Incorporate security at the design phase
Some of the actions we are planning to take here are changing default usernames and passwords and using an updated operating system in the Raspberry PI.

We are planning to use the hardware we already have, we are not sure about the security features it incorporates, and we think that the system and operation disruption does not really apply to us because in case the smart lock does not work we can always use the key to enter the house.

### Advance security updates and vulnerability management
We believe this section applies more to systems that have been deployed in large scale where software and hardware needs to be updated periodically. We are not planning to develop policies and security practices as the final users of the system will be ourselves, coordinate software updates among third-party vendors or automatic mechanisms for addressing vulnerabilities.

In our case, as a one semester project, we will only deploy the system in one controlled environment and we will be in fully control of any updates as we make progress.

### Build on recognized security practices
In this section the article recommends employing a holistic approach to secure the system and it is what we are actually doing, as we are taking security into consideration from the design phase using basic software security practices. Also, we will research security guidelines for systems with cameras and how to connect the Raspberry PI to the internet in a secured way. Part of this research will be done in cybersecurity information sharing platforms.

### Prioritize security measures according to potential impact
As developers and users, we know the device intended use and environment and we are nearly in full control of it. Also, our system has a limited and known number of devices so we will have them identified. 

We are only two people in the team and both of us will be part of the design of the system, so read-teaming may be a bit complicated but we will try to find any security flaw as we design, implement, and test the system. 

### Promote transparency across IoT
In our case, we can do a risk assessment of the elements we are using but other ideas like bug bounty programs and employing software bill of materials are out of scope for this project.

### Connect carefully and deliberately
We have not yet decided how all the system is going to be connect and every component will interact with each other, but we do not need our Raspberry PI, camera and lock to be connected to the cloud service 24/7, this service is only needed when a person is detected, so making only deliberate connections is something we will work on.


## Timeline
The first tasks would consist on acquiring the components and services and understand how to work with them. Once know how to do it, we will work on connecting them through the defined interfaces. Sam will focus on the physical device, while Alvaro will focus on integration with a cloud API for facial recognition, but as a group of two, most of the work will be completed together so that we may learn from each other.

### Checkpoint 1
* Acquire camera, understand how to install it and what type of output it gives
* Acquire smart lock, understand how to install it and which commands receives to operate it
* Look for a cloud service where we can store images and run a face recognition algorithm or service to match faces
* Acquire a Raspberry Pi, decide which OS we need in it and investigate how we can connect it with all the other components

### Checkpoint 2
* Establish the connection between the camera and the Raspberry Pi and be able to send images or video in a streaming way
* Establish the connection between the Raspberry Pi and the smart lock and be able to operate it from the computer
* Establish the connection between the cloud and the Raspberry Pi, be able to send the images or video through the API and receive response from the cloud

### Final Submission
* Connect the whole system and start making the first tests
* Make the necessary changes to improve the collection of images, transmission, face recognition and lock operation
* Try to extend the system by providing a way to add new images through a mobile application and also be able to operate the lock directly from the application, e.g. to open the door to a friend

## Required Materials
* Raspberry Pi and camera module (already aquired)
* Azure subscription (if a free trial isn't enough for the scope of this project)
* [Adafruit Rotating Door Lock DC Motor](https://www.adafruit.com/product/3881)
* [Lens Adjustment Tool for Raspberry Pi Camera](https://www.adafruit.com/product/3518)
* A Door Lock (deadbolt)
* An Actual Door (tenative for demo)
