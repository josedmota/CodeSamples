import logging
import azure.functions as func
import os
import json
import sys
#sys.path.append(os.path.abspath(""))

from __app__.SharedCode import cacheserver as CacheConnection

#to use the new IEX cloud change this value to 'iexcloud-v1' (is also the default)
#os.environ['IEX_API_VERSION']='iexcloud-sandbox'
#os.environ['IEX_TOKEN']='Tsk_df0426c99cc64421ac28ac2745944a03'

import iexfinance.stocks as Stock


def GetPrice(name):
    if type(name) is str:
        stock = Stock.Stock(name)
        return stock.get_price()
    

def BuildRequestString(name):
        return "/GetValue/{0}".format(name)


def main(req: func.HttpRequest) -> func.HttpResponse:
    #Cors Header
    Header = {"Access-Control-Allow-Origin": "*"}
    logging.info('Python HTTP trigger function processed a request.')

    name = req.params.get('name')
    
    Req = BuildRequestString(name)

    logging.info("request: " + str(Req))

    data = CacheConnection.GetFromCache(Req,1)

    #return from cache
    if data is not None:
        logging.info("Returning From Cache")
        return func.HttpResponse(str(data),headers=Header)
    
    # else make the api call
    result =GetPrice(str(name))
    
    #build Json Objs
    data = {}
    data['SharePrice'] = str(result)
    result = json.dumps(data,skipkeys=False)

    logging.info("Writing to Cache")
    CacheConnection.WriteToCache(Req,str(result))

    logging.info("returning")
    return func.HttpResponse(str(result),headers=Header)