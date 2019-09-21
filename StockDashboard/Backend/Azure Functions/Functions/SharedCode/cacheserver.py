import pyodbc
from datetime import datetime, timedelta
import os

server = os.environ['sqlserver']#'iexcache.database.windows.net'
database = os.environ['cacheDBName']#'iexcachedb'
username = os.environ['cacheUsername']#'iexdb'
password = os.environ['cachePassword']#'Ronaldinho_94'
driver= '{ODBC Driver 17 for SQL Server}'
    
#Check if exists    
def GetFromCache(Request,dateLimit):
    if(CheckExists(Request) is False):
        return None
    
    if(CheckExpired(Request,dateLimit)):
        return None
    
    #If it exists and is not expired send cached response
    cnxn = pyodbc.connect('DRIVER='+driver+';SERVER='+server+';PORT=1433;DATABASE='+database+';UID='+username+';PWD='+ password)
    cursor=cnxn.cursor()
    requestString = "SELECT Response FROM Cached WHERE Request='{0}'".format(Request)
    cursor.execute(requestString)
    row = cursor.fetchone()
    cnxn.close()
    return str(row[0])

#Check if records are older than the time limit for this request
def CheckExpired(Request,dateLimit):
    #Get Current Date
    cnxn = pyodbc.connect('DRIVER='+driver+';SERVER='+server+';PORT=1433;DATABASE='+database+';UID='+username+';PWD='+ password)
    cursor=cnxn.cursor()
    requestString = "SELECT Regdate FROM Cached WHERE Request='{0}'".format(Request)
    cursor.execute(requestString)
    row=cursor.fetchone()
    cnxn.close()
    #Convert from string to datetime object
    savedDate= datetime.strptime(str(row[0]),'%Y-%m-%d')
    delta = datetime.now() - savedDate
    
    if(delta.days>=dateLimit):
        return True
    else:
        return False
    
#Write new request result
def WriteToCache(Request,Result):
    #Build New DateTime String
    dt = datetime.now().strftime("%Y/%m/%d")
    
    if(CheckExists(Request)):
        #Replace
        cnxn = pyodbc.connect('DRIVER='+driver+';SERVER='+server+';PORT=1433;DATABASE='+database+';UID='+username+';PWD='+ password)
        requestString = "UPDATE Cached SET Regdate='{0}',Response='{1}' WHERE Request='{2}'".format(dt,Result,Request)
        cursor = cnxn.cursor()
        cursor.execute(requestString)
        cnxn.commit()
        cnxn.close()
    else:
        #Write New
        cnxn = pyodbc.connect('DRIVER='+driver+';SERVER='+server+';PORT=1433;DATABASE='+database+';UID='+username+';PWD='+ password)
        requestString = "INSERT INTO Cached VALUES ('{0}','{1}','{2}')".format(Request,dt,Result)
        cursor = cnxn.cursor()
        cursor.execute(requestString)
        cnxn.commit()
        cnxn.close()
    
#Check if there are records of this request in cache
def CheckExists(Request):
    cnxn = pyodbc.connect('DRIVER='+driver+';SERVER='+server+';PORT=1433;DATABASE='+database+';UID='+username+';PWD='+ password)
    requestString = "SELECT COUNT(1) FROM Cached WHERE Request='{0}'".format(Request)
    
    #make request
    cursor=cnxn.cursor()
    cursor.execute(requestString)
    
    #check if it exists
    row = cursor.fetchone()
    cnxn.close()
    if(int(row[0])==0):
        return False
    else:
        return True
    