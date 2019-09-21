# -*- coding: utf-8 -*-
"""
Created on Thu May 16 15:48:20 2019

@author: JPERE
"""

import pandas as pd
import numpy as np
import pickle

from sklearn.model_selection import train_test_split

data = pd.read_csv('FacadeLoadTrainningData.csv')
#data = pd.read_csv('UniformLoadTrainningData.csv')

#convert inertia and area to SI units from mm4 and cm2
#data.iloc[:,8] = data.iloc[:,8]/1000000000000
#data.iloc[:,10]=data.iloc[:,10]/1000000000000
#data.iloc[:,9] = data.iloc[:,9]*0.0001
#data.iloc[:,11] = data.iloc[:,11]*0.0001
#

################
x = data.iloc[:,0:13].values
y = data.iloc[:,13].values

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)


from sklearn.metrics import mean_squared_error

from sklearn import ensemble

Boost = ensemble.GradientBoostingRegressor(n_estimators=500, max_depth=4, min_samples_split= 2,learning_rate=0.01, loss= 'ls')

Boost.fit(x_train,y_train)

pred2 =Boost.predict(x_test)

mean_squared_error(y_test,pred2)

pickle.dump(Boost,open('GradientFcd180.sav','wb'))
#pickle.dump(Boost,open('GradientUni180.sav','wb'))

###################################

x = data.iloc[:,0:13].values
y = data.iloc[:,14].values

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)


from sklearn.metrics import mean_squared_error

from sklearn import ensemble

Boost = ensemble.GradientBoostingRegressor(n_estimators=12500, max_depth=4, min_samples_split= 2,learning_rate=0.01, loss= 'ls')

Boost.fit(x_train,y_train)

pred2 =Boost.predict(x_test)

mean_squared_error(y_test,pred2)

pickle.dump(Boost,open('GradientFcd500.sav','wb'))
#pickle.dump(Boost,open('GradientUni500.sav','wb'))

####################################

x = data.iloc[:,0:13].values
y = data.iloc[:,15].values

x_train, x_test, y_train, y_test = train_test_split(x,y,test_size = 0.10)


from sklearn.metrics import mean_squared_error

from sklearn import ensemble

Boost = ensemble.GradientBoostingRegressor(n_estimators=500, max_depth=4, min_samples_split= 2,learning_rate=0.01, loss= 'ls')

Boost.fit(x_train,y_train)

pred2 =Boost.predict(x_test)

mean_squared_error(y_test,pred2)

pickle.dump(Boost,open('GradientFcd250.sav','wb'))
#pickle.dump(Boost,open('GradientUni250.sav','wb'))

delta2 = (pred2-y_test)*10