#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sat Aug 17 11:10:34 2019

@author: diogo
"""
import os
#to use the new IEX cloud change this value to 'iexcloud-v1' (is also the default)
os.environ['IEX_API_VERSION']='iexcloud-sandbox'
os.environ['IEX_TOKEN']='Tsk_df0426c99cc64421ac28ac2745944a03'

from datetime import datetime
import iexfinance.stocks as Stock

def GetPrice(name):
    if type(name) is str:
        stock = Stock.Stock(name)
        return stock.get_price()

a=GetPrice("MCD")