#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Created on Sat Aug 17 14:59:02 2019

@author: diogo
"""

import os
#to use the new IEX cloud change this value to 'iexcloud-v1' (is also the default)
os.environ['IEX_API_VERSION']='iexcloud-sandbox'
os.environ['IEX_TOKEN']='Tsk_df0426c99cc64421ac28ac2745944a03'

from datetime import datetime
import iexfinance.stocks as Stock
from iexfinance.stocks import get_historical_data

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
    return growth
    

end=datetime.now()
start=monthdelta(end,-1)

data = get_historical_data('MCD',start,end,output_format='pandas')
print(CalculateGrowth(data))
