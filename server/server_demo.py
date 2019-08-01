from __future__ import absolute_import, division, print_function
from timeit import default_timer as timer
import sys, os

from flask import Flask, jsonify, request, json, Response
from werkzeug import secure_filename

import pandas as pd
import argparse
import subprocess
import scipy.io.wavfile as wav
import numpy as np

from deepspeech.model import Model

#import pyfreeling
#import analyzer


# These constants control the beam search decoder

# Beam width used in the CTC decoder when building candidate transcriptions
BEAM_WIDTH = 500

# The alpha hyperparameter of the CTC decoder. Language Model weight
LM_WEIGHT = 1.75

# The beta hyperparameter of the CTC decoder. Word insertion weight (penalty)
WORD_COUNT_WEIGHT = 1.00

# Valid word insertion weight. This is used to lessen the word insertion penalty
# when the inserted word is part of the vocabulary
VALID_WORD_COUNT_WEIGHT = 1.00


# These constants are tied to the shape of the graph used (changing them changes
# the geometry of the first layer), so make sure you use the same constants that
# were used during training

# Number of MFCC features to use
N_FEATURES = 26

# Size of the context window used for producing timesteps in the input vector
N_CONTEXT = 9

def load_model():
    model_path = 'output_graph.pb'
    alphabet_path = 'alphabet.txt'
    lm_path = 'lm.binary'
    trie_path = 'trie'

    ds = Model(model_path, N_FEATURES, N_CONTEXT, alphabet_path, BEAM_WIDTH)
   
    ds.enableDecoderWithLM(alphabet_path, lm_path, trie_path, LM_WEIGHT, WORD_COUNT_WEIGHT, VALID_WORD_COUNT_WEIGHT)
    return ds

def convert_samplerate(input_audio_path, output_audio_path):

    p = subprocess.Popen(["ffmpeg",
                    "-i", input_audio_path,
                    "-acodec", "pcm_s16le",
                    "-ac", "1",
                    "-ar", "16000",
                    output_audio_path],
                    stdout=subprocess.PIPE,
                    stderr=subprocess.PIPE)
    out, err = p.communicate()    

    if p.returncode:
        raise RuntimeError('Ffmpeg returned non-zero status: {}'.format(err))
    
    fs, audio = wav.read(output_audio_path)
    return fs, audio

def predict(ds, audio_path, out_audio_path):
    #print('Reading audio...', file=sys.stderr)
    
    fs, audio = convert_samplerate(audio_path, out_audio_path)

    #print('Running inference.', file=sys.stderr)
    text = ds.stt(audio, fs)
    return text

ds_global = load_model()

app = Flask(__name__)

@app.route('/test', methods=['Get'])
def apitest():
    return('Test method')

@app.route('/predict', methods=['Post'])
def apicall():
    
    print("Resived POST request...")

    try:
        f = request.files['file']
        file_path = './data/' + secure_filename(f.filename)        
        f.save(file_path)
    except Exception as e:
        print(e);
        raise e
    print("File has been saved...")
  
    print("Getting transcript...")
    out_file_path = './data/' + 'audio_to_predict.wav'
    text = predict(ds_global, file_path, out_file_path)
    os.remove(out_file_path)
    print("Text:")
    print(text)
    print()
    #responce = jsonify({ 'text': text })
    #responce.status_code = 200

    return text

@app.errorhandler(400)
def bad_request(error=None):
    message = {
                'status': 400,
                'message': 'Bad Request: ' + request.url + '--> Please, check your data payload...',
            }
    resp = jsonify(message)
    resp.status_code = 400

    return resp
