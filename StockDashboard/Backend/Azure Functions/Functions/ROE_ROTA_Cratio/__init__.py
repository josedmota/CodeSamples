import logging
import azure.functions as func
import os
import json
import sys

from __app__.SharedCode import cacheserver as CacheConnection

import iexfinance.stocks as Stock
import decimal

def CalcReturnOnEquity(data):
    ROE= float(data[0]['netIncome'])/float(data[0]['shareholderEquity'])
    return round(ROE*100,2)

def CalcReturnOnTotalAssets(data):
    EBIT = float(data[0]['grossProfit'])-float(data[0]['operatingExpense'])
    ROTA = EBIT / float(data[0]['totalAssets'])
    return round(ROTA*100,2)

#this is for long term ability to pay debts
def CalcDebtToEquityRatio(data):
    output = float(data[0]['totalLiabilities'])/float(data[0]['shareholderEquity'])
    return round(output*100,2)

#this is for short term ability to pay debts
def CalcCurrentRatio(data):
    output= float(data[0]['currentDebt'])/float(data[0]['currentAssets'])
    return round(output*100,2)

def GetData(name):
        stock=Stock.Stock(str(name))
        return stock.get_financials()

def BuildRequestString(name):
        return "/ROE_ROTA_Cratio/{0}".format(name)


def main(req: func.HttpRequest) -> func.HttpResponse:
    #Cors Header
    Header = {"Access-Control-Allow-Origin": "*"}
    logging.info('Python HTTP trigger function processed a request.')

    name = req.params.get('name')

    Req = BuildRequestString(name)

    logging.info("request: " + str(Req))

    data = CacheConnection.GetFromCache(Req,45)

    #return from cache
    if data is not None:
        logging.info("Returning From Cache")
        return func.HttpResponse(str(data),headers=Header)
    
    
    # else make the api call
    data = GetData(str(name))

    ROE = CalcReturnOnEquity(data)
    ROTA = CalcReturnOnTotalAssets(data)
    DEratio = CalcDebtToEquityRatio(data)
    Cratio = CalcCurrentRatio(data)
    
    #build Json Objs
    data = {}
    data['ROE'] = str(ROE)+"%"
    data['ROTA'] = str(ROTA)+"%"
    data['DEratio'] = str(DEratio)+"%"
    data['Cratio'] = str(Cratio) + "%"

    result = json.dumps(data,skipkeys=False)

    logging.info("Writing to Cache")
    CacheConnection.WriteToCache(Req,str(result))

    logging.info("returning")
    return func.HttpResponse(str(result),headers=Header)