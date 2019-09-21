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
modelUniform = load_model('./TrainedModels/UniformLoadTrainedModel.h5')
modelFacade = load_model('./TrainedModels/FacadeLoadTrainedModel.h5')


##Load GBoost models for UDL Load
UniLoad180Model = pickle.load(open('./Serving/TrainedModels/UniLoad180.sav', 'rb'))
UniLoad500Model = pickle.load(open('./Serving/TrainedModels/UniLoad500.sav', 'rb'))
UniLoad250Model = pickle.load(open('./Serving/TrainedModels/UniLoad250.sav', 'rb'))

#Load GBoos models for Facade Load
FcdLoad180Model = pickle.load(open('./Serving/TrainedModels/FcdLoad180.sav', 'rb'))
FcdLoad500Model = pickle.load(open('./Serving/TrainedModels/FcdLoad180.sav', 'rb'))
FcdLoad250Model = pickle.load(open('./Serving/TrainedModels/FcdLoad180.sav', 'rb'))

#Flask server
app = Flask(__name__)

@app.route("/")
def home():

    return "Usage: </br>Go to: /Size?Med=ValueHere&Ned=ValueHere</br> OR </br> With Post requests stating Med and Ned"

#@app.route("/Size")
#def Size():
 #   Med=float(request.args.get('Med'))
  #  Ned=float(request.args.get('Ned'))
   # return str(getUFactor(Med,Ned))

# Deflection predicted using Neural net model for uniform loads
@app.route('/NNCalcUniformLoad',methods=['POST'])
def NNPredictDeflFromUDL():
    
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
    
    ##Vects to predict
    with graph.as_default():
        DeadDfl180 = TotalDead * modelUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][0]
        DeadDfl500 = TotalDead * modelUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][1]
        DeadDfl250 = TotalDead * modelUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][2]
        
        #LiveLoad
        LiveDfl180 = LLReduction * LiveLoad * modelUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][0]
        LiveDfl500 = LLReduction * LiveLoad * modelUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][1]
        LiveDfl250 = LLReduction * LiveLoad * modelUniform.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][2]
        
    if LiveLoad>0:
        returnPacket = {
                'Span180':str(DeadDfl180+LiveDfl180),
                'Span500':str(DeadDfl500+LiveDfl500),
                'Span250':str(DeadDfl250+LiveDfl250),
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
    
    ##Vects to Predict
    with graph.as_default():
        Dfl180 = FacadeLoad * modelFacade.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][0]
        Dfl250 = FacadeLoad * modelFacade.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][2]
        Dfl500 = FacadeLoad * modelFacade.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0][1]
    
    print(FacadeLoad)
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
#Deflection prediccted using Gradient boosting model for uniform loads
@app.route('/XGCalcUniformLoad',methods=['POST'])
def PredictDeflFromUDL():
    
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
    
    ##Vects to predict
    
    #DeadLoad
    TotalDead = DeadLoad+Finishes+SDL
    
    DeadDfl180 = TotalDead * UniLoad180Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    DeadDfl250 = TotalDead * UniLoad250Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    DeadDfl500 = TotalDead * UniLoad500Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    
    #LiveLoad
    LiveDfl180 = LLReduction * LiveLoad * UniLoad180Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    LiveDfl250 = LLReduction * LiveLoad * UniLoad250Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    LiveDfl500 = LLReduction * LiveLoad * UniLoad500Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,1.0,0.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    
    #output
    if LiveLoad>0:
        returnPacket ={
                'Span180':str(DeadDfl180+LiveDfl180),
                'Span500':str(DeadDfl500+LiveDfl500),
                'Span250':str(DeadDfl250+LiveDfl250),
                }
    elif TotalDead>0:
        returnPacket ={
                'Span180':str(DeadDfl180),
                'Span500':str(DeadDfl500),
                'Span250':str(DeadDfl250),
                }
    else:
        returnPacket ={
                'Span180':str(0.0),
                'Span500':str(0.0),
                'Span250':str(0.0),
                }
            
    
    return jsonify(returnPacket) 

@app.route('/XGCalcFacadeLoad',methods = ['Post'])
def PredictDeflFromFacadeLoad():
    
    data = request.get_json()
    print(data)
    
    Dir = data['Dir']
    xSpan = data['xSpan']
    ySpan = data['ySpan']
    xCantilever = data['xCantilever']
    Subdiv = data['Subdiv']
    FacadeLoad = data['FacadeLoad']
    PrimSectI = data['PrimSectI']*1000000000000
    PrimSectA = data['PrimSectA']/0.0001
    SecSectI = data['SecSectI']*1000000000000
    SecSectA = data['SecSectA']/0.0001
    
    ##Vects to Predict
    
    Dfl180 = FacadeLoad * FcdLoad180Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    Dfl250 = FacadeLoad * FcdLoad250Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    Dfl500 = FacadeLoad * FcdLoad500Model.predict(np.array([[Dir,xSpan,ySpan,xCantilever,Subdiv,0.0,0.0,0.0,1.0,PrimSectI,PrimSectA,SecSectI,SecSectA]]))[0]
    
    print(FacadeLoad)
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