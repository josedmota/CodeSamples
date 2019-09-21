#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun Aug 18 09:01:37 2019

@author: diogo
"""


import os
#to use the new IEX cloud change this value to 'iexcloud-v1' (is also the default)
os.environ['IEX_API_VERSION']='iexcloud-sandbox'
os.environ['IEX_TOKEN']='Tsk_df0426c99cc64421ac28ac2745944a03'

from datetime import datetime
import iexfinance.stocks as Stock
from iexfinance.stocks import get_historical_data
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
    
def CalcCurrentRatio(data):
    output= float(data[0]['currentDebt'])/float(data[0]['currentAssets'])
    return round(output*100,2)

stock = Stock.Stock("MCD")

d=stock.get_financials()

a=CalcReturnOnEquity(d)

b= CalcReturnOnTotalAssets(d)

c=CalcDebtToEquityRatio(d)

aa= CalcCurrentRatio(d)

