from flask import Flask, request, send_file
from flask_restful import Resource, Api
from picamera import PiCamera

app = Flask(__name__)
api = Api(app)
camera=PiCamera()

class Unlock(Resource):
    def get(self):
        print("unlock")
        return "unlock"

class CheckDoor(Resource):
    def get(self):
        #print("checkDoor")
        camera.capture("/home/pi/image1.jpg")
        #img_fd = open('/home/pi/image1.jpg','rb')
        return send_file("/home/pi/image1.jpg", mimetype='image/png')
        

api.add_resource(Unlock, '/unlock') # Route_1
api.add_resource(CheckDoor, '/checkdoor') # Route_2

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5002')
    print("past run")