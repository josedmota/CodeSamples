# -*- coding: utf-8 -*-
"""
Created on Tue May 28 16:38:35 2019

@author: JPERE
"""

import pandas as pd
import numpy as np
import pickle

from sklearn.model_selection import train_test_split

data = pd.read_csv('FacadeLoadTrainningData.csv')
#data = pd.read_csv('UniformLoadTrainningData.csv')

x = data.iloc[:,0:13].values
y = data.iloc[:,13].values

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

from sklearn.metrics import mean_squared_error

from sklearn.neighbors import KNeighborsRegressor

Model = KNeighborsRegressor(n_neighbors=5,weights='distance',p=1)

Model.fit(x_train,y_train)

pred2 = Model.predict(x_test)
mean_squared_error(y_test,pred2)

pickle.dump(Model,open('KNNFcd180.sav','wb'))
#pickle.dump(Model,open('KNNUni180.sav','wb'))

################
x = data.iloc[:,0:13].values
y = data.iloc[:,14].values

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

from sklearn.metrics import mean_squared_error

from sklearn.neighbors import KNeighborsRegressor

Model = KNeighborsRegressor(n_neighbors=5,weights='distance',p=1)

Model.fit(x_train,y_train)

pred2 = Model.predict(x_test)
mean_squared_error(y_test,pred2)

pickle.dump(Model,open('KNNFcd500.sav','wb'))
#pickle.dump(Model,open('KNNUni500.sav','wb'))

###############

x = data.iloc[:,0:13].values
y = data.iloc[:,15].values

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)

from sklearn.metrics import mean_squared_error

from sklearn.neighbors import KNeighborsRegressor

Model = KNeighborsRegressor(n_neighbors=5,weights='distance',p=1)

Model.fit(x_train,y_train)

pred2 = Model.predict(x_test)
mean_squared_error(y_test,pred2)

pickle.dump(Model,open('KNNFcd250.sav','wb'))
#pickle.dump(Model,open('KNNUni250.sav','wb'))