from picamera import PiCamera
from time import sleep
import RPi.GPIO as GPIO
import requests

GPIO.setmode(GPIO.BCM)
GPIO.setup(12, GPIO.IN)
GPIO.setup(19, GPIO.OUT)
prev_input=0
camera=PiCamera()

def actuate():
    #I have to order a motor controller chip. RasPI GPIO only outputs 3.3V.
    print("Actuated")
    #move to unlocked "state"
    unlocked()
    
def unlocked():
    sleep(30)
    #then relock

while True:
    input = GPIO.input(12)
    if((not prev_input) and input):
        GPIO.output(19, GPIO.HIGH)
        #button pressed; take photo and seek confirmation
        sleep(2)
        #camera.capture("/home/pi/image1.jpg")
        img_fd = open('/home/pi/image.jpg','rb')
        files = {'image': img_fd}
        post_response = requests.post(url='https://gw-iot-facerecognitionserver.azurewebsites.net/facerecognition/FaceMatch', files=files)
        print("posted")
        print(post_response.text)
        if(post_response.text == "true"):
            actuate()
    prev_input=input
    sleep(.05)