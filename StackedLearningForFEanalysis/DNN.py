# -*- coding: utf-8 -*-
"""
Created on Wed May 15 09:59:43 2019

@author: JPERE
"""

import pandas as pd
import numpy as np

import os
os.environ["CUDA_DEVICE_ORDER"] = '-1'
os.environ["CUDA_VISIBLE_DEVICES"] = ""
from sklearn.model_selection import train_test_split


data = pd.read_csv('FacadeLoadTrainningData.csv')

#convert inertia and area to SI units from mm4 and cm2
#data.iloc[:,8] = data.iloc[:,9]/1000000000000
#data.iloc[:,10] = data.iloc[:,11]/1000000000000
#data.iloc[:,9] = data.iloc[:,10]*0.0001
#data.iloc[:,11] = data.iloc[:,12]*0.0001
#

x = data.iloc[:,0:13].values
y = data.iloc[:,13:16].values
#y = data.iloc[:,14]
x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.010)



from keras.models import Sequential
from keras.layers import Dense, Activation
import keras

import tensorflow as tf

import matplotlib.pyplot as plt

model = Sequential()
model.add(Dense(13, input_dim=13))

###
#model.add(Dense(150,activation = 'relu'))
#model.add(Dense(140,activation = 'relu'))
#model.add(Dense(130,activation = 'relu'))
###
#model.add(Dense(130,activation = 'sigmoid'))
#model.add(Dense(120,activation = 'relu'))
### 
#model.add(Dense(110,activation = 'sigmoid'))
#model.add(Dense(100,activation = 'relu'))
###
#model.add(Dense(90,activation = 'softmax'))
#model.add(Dense(80,activation = 'relu'))

###

model.add(Dense(80,activation = 'relu'))
model.add(Dense(70,activation = 'relu'))
model.add(Dense(60,activation = 'relu'))
model.add(Dense(50,activation = 'relu'))
model.add(Dense(40,activation = 'relu'))
model.add(Dense(30,activation = 'relu'))
model.add(Dense(15,activation = 'relu'))
model.add(Dense(3))

model.compile(optimizer='adam', loss ='mean_squared_error', metrics=[keras.metrics.mae,keras.metrics.mape,keras.metrics.cosine_proximity])#accuracy



#Callbacks to tensorboard for visualization
callbacks = [keras.callbacks.TensorBoard(log_dir='./logs')]
#training
history = model.fit(x_train,y_train,epochs=500, batch_size = 100, callbacks = callbacks)


############################
###Save model
model.save('./TrainedModels/FacadeLoadTrainedModel.h5')

pred=model.predict(x_test)
aa = abs(pred)-abs(y_test)
