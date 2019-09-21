#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sat Jan 19 17:03:01 2019

@author: diogo
"""
import os
#to use the new IEX cloud change this value to 'iexcloud-beta'
os.environ['IEX_API_VERSION']='iexcloud-sandbox'

#This is a test token, replace with real token when in full scale production
os.environ['IEX_TOKEN']='Tsk_df0426c99cc64421ac28ac2745944a03'
from datetime import datetime
from iexfinance.stocks import get_historical_data
from iexfinance.stocks import Stock
from iexfinance.account import get_usage
import iexfinance

#aa= get_usage()

import matplotlib.pyplot as plt

apple = Stock("MCD")
a=apple.get_financials()

start = datetime(2018,1,1)
end = datetime(2019,4,1)

df= get_historical_data("MCD",start,end)


xAxis = []
yAxis = []
for k,v in df.items():
    xAxis.append(k)
    yAxis.append(v['close'])

plt.plot(xAxis,yAxis,label ='tsla')
plt.show()