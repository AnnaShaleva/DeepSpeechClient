import os
from flask import Flask, jsonify, request
import pandas as pd

app = Flask(__name__)

@app.route('/predict', methods=['Post'])
def apicall():
    
    print("Resived POST request...")

    try:
        f = request.files['file']
        filePath = "./data/" + secure_filename(f.filename)
        f.save(filePath)
    except Exception as e:
        raise e

    print("File has been saved...")

    responce = jsonify({ 'text': 'Success!!!')
    responce.status_code = 200

    return(responce)

@app.errorhandler(400)
def bad_request(error=None):
    message = {
                'status': 400,
                'message': 'Bad Request: ' + request.url + '--> Please, check your data payload...',
            }
    resp = jsonify(message)
    resp.status_code = 400

    return resp
