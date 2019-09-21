import logging
import azure.functions as func
import os
import json
import sys

from __app__.SharedCode import cacheserver as CacheConnection
import __app__.GetGrowth as GrowthObject

import iexfinance.stocks as Stock
import decimal


def main(req: func.HttpRequest) -> func.HttpResponse:
    #Cors Header
    Header = {"Access-Control-Allow-Origin": "*"}
    logging.info('Python HTTP trigger function processed a request.')

    name = req.params.get('name')
    result= GrowthObject.InnerCall(str(name),2)

    logging.info("returning")
    return func.HttpResponse(str(result),headers=Header)