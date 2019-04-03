from flask import Flask

app = Flask(__name__)

@app.route('/test', methods=['Get'])
def apitest():
    return('Test method')

@app.route('/predict', methods=['Post'])
def apicall():
    
    return 'text'


