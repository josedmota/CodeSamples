#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun Jul 21 08:26:18 2019

@author: diogo
"""

import os
#to use the new IEX cloud change this value to 'iexcloud-v1' (is also the default)
os.environ['IEX_API_VERSION']='iexcloud-sandbox'
os.environ['IEX_TOKEN']='Tsk_df0426c99cc64421ac28ac2745944a03'

from datetime import datetime
import iexfinance.stocks as Stock

#update july 2019 Moody's
USAAA20yrCorporateBondYield = 3.31


#calculate revenue growth last 3y
def AvgRevenueGrowth(Data):
    FirstYear = Data[3]['totalRevenue']
    LastYear = Data[0]['totalRevenue']
    if(FirstYear != None and LastYear != None and FirstYear != 0):
        rate = (LastYear/FirstYear)/3
        if(FirstYear>LastYear):
            return rate*-1
        else:
            return rate
    
#calculate Net Income growth last 3y
def AvgNetIncomeGrowth(Data):
    FirstYear = Data[3]['netIncome']
    LastYear = Data[0]['netIncome']
    if(FirstYear != None and LastYear != None and FirstYear != 0):
        rate = (LastYear/FirstYear)/3
        if(FirstYear>LastYear):
            return rate*-1
        else:
            return rate

#Get latest earnings value
Company = Stock.Stock("MCD")
eps = Company.get_earnings(last=1)

#inform on graham growth rate correction factors
YoYGrowth = eps[0]['actualEPS']/eps[0]['yearAgo']
SurprisePercent = eps[0]['EPSSurpriseDollar']/eps[0]['consensusEPS']


flow=Company.get_income_statement(period='annual',last=4)
RevGrowth = AvgRevenueGrowth(flow)
IncGrowth = AvgNetIncomeGrowth(flow)

#To account for companies that are becoming
#more efficient while not increasing Revenue
GrowthRate = min(RevGrowth,IncGrowth) 

#Corrections based on EPS GrowthYoY
#This is a judgement call and likely will need adjustment
#Will only bring Growth Rate down, never up to keep a reasonable margin of safety

if(SurprisePercent>0):
    GrowthRate = GrowthRate + SurprisePercent*0.5

Result = (eps[0]['actualEPS'] * (8.5+2.0*100.0*GrowthRate)*4.4)/USAAA20yrCorporateBondYield 
