/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//
//  NOTE!: This file is autogenerated - do not modify!
//         if you need to make a change, please see the Groovy scripts in the
//         activemq-core module
//

using System;
using System.Collections;

using Apache.NMS.ActiveMQ.OpenWire;
using Apache.NMS.ActiveMQ.Commands;

namespace Apache.NMS.ActiveMQ.Commands
{
    /// <summary>
    ///  The ActiveMQ MessageDispatch Command
    /// </summary>
    public class MessageDispatch : BaseCommand
    {
        public const byte ID_MessageDispatch = 21;
    			
        ConsumerId consumerId;
        ActiveMQDestination destination;
        Message message;
        int redeliveryCounter;

		public override string ToString() {
            return GetType().Name + "["
                + " ConsumerId=" + ConsumerId
                + " Destination=" + Destination
                + " Message=" + Message
                + " RedeliveryCounter=" + RedeliveryCounter
                + " ]";

		}

        public override byte GetDataStructureType() {
            return ID_MessageDispatch;
        }


        // Properties

        public ConsumerId ConsumerId
        {
            get { return consumerId; }
            set { this.consumerId = value; }            
        }

        public ActiveMQDestination Destination
        {
            get { return destination; }
            set { this.destination = value; }            
        }

        public Message Message
        {
            get { return message; }
            set { this.message = value; }            
        }

        public int RedeliveryCounter
        {
            get { return redeliveryCounter; }
            set { this.redeliveryCounter = value; }            
        }

    }
}
