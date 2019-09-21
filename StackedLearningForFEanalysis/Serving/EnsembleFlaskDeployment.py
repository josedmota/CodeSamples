# -*- coding: utf-8 -*-
"""
Created on Fri May 17 13:05:57 2019

@author: JPERE
"""


import pandas as pd
import numpy as np
from keras.models import load_model
import keras
import pickle

import os
os.environ["CUDA_DEVICE_ORDER"] = '-1'
os.environ["CUDA_VISIBLE_DEVICES"] = ""
import tensorflow as tf

from flask import Flask, request, jsonify

#Load model
global graph
graph = tf.get_default_graph()
##Load ensemble NNs
#Uniform Loading

EnsembleUniLoad180 = load_model('./Serving/TrainedModels/EnsembleUniLoad180.h5')
EnsembleUniLoad250 = load_model('./Serving/TrainedModels/EnsembleUniLoad250.h5')
EnsembleUniLoad500 = load_model('./Serving/TrainedModels/EnsembleUniLoad500.h5')

#Facade Loading

EnsembleFcdLoad180 = load_model('./TrainedModels/EnsembleFcdLoad180.h5')
EnsembleFcdLoad250 = load_model('./TrainedModels/EnsembleFcdLoad250.h5')
EnsembleFcdLoad500 = load_model('./TrainedModels/EnsembleFcdLoad500.h5')

## Load Boosting KNN and NN models ####
## Uniform loading

GradientUni180 = pickle.load(open('./Serving/TrainedModels/GradientUni180.sav','rb'))
GradientUni250 = pickle.load(open('./Serving/TrainedModels/GradientUni250.sav','rb'))
GradientUni500 = pickle.load(open('./Serving/TrainedModels/GradientUni500.sav','rb'))

KNNUni180 = pickle.load(open('./Serving/TrainedModels/KNNUni180.sav','rb'))
KNNUni250 = pickle.load(open('./Serving/TrainedModels/KNNUni250.sav','rb'))
KNNUni500 = pickle.load(open('./Serving/TrainedModels/KNNUni500.sav','rb'))

NNUniform = load_model('./Serving/TrainedModels/UniformLoadTrainedModel.h5')

## Facade loading
GradientFcd180 = pickle.load(open('./Serving/TrainedModels/GradientFcd180.sav','rb'))
GradientFcd250 = pickle.load(open('./Serving/TrainedModels/GradientFcd250.sav','rb'))
GradientFcd500 = pickle.load(open('./Serving/TrainedModels/GradientFcd500.sav','rb'))

KNNFcd180 = pickle.load(open('./Serving/TrainedModels/KNNFcd180.sav','rb'))
KNNFcd250 = pickle.load(open('./Serving/TrainedModels/KNNFcd250.sav','rb'))
KNNFcd500 = pickle.load(open('./Serving/TrainedModels/KNNFcd500.sav','rb'))

NNFcd = load_model('./Serving/TrainedModels/FacadeLoadTrainedModel.h5')

##TEMP(while we don't have ensemble for Facade):
#modelFacade = load_model('./TrainedModels/FacadeLoadTrainedModel.h5')

##Load GBoost models for UDL Load
#UniLoad180Model = pickle.load(open('./Serving/TrainedModels/UniLoad180.sav', 'rb'))
#UniLoad500Model = pickle.load(open('./Serving/TrainedModels/UniLoad500.sav', 'rb'))
#UniLoad250Model = pickle.load(open('./Serving/TrainedModels/UniLoad250.sav', 'rb'))
#
##Load GBoos models for Facade Load
#FcdLoad180Model = pickle.load(open('./Serving/TrainedModels/FcdLoad180.sav', 'rb'))
#FcdLoad500Model = pickle.load(open('./Serving/TrainedModels/FcdLoad180.sav', 'rb'))
#FcdLoad250Model = pickle.load(open('./Serving/TrainedModels/FcdLoad180.sav', 'rb'))

#Flask server
app = Flask(__name__)

@app.route("/")
def home():

    return "Usage: </br>Go to: /Size?Med=ValueHere&Ned=ValueHere</br> OR </br> With Post requests stating Med and Ned"


# Deflection predicted using Neural net model for uniform loads
@app.route('/NNCalcUniformLoad',methods=['POST'])
def NNPredictDeflFromUDL():
    
    ##Get Input parameters
    data = request.get_json()
    print(data)
    Dir = data['Dir']
    xSpan = data['xSpan']
    ySpan = data['ySpan']
    xCantilever = data['xCantilever']
    Subdiv = data['Subdiv']
    LiveLoad = data['LiveLoad']
    DeadLoad = data['DeadLoad']
    Finishes = data['Finishes']
    SDL = data['SDL']
    PrimSectI = data['PrimSectI']*1000000000000
    PrimSectA = data['PrimSectA']/0.0001
    SecSectI = data['SecSectI']*1000000000000
    SecSectA = data['SecSectA']/0.0001
    LLReduction = data['LLreduct']
    
    #DeadLoad
    TotalDead = DeadLoad+Finishes+SDL
    
    #Get predictions for the various ML Models
    
    #DeadLoad
    
    GradientDLPrediction180 = GradientUni180.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    GradientDLPrediction250 = GradientUni250.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    GradientDLPrediction500 = GradientUni500.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    
    KNNDLPrediction180 = KNNUni180.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    KNNDLPrediction250 = KNNUni250.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    KNNDLPrediction500 = KNNUni500.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    
    with graph.as_default():
        DeadDfl180 = NNUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][0]
        DeadDfl500 = NNUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][1]
        DeadDfl250 = NNUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][2]

    # LiveLoad
    GradientLLPrediction180 = GradientUni180.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    GradientLLPrediction250 = GradientUni250.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    GradientLLPrediction500 = GradientUni500.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    
    KNNLLPrediction180 = KNNUni180.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    KNNLLPrediction250 = KNNUni250.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    KNNLLPrediction500 = KNNUni500.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    
    with graph.as_default():
        LiveDfl180 = NNUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][0]
        LiveDfl500 = NNUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][1]
        LiveDfl250 = NNUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][2]

    
    ####Ensemble Prediction
    #Dead    
    Votes180 = np.transpose(np.vstack([DeadDfl180,GradientDLPrediction180[0],KNNDLPrediction180[0]]))
    Votes250 = np.transpose(np.vstack([DeadDfl250,GradientDLPrediction250[0],KNNDLPrediction250[0]]))
    Votes500 = np.transpose(np.vstack([DeadDfl500,GradientDLPrediction500[0],KNNDLPrediction500[0]]))
    
    with graph.as_default(): 
        DeadDfl180 = TotalDead * EnsembleUniLoad180.predict(Votes180)[0][0]
        DeadDfl250 = TotalDead * EnsembleUniLoad250.predict(Votes250)[0][0]
        DeadDfl500 = TotalDead * EnsembleUniLoad500.predict(Votes500)[0][0]
    
    #Live
    Votes180 = np.transpose(np.vstack([LiveDfl180,GradientLLPrediction180[0],KNNLLPrediction180[0]]))
    Votes250 = np.transpose(np.vstack([LiveDfl250,GradientLLPrediction250[0],KNNLLPrediction250[0]]))
    Votes500 = np.transpose(np.vstack([LiveDfl500,GradientLLPrediction500[0],KNNLLPrediction500[0]]))
    
    with graph.as_default():
        LLDfl180 = LiveLoad * LLReduction * EnsembleUniLoad180.predict(Votes180)[0][0]
        LLDfl250 = LiveLoad * LLReduction * EnsembleUniLoad250.predict(Votes250)[0][0]
        LLDfl500 = LiveLoad * LLReduction * EnsembleUniLoad500.predict(Votes500)[0][0]
    
    #compile results 
    if LiveLoad>0:
        returnPacket = {
                'Span180':str(DeadDfl180+LLDfl180),
                'Span500':str(DeadDfl500+LLDfl500),
                'Span250':str(DeadDfl250+LLDfl250),
                }
    else:
        returnPacket ={
                'Span180':str(DeadDfl180),
                'Span500':str(DeadDfl500),
                'Span250':str(DeadDfl250),
                }
        
    return jsonify(returnPacket) 

#Deflection predicted using neural net for Facade loading
@app.route('/NNCalcFacadeLoad',methods=['POST'])
def NNPredictDeflFromFacadeLoad():
    
    ##Get Input parameters
    data = request.get_json()
    print(data)
    
    Dir = data['Dir']
    xSpan = data['xSpan']
    ySpan = data['ySpan']
    xCantilever = data['xCantilever']
    Subdiv = data['Subdiv']
    FacadeLoad = data['FacadeLoad']
    PrimSectI = data['PrimSectI'] * 1000000000000
    PrimSectA = data['PrimSectA'] / 0.0001
    SecSectI = data['SecSectI'] * 1000000000000
    SecSectA = data['SecSectA'] / 0.0001
    
    
    #Get prediction for various ML models
    GradientPrediction180 = GradientFcd180.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    GradientPrediction250 = GradientFcd250.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    GradientPrediction500 = GradientFcd500.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    
    KNNPrediction180 = KNNFcd180.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    KNNPrediction250 = KNNFcd250.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    KNNPrediction500 = KNNFcd500.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))
    
    with graph.as_default():
        NNPrediction180 = NNFcd.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][0]
        NNPrediction250 = NNFcd.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][2]
        NNPrediction500 = NNFcd.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][1]
    
    #Ensemble prediction
    Votes180 = np.transpose(np.vstack([NNPrediction180,GradientPrediction180[0],KNNPrediction180[0]]))
    Votes250 = np.transpose(np.vstack([NNPrediction250,GradientPrediction250[0],KNNPrediction250[0]]))
    Votes500 = np.transpose(np.vstack([NNPrediction500,GradientPrediction500[0],KNNPrediction500[0]]))
    
    with graph.as_default(): 
        Dfl180 = FacadeLoad * EnsembleFcdLoad180.predict(Votes180)[0][0]
        Dfl250 = FacadeLoad * EnsembleFcdLoad250.predict(Votes250)[0][0]
        Dfl500 = FacadeLoad * EnsembleFcdLoad500.predict(Votes500)[0][0]
    
    
    if FacadeLoad>0:
        returnPacket ={
                'Span180':str(Dfl180),
                'Span500':str(Dfl500),
                'Span250':str(Dfl250),
                }
    else:
        returnPacket ={
                'Span180':str(0.0),
                'Span500':str(0.0),
                'Span250':str(0.0),
                }
    
    return jsonify(returnPacket)


app.run(threaded=True, debug=False)