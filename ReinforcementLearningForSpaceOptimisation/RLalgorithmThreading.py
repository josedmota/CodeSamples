# -*- coding: utf-8 -*-
"""
Created on Fri Aug 17 09:10:25 2018

@author: JPERE
"""

import numpy as np
import time
import os

import random


from collections      import deque
from keras.models     import Sequential
from keras.layers     import Dense
from keras.optimizers import Adam
import keras
import tensorflow as tf


class Agent:
    def __init__(self, state_size):
        self.state_size         = state_size
        self.actions            = self._buildActionList()
        self.action_size        = self.actions.shape[0]
        self.memory             = deque(maxlen=2000)
        self.learning_rate      = 0.001
        self.gamma              = 0.95
        self.exploration_rate   = 1.0
        self.exploration_min    = 0.01
        self.exploration_decay  = 0.995
        self.brain              = self._build_model()
        self.prevActions   = [] # list size  = length of possible actions array// used list because is more efficient to rewrite
        
    def _buildActionList(self):
       #create action List
       #change 100 to 1000 to increase precision
       #change 0,180,90 to add more rotation freedom
       for  u in range(0,100,5):
           for v in range(0,100,5):
               for rot in range(0,90,90):
                   if (u==0 and v==0 and rot ==0):
                       actions = np.array([[u/100,v/100,rot]])
                   else:
                       actions = np.append(actions,[[u/100,v/100,rot]],axis=0)
                       
       return actions               
            
    def _build_model(self):
        
        #config = tf.ConfigProto(device_count = {'CPU': 8})
        #sess = tf.Session(config=config)
        #keras.backend.set_session(sess)
        
        # Neural Net for Deep-Q learning Model
        model = Sequential()
        model.add(Dense(24, input_dim=self.state_size, activation='relu'))
        model.add(Dense(100, activation='relu'))
        model.add(Dense(500,activation ='sigmoid'))
        model.add(Dense(self.action_size, activation='linear'))
        model.compile(loss='mse', optimizer=Adam(lr=self.learning_rate))

        #if os.path.isfile(self.weight_backup):
         #   model.load_weights(self.weight_backup)
           # self.exploration_rate = self.exploration_min
        return model

    def remember(self, state, action, reward, next_state, done):
        self.memory.append((state, action, reward, next_state, done))

    def act(self, state):
        if np.random.rand() <= self.exploration_rate:
            actionId= random.randrange(self.action_size)
            #write to file to be read by GH
            while not os.path.isfile("TestSpot.txt"): #write only when previous TestSpot is deleted
                with open("TestSpot.txt", "w+") as text_file:
                    print("{0},{1},{2}".format(self.actions[actionId][0],self.actions[actionId][1],self.actions[actionId][2]), file=text_file)
            time.sleep(0.5) #may not be needed
            return actionId #random which action to take, not directly the xy coordinates + Rz
        
        act_values = self.brain.predict(state)
        actionId = np.argmax(act_values) #act_values[0]?
        
        while actionId in self.prevActions: #if action exec in the past: correct reward to 0 and choose the next highest pred. reward action
            act_values[actionId]=0;
            actionId = np.argmax(act_values);
            #write to file to be read by GH
            
        while not os.path.isfile("TestSpot.txt"): #write only when previous TestSpot is deleted
            with open("TestSpot.txt", "w+") as text_file:
                print("{0},{1},{2}".format(self.actions[actionId][0],self.actions[actionId][1],self.actions[actionId][2]), file=text_file)
        time.sleep(0.5) #may not be needed
        
        return actionId
    def clear_prevActions(self):
        self.prevActions = []
        
        
    def replay(self, sample_batch_size):
        if len(self.memory) < sample_batch_size:
            return
        sample_batch = random.sample(self.memory, sample_batch_size)
        for state, action, reward, next_state, done in sample_batch:
            target = reward
            
            if not done:
              target = reward + self.gamma * np.amax(self.brain.predict(next_state))#[0]?
            target_f = self.brain.predict(state)##############################################
            target_f[0][action] = target
            self.brain.fit(state, target_f, epochs=1, verbose=0)
            
        if self.exploration_rate > self.exploration_min:
            self.exploration_rate *= self.exploration_decay
        
        
class Play:
    def __init__(self,state_size):
        self.sample_batch_size = 400
        self.episodes          = 4000
        self.state_size        = state_size
        self.agent             = Agent(self.state_size)      
        
    def run(self):
            global CanGoNextStep

            for index_episode in range(self.episodes):
                print("Start New Episode")
                fullyRead=False
                while not fullyRead:
                    try:
                        state = np.loadtxt("Startstate.txt")
                        state = np.reshape(state,[1,81]) 
                        fullyRead=True
                        print("Read Startstate")
                    except:
                        None
                
                #try:
                #    os.remove("state.txt")
                #except:
                #    None

                #state = np.reshape(state, [1, self.state_size])#probably not needed in this case?
                
                done = False
                score = 0
                
                CanGoNextStep = False
                AgentLearned = True
                ActionSent = False
                
                while not done:
                    
                    if not ActionSent and AgentLearned: #send action to file
                        action = self.agent.act(state)
                        print("Execute Action: " + str(action))
                        #Write to files
                        ActionSent = True
                        AgentLearned = False
                        #print("Action Sent")
                        continue
                    try:
                        contents=np.loadtxt("broadcast.txt")
                        if contents.size>0: #make sure contents are captured needed because GH is slow!
                            reward = int(contents[0])
                            done = bool(contents[1]) #convert str to int 0 or 1 to False or True
                            CanGoNextStep = True
                            os.remove("broadcast.txt")
                            print("Deleted BroadCast")
                            
                    except:
                           None 
                    
                    if CanGoNextStep: #when the file is read and GH has results run this
                        #Get next_state reward, done after executing the action___ GH trigger needed here
                        stateNotAvailable = True
                        
                        while stateNotAvailable:
                            try:
                                next_state = np.loadtxt("newState.txt")
                                next_state = np.reshape(next_state,[1,81])
                                #delete state.txt ??????????????????????????????????
                                stateNotAvailable = False
                                os.remove("newState.txt")
                                print("readNewState")
                            except:
                                None
                                
                        
                        self.agent.remember(state, action, reward, next_state, done)
                        #next_state = np.reshape(next_state, [1, self.state_size])  ##check if needed
                        state = next_state
                        score += reward
                        
                        self.agent.replay(self.sample_batch_size)
                        
                        CanGoNextStep = False
                        
                        AgentLearned = True
                        ActionSent = False
                        print("Completed")
                                            
                    
                    
                    #next_state, reward, done, _ = self.env.step(action)
                self.agent.clear_prevActions()
                print("Episode {}# Score: {}".format(index_episode, score))
                
               



#if __name__ == "__main__":
    #cartpole = CartPole()
    #cartpole.run()        
        
def start(st_size):

        global running
        running = True
        simulation = Play(st_size) 
        print("running")
        #running = True
        simulation.run()

        
    
def SeeFileEvents():
    #wait for start trigger
    #global running
    #while not running:
        #time.sleep(5)
        #try:
           #ab=np.loadtxt("start.txt")
            #if(ab>0):
                #start(int(ab))
                #running = True
        #except:
            #print("nop")
    
    while running:
        try:
            contents=np.loadtxt("broadcast.txt")
            global reward  
            global done
            global CanGoNextStep
            reward = int(contents[0])
            done = bool(contents[1]) #convert str to int 0 or 1 to False or True
            CanGoNextStep = True
            print("Learning...")
            os.remove("broadcast.txt")
        except:
            print("")        


CanGoNextStep = True
simulation = None
player = None #initialize here with null just to have global vars

reward = 0
done = False

running = False      
start(81)
#try:
    #Thread(target=SeeFileEvents).start()
#except:
   # print("ups")

#_thread.start_new_thread(SeeFileEvents,[0,0])

print("created thread")
#Thread(target=start(155)).start()

print("ok")


    
    


