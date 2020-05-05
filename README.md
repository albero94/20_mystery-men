# 20_mystery-men
Sam Frey (github: samfrey99)

Alvaro Albero (github: albero94)

## Description
We have created a smart door application that uses face recognition. The basic description of the service is, we have a cloud application that contains the list and images of people that are allowed into the house. We have a mobile application that is used as an administrator and can add and delete people from the list and operate the door lock directly. We have an IoT system with a Raspberry PI, a camera and a lock actuator that will take a picture of the person trying to get into the house, send it to the cloud service, and operate the door if there is a match in the face.

* [Face Recognition Server](./FaceRecognitionServer) contains the cloud service
* [Android](./android) contains the mobile application
* [PI](./pi) contains the Raspberry PI code to manage the actuators
* [Deployment](./deployment) contans the files to deploy the project
* A short demo video can be found [here](https://drive.google.com/open?id=1d16epz6AwzkoG-WXnCZSgXjU2IlyBvqN).

## Deployment
If you want to use our application, you need the following components. A hosting environment to deploy the .NET server, an Azure Face API subscription (basic is free), an android device to administrate the system, a camera, a door lock, and a Raspberry PI to operate both of them.

We have included a [Deployment](./deployment) folder where you can find the three projects (Cloud, Web and PI).

To deploy the cloud application
* You need to copy the files in FaceRecognitionServer to the hosting provider of your choice, we have used Azure App Services
* You need to register to [Azure Face API](https://azure.microsoft.com/en-us/services/cognitive-services/face/) to get the API keys, then include them as environment variables named AZURE_FACE_SUBSCRIPTION_KEY and AZURE_FACE_ENDPOINT
* Your cloud service is ready to use

To deploy the mobile application
* Download the [APK](./deployment/android/FaceMatch-Door-Lock.apk) and install on an Android Device, or build the [project](./android) in Android Studio and run in an emulator.

To deploy the IoT component
* Download [actuator.py](./deployment/pi/actuator.py) and run on a Raspberry Pi.
* A wiring example can be found [here](./deployment/pi/wiring.JPG)
