#!/usr/bin/env python3
# -*- coding: utf-8 -*-

from flask import escape, Flask, request, jsonify
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
    

app = Flask(__name__)

@app.route('/', methods=["POST"])
def HTTPReceiver():
    name = request.args['name']
    
    if type(name) is str:
        result=GetPrice(name)
    
    return jsonify(str(result))
