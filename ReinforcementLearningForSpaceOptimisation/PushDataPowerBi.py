# -*- coding: utf-8 -*-
"""
Created on Fri Aug 24 09:41:27 2018

@author: JPERE
"""
import requests
import time
import json
from datetime import datetime as dt


REST_API_URL = "https://api.powerbi.com/beta/c8823c91-be81-4f89-b024-6c3dd789c106/datasets/8dff5d45-8ecf-45f8-bb66-9ee6b8ffdf66/rows?key=o8tdxfzHGGKc3DBuKHoEkpx%2Bc6rFWoNgyKE%2Fic5ULd6dLDLNQdY5coKLhLsG5NL1KH81AKAUVKsWg%2BzzTyI9Og%3D%3D"

def pushToServer(a,b):
    data = [{
            'Loss': a,
            'Fitness':b,
            'timestep':dt.strftime(dt.now(),"%Y-%m-%dT%H:%M:%S")
            }]
    
    data =bytes(json.dumps(data),'utf-8');
    #print(data)
    req = requests.post(REST_API_URL,data,verify = False)
   
    #print(req.status_code)

########for debug uncomment
#i=0
#dt= datetime.datetime
#ts=dt.strftime(dt.now(),"%Y-%m-%dT%H:%M:%S")

#while(True):
#    i +=1
#    print(ts)
#    ts = dt.strftime(dt.now(),"%Y-%m-%dT%H:%M:%S")
#    pushToServer(i,i*0.95,ts)
#    time.sleep(2)



    