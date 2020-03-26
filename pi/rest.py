from flask import Flask, request
from flask_restful import Resource, Api

app = Flask(__name__)
api = Api(app)

class Unlock(Resource):
    def get(self):
        print("unlock")
        return "unlock"

class CheckDoor(Resource):
    def get(self):
        print("checkDoor")
        return "photo"
        

api.add_resource(Unlock, '/unlock') # Route_1
api.add_resource(CheckDoor, '/checkdoor') # Route_2

if __name__ == '__main__':
    app.run(host='0.0.0.0', port='5002')