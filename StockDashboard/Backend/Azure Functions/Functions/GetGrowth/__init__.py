import logging
import azure.functions as func
import os
import json
import sys

from __app__.SharedCode import cacheserver as CacheConnection

import iexfinance.stocks as Stock
from iexfinance.stocks import get_historical_data
import decimal
from datetime import datetime

#go forwards or backwards delta months (works with months w/0 31st 30th etc)
def monthdelta(date, delta):
    m, y = (date.month+delta) % 12, date.year + ((date.month)+delta-1) // 12
    if not m: m = 12
    d = min(date.day, [31,
        29 if y%4==0 and not y%400==0 else 28,31,30,31,30,31,31,30,31,30,31][m-1])
    return date.replace(day=d,month=m, year=y)


def CalculateGrowth(data):
    prevPrice=data['close'][0]
    current=data['close'][-1]
    growth= ((current/prevPrice)-1)*100
    return float(round(growth,2))


def GetGrowth(name,delta):
        end=datetime.now()
        start=monthdelta(end,int(delta)*-1)
        data = get_historical_data(name,start,end,output_format='pandas')
        result = CalculateGrowth(data)
        return str(result) + " %"

def BuildRequestString(name,period):
        return "/GetGrowth/{0}/{1}".format(name,period)

def InnerCall(name,period):
    Req = BuildRequestString(name,period)

    logging.info("request: " + str(Req))

    data = CacheConnection.GetFromCache(Req,1)

    #return from cache
    if data is not None:
        logging.info("Returning From Cache")
        return func.HttpResponse(str(data),headers=Header)
    
    # else make the api call
    result =GetGrowth(str(name),period)
    
    #build Json Objs
    data = {}
    data['Growth'] = str(result)
    result = json.dumps(data,skipkeys=False)

    logging.info("Writing to Cache")
    CacheConnection.WriteToCache(Req,str(result))

    logging.info("returning")
    return str(result)

def main(req: func.HttpRequest) -> func.HttpResponse:
    #Cors Header
    Header = {"Access-Control-Allow-Origin": "*"}
    logging.info('Python HTTP trigger function processed a request.')

    name = req.params.get('name')
    period =req.params.get('period')

    Req = BuildRequestString(name,period)

    logging.info("request: " + str(Req))

    data = CacheConnection.GetFromCache(Req,1)

    #return from cache
    if data is not None:
        logging.info("Returning From Cache")
        return func.HttpResponse(str(data),headers=Header)
    
    # else make the api call
    result =GetGrowth(str(name),period)
    
    #build Json Objs
    data = {}
    data['Growth'] = str(result)
    result = json.dumps(data,skipkeys=False)

    logging.info("Writing to Cache")
    CacheConnection.WriteToCache(Req,str(result))

    logging.info("returning")
    return func.HttpResponse(str(result),headers=Header)