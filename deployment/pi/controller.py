from picamera import PiCamera
from time import sleep
import RPi.GPIO as GPIO
import requests
from requests.auth import HTTPBasicAuth
from flask import Flask, request, send_file, jsonify
from flask_restful import Resource, Api
from picamera import PiCamera
import threading

app = Flask(__name__)
api = Api(app)
camera=PiCamera()
lock = threading.Lock()

class Unlock(Resource):
    def get(self):
        if(verify()):
            unlocked()
        return jsonify({"message":"UNAUTHORIZED"})

class CheckDoor(Resource):
    def get(self):
        if(verify()):
            lock.acquire()
            camera.capture("/home/pi/image1.jpg")
            lock.release()
            return send_file("/home/pi/image1.jpg", mimetype='image/png')
        return jsonify({"message":"UNAUTHORIZED"})

def verify():
    headers = request.headers
    auth = headers.get("ApiKey")
    if(auth == "MySecretKey"):
        return True
    else: return False

GPIO.setmode(GPIO.BCM)
GPIO.setwarnings(False)
GPIO.setup(12, GPIO.IN)
GPIO.setup(19, GPIO.OUT)
GPIO.setup(6, GPIO.OUT)
GPIO.output(19, GPIO.LOW)
GPIO.output(6, GPIO.LOW)
prev_input=0

def actuate():
    #I have to order a motor controller chip. RasPI GPIO only outputs 3.3V.
    print("Actuated")
    #move to unlocked "state"
    unlocked()
    
def unlocked():
    GPIO.output(19, GPIO.HIGH)
    sleep(2)
    GPIO.output(19, GPIO.LOW)
    #then relock
    
def rest():
    api.add_resource(Unlock, '/unlock') # Route_1
    api.add_resource(CheckDoor, '/checkdoor') # Route_2
    app.run(host='0.0.0.0', port='5002')

t1 = threading.Thread(target=rest, name='t1')
t1.start()

while True:
    input = GPIO.input(12)
    if((not prev_input) and input):
        #button pressed; take photo and seek confirmation
        lock.acquire()
        sleep(2)
        camera.capture("/home/pi/image1.jpg")
        lock.release()
        img_fd = open('/home/pi/image1.jpg','rb')
        file = {'file': img_fd}
        post_response = requests.post(url='https://gw-iot-facerecognitionserver.azurewebsites.net/facerecognition/FaceMatch', files=file, headers={'ApiKey':'MySecretKey'})
        print("posted")
        print(post_response.text)
        if(post_response.text == "true"):
            actuate()
        else:
            GPIO.output(6, GPIO.HIGH)
            sleep(2)
            GPIO.output(6, GPIO.LOW)
    prev_input=input
    sleep(.05)