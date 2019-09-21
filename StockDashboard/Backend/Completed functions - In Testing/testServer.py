#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sun Jan 20 12:13:04 2019

@author: diogo
"""

from flask import escape, Flask, request, jsonify

from datetime import datetime
from iexfinance.stocks import get_historical_data

app = Flask(__name__)


@app.route('/', methods=["POST"])
def hello_world():
    name = request.args['name']
    start = datetime(2018,1,1)
    end = datetime(2019,1,1)

    df= get_historical_data(str(name),start,end)

    xAxis = []
    yAxis = []
    for k,v in df.items():
        xAxis.append(k)
        yAxis.append(v['close'])
    return jsonify(dict(zip(xAxis,yAxis)))
        
    