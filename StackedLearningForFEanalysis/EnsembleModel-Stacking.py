# -*- coding: utf-8 -*-
"""
Created on Fri May 31 16:04:26 2019

@author: JPERE
"""

import pandas as pd
import numpy as np
import pickle 
import keras 
import tensorflow
from keras.models import load_model

import os
os.environ["CUDA_DEVICE_ORDER"] = '-1'
os.environ["CUDA_VISIBLE_DEVICES"] = ""
import tensorflow as tf

##Loading the models 

global graph
graph = tf.get_default_graph()

modelUniform = load_model('./TrainedModels/UniformLoadTrainedModel.h5')
modelFacade = load_model('./TrainedModels/FacadeLoadTrainedModel.h5')

GradientUniLoad180Model = pickle.load(open('./GradientUni180.sav', 'rb'))
GradientUniLoad500Model = pickle.load(open('./GradientUni500.sav','rb'))
GradientUniLoad250Model = pickle.load(open('./GradientUni250.sav','rb'))


KNNUniLoad180Model = pickle.load(open('./KNNUni180.sav','rb'))
KNNUniLoad500Model = pickle.load(open('./KNNUni500.sav','rb'))
KNNUniLoad250Model = pickle.load(open('./KNNUni250.sav','rb'))


GradientFcdLoad180Model = pickle.load(open('./GradientFcd180.sav','rb'))
GradientFcdLoad500Model = pickle.load(open('./GradientFcd500.sav','rb'))
GradientFcdLoad250Model = pickle.load(open('./GradientFcd250.sav','rb'))

KNNFcdLoad180Model = pickle.load(open('./KNNFcd180.sav','rb'))
KNNFcdLoad500Model = pickle.load(open('./KNNFcd500.sav','rb'))
KNNFcdLoad250Model = pickle.load(open('./KNNFcd250.sav','rb'))

##############################################################
############Uniform Load Data and Model trainning#############


## Load train data
data = pd.read_csv('UniformLoadTrainningData.csv')

x= data.iloc[:,0:13]
y= data.iloc[:,13]
#Train test split

from sklearn.model_selection import train_test_split

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

###############
##Make predictions in all the models for L180
###############

resultNN = modelUniform.predict(x_train)[:,0]
resultGradient = GradientUniLoad180Model.predict(x_train)
resultKNN = KNNUniLoad180Model.predict(x_train)
##

NNInputs = np.transpose(np.vstack([resultNN,resultGradient,resultKNN]))

##Build the agregator NN for our stacked model for L180 uniform predictions
from keras.models import Sequential
from keras.layers import Dense, Activation
import keras
import tensorflow as tf

import matplotlib.pyplot as plt

model = Sequential()
model.add(Dense(3, input_dim=3))

model.add(Dense(20,activation='relu'))

model.add(Dense(1))

model.compile(optimizer='adam', loss ='mean_squared_error', metrics=[keras.metrics.mae,keras.metrics.mape,keras.metrics.cosine_proximity])#accuracy

callbacks = [keras.callbacks.TensorBoard(log_dir='./logs')]
#training
history = model.fit(NNInputs,y_train,epochs=200, batch_size = 250, callbacks = callbacks)

predNN = modelUniform.predict(x_test)[:,0]
PredGr = GradientUniLoad180Model.predict(x_test)
PredKNN = KNNUniLoad180Model.predict(x_test)

#Assemble predict array
Votes = np.transpose(np.vstack([predNN,PredGr,PredKNN]))

#Predict using ensamble NN

final = model.predict(Votes)

resultss = abs(final[:,0])-abs(y_test)

model.save('./TrainedModels/EnsembleUniLoad180.h5')

###################
##Make Predictions in all models for L500
###################

x= data.iloc[:,0:13]
y= data.iloc[:,14]
#Train test split

from sklearn.model_selection import train_test_split

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

##Make predictions in all the models for L500
resultNN = modelUniform.predict(x_train)[:,1]
resultGradient = GradientUniLoad500Model.predict(x_train)
resultKNN = KNNUniLoad500Model.predict(x_train)
##

NNInputs = np.transpose(np.vstack([resultNN,resultGradient,resultKNN]))

##Build the agregator NN for our stacked model for L500 uniform predictions
from keras.models import Sequential
from keras.layers import Dense, Activation
import keras
import tensorflow as tf

import matplotlib.pyplot as plt

model = Sequential()
model.add(Dense(3, input_dim=3))

model.add(Dense(2,activation='relu'))

model.add(Dense(1))

model.compile(optimizer='adam', loss ='mean_squared_error', metrics=[keras.metrics.mae,keras.metrics.mape,keras.metrics.cosine_proximity])#accuracy

callbacks = [keras.callbacks.TensorBoard(log_dir='./logs')]
#training
history = model.fit(NNInputs,y_train,epochs=200, batch_size = 100, callbacks = callbacks)

predNN = modelUniform.predict(x_test)[:,1]
PredGr = GradientUniLoad500Model.predict(x_test)
PredKNN = KNNUniLoad500Model.predict(x_test)

#Assemble predict array
Votes = np.transpose(np.vstack([predNN,PredGr,PredKNN]))

#Predict using ensamble NN

final = model.predict(Votes)

resultss = abs(final[:,0])-abs(y_test)

model.save('./TrainedModels/EnsembleUniLoad500.h5')

###################
##Make Predictions in all models for L250
###################

x= data.iloc[:,0:13]
y= data.iloc[:,15]
#Train test split

from sklearn.model_selection import train_test_split

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

##Make predictions in all the models for L250
resultNN = modelUniform.predict(x_train)[:,2]
resultGradient = GradientUniLoad250Model.predict(x_train)
resultKNN = KNNUniLoad250Model.predict(x_train)
##

NNInputs = np.transpose(np.vstack([resultNN,resultGradient,resultKNN]))

##Build the agregator NN for our stacked model for L250 uniform predictions
from keras.models import Sequential
from keras.layers import Dense, Activation
import keras
import tensorflow as tf

import matplotlib.pyplot as plt

model = Sequential()
model.add(Dense(3, input_dim=3))

model.add(Dense(10,activation='relu'))

model.add(Dense(1))

model.compile(optimizer='adam', loss ='mean_squared_error', metrics=[keras.metrics.mae,keras.metrics.mape,keras.metrics.cosine_proximity])#accuracy

callbacks = [keras.callbacks.TensorBoard(log_dir='./logs')]
#training
history = model.fit(NNInputs,y_train,epochs=200, batch_size = 50, callbacks = callbacks)

predNN = modelUniform.predict(x_test)[:,2]
PredGr = GradientUniLoad250Model.predict(x_test)
PredKNN = KNNUniLoad250Model.predict(x_test)

#Assemble predict array
Votes = np.transpose(np.vstack([predNN,PredGr,PredKNN]))

#Predict using ensamble NN

final = model.predict(Votes)

resultss = abs(final[:,0])-abs(y_test)

model.save('./TrainedModels/EnsembleUniLoad250.h5')


#############################################################
############                                    #############
########### Facade Load Data and Model trainning ############
############                                    #############
#############################################################

#TODO:


## Load train data
data = pd.read_csv('FacadeLoadTrainningData.csv')

#########################L180################################
x= data.iloc[:,0:13]
y= data.iloc[:,13]
#Train test split

from sklearn.model_selection import train_test_split

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

###############
##Make predictions in all the models for L180
###############

resultNN = modelFacade.predict(x_train)[:,0]
resultGradient = GradientFcdLoad180Model.predict(x_train)
resultKNN = KNNFcdLoad180Model.predict(x_train)
##

NNInputs = np.transpose(np.vstack([resultNN,resultGradient,resultKNN]))


##Build the agregator NN for our stacked model for L180 uniform predictions
from keras.models import Sequential
from keras.layers import Dense, Activation
import keras
import tensorflow as tf

import matplotlib.pyplot as plt

model = Sequential()
model.add(Dense(3, input_dim=3))

model.add(Dense(20,activation='relu'))

model.add(Dense(1))

model.compile(optimizer='adam', loss ='mean_squared_error', metrics=[keras.metrics.mae,keras.metrics.mape,keras.metrics.cosine_proximity])#accuracy

callbacks = [keras.callbacks.TensorBoard(log_dir='./logs')]

#training
history = model.fit(NNInputs,y_train,epochs=200, batch_size = 250, callbacks = callbacks)

predNN = modelFacade.predict(x_test)[:,0]
PredGr = GradientFcdLoad180Model.predict(x_test)
PredKNN = KNNFcdLoad180Model.predict(x_test)

#Assemble predict array
Votes = np.transpose(np.vstack([predNN,PredGr,PredKNN]))

#Predict using ensamble NN

final = model.predict(Votes)

resultss = abs(final[:,0])-abs(y_test)

model.save('./TrainedModels/EnsembleFcdLoad180.h5')


#########################L500################################
x= data.iloc[:,0:13]
y= data.iloc[:,14]
#Train test split

from sklearn.model_selection import train_test_split

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

###############
##Make predictions in all the models for L500
###############

resultNN = modelFacade.predict(x_train)[:,0]
resultGradient = GradientFcdLoad500Model.predict(x_train)
resultKNN = KNNFcdLoad500Model.predict(x_train)
##

NNInputs = np.transpose(np.vstack([resultNN,resultGradient,resultKNN]))


##Build the agregator NN for our stacked model for L500 uniform predictions
from keras.models import Sequential
from keras.layers import Dense, Activation
import keras
import tensorflow as tf

import matplotlib.pyplot as plt

model = Sequential()
model.add(Dense(3, input_dim=3))

model.add(Dense(20,activation='relu'))

model.add(Dense(1))

model.compile(optimizer='adam', loss ='mean_squared_error', metrics=[keras.metrics.mae,keras.metrics.mape,keras.metrics.cosine_proximity])#accuracy

callbacks = [keras.callbacks.TensorBoard(log_dir='./logs')]

#training
history = model.fit(NNInputs,y_train,epochs=200, batch_size = 250, callbacks = callbacks)

predNN = modelFacade.predict(x_test)[:,0]
PredGr = GradientFcdLoad500Model.predict(x_test)
PredKNN = KNNFcdLoad500Model.predict(x_test)

#Assemble predict array
Votes = np.transpose(np.vstack([predNN,PredGr,PredKNN]))

#Predict using ensamble NN

final = model.predict(Votes)

resultss = abs(final[:,0])-abs(y_test)

model.save('./TrainedModels/EnsembleFcdLoad500.h5')


#########################L250################################
x= data.iloc[:,0:13]
y= data.iloc[:,15]
#Train test split

from sklearn.model_selection import train_test_split

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

###############
##Make predictions in all the models for L250
###############

resultNN = modelFacade.predict(x_train)[:,0]
resultGradient = GradientFcdLoad250Model.predict(x_train)
resultKNN = KNNFcdLoad250Model.predict(x_train)
##

NNInputs = np.transpose(np.vstack([resultNN,resultGradient,resultKNN]))


##Build the agregator NN for our stacked model for L250 uniform predictions
from keras.models import Sequential
from keras.layers import Dense, Activation
import keras
import tensorflow as tf

import matplotlib.pyplot as plt

model = Sequential()
model.add(Dense(3, input_dim=3))

model.add(Dense(20,activation='relu'))

model.add(Dense(1))

model.compile(optimizer='adam', loss ='mean_squared_error', metrics=[keras.metrics.mae,keras.metrics.mape,keras.metrics.cosine_proximity])#accuracy

callbacks = [keras.callbacks.TensorBoard(log_dir='./logs')]

#training
history = model.fit(NNInputs,y_train,epochs=200, batch_size = 250, callbacks = callbacks)

predNN = modelFacade.predict(x_test)[:,0]
PredGr = GradientFcdLoad250Model.predict(x_test)
PredKNN = KNNFcdLoad250Model.predict(x_test)

#Assemble predict array
Votes = np.transpose(np.vstack([predNN,PredGr,PredKNN]))

#Predict using ensamble NN

final = model.predict(Votes)

resultss = abs(final[:,0])-abs(y_test)

model.save('./TrainedModels/EnsembleFcdLoad250.h5')