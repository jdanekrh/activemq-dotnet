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

using ActiveMQ.OpenWire;
using ActiveMQ.Commands;

namespace ActiveMQ.Commands
{
    /// <summary>
    ///  The ActiveMQ ConsumerInfo Command
    /// </summary>
    public class ConsumerInfo : BaseCommand
    {
        public const byte ID_ConsumerInfo = 5;
    			
        ConsumerId consumerId;
        bool browser;
        ActiveMQDestination destination;
        int prefetchSize;
        int maximumPendingMessageLimit;
        bool dispatchAsync;
        string selector;
        string subscriptionName;
        bool noLocal;
        bool exclusive;
        bool retroactive;
        byte priority;
        BrokerId[] brokerPath;
        BooleanExpression additionalPredicate;
        bool networkSubscription;
        bool optimizedAcknowledge;
        bool noRangeAcks;

		public override string ToString() {
            return GetType().Name + "["
                + " ConsumerId=" + ConsumerId
                + " Browser=" + Browser
                + " Destination=" + Destination
                + " PrefetchSize=" + PrefetchSize
                + " MaximumPendingMessageLimit=" + MaximumPendingMessageLimit
                + " DispatchAsync=" + DispatchAsync
                + " Selector=" + Selector
                + " SubscriptionName=" + SubscriptionName
                + " NoLocal=" + NoLocal
                + " Exclusive=" + Exclusive
                + " Retroactive=" + Retroactive
                + " Priority=" + Priority
                + " BrokerPath=" + BrokerPath
                + " AdditionalPredicate=" + AdditionalPredicate
                + " NetworkSubscription=" + NetworkSubscription
                + " OptimizedAcknowledge=" + OptimizedAcknowledge
                + " NoRangeAcks=" + NoRangeAcks
                + " ]";

		}

        public override byte GetDataStructureType() {
            return ID_ConsumerInfo;
        }


        // Properties

        public ConsumerId ConsumerId
        {
            get { return consumerId; }
            set { this.consumerId = value; }            
        }

        public bool Browser
        {
            get { return browser; }
            set { this.browser = value; }            
        }

        public ActiveMQDestination Destination
        {
            get { return destination; }
            set { this.destination = value; }            
        }

        public int PrefetchSize
        {
            get { return prefetchSize; }
            set { this.prefetchSize = value; }            
        }

        public int MaximumPendingMessageLimit
        {
            get { return maximumPendingMessageLimit; }
            set { this.maximumPendingMessageLimit = value; }            
        }

        public bool DispatchAsync
        {
            get { return dispatchAsync; }
            set { this.dispatchAsync = value; }            
        }

        public string Selector
        {
            get { return selector; }
            set { this.selector = value; }            
        }

        public string SubscriptionName
        {
            get { return subscriptionName; }
            set { this.subscriptionName = value; }            
        }

        public bool NoLocal
        {
            get { return noLocal; }
            set { this.noLocal = value; }            
        }

        public bool Exclusive
        {
            get { return exclusive; }
            set { this.exclusive = value; }            
        }

        public bool Retroactive
        {
            get { return retroactive; }
            set { this.retroactive = value; }            
        }

        public byte Priority
        {
            get { return priority; }
            set { this.priority = value; }            
        }

        public BrokerId[] BrokerPath
        {
            get { return brokerPath; }
            set { this.brokerPath = value; }            
        }

        public BooleanExpression AdditionalPredicate
        {
            get { return additionalPredicate; }
            set { this.additionalPredicate = value; }            
        }

        public bool NetworkSubscription
        {
            get { return networkSubscription; }
            set { this.networkSubscription = value; }            
        }

        public bool OptimizedAcknowledge
        {
            get { return optimizedAcknowledge; }
            set { this.optimizedAcknowledge = value; }            
        }

        public bool NoRangeAcks
        {
            get { return noRangeAcks; }
            set { this.noRangeAcks = value; }            
        }

    }
}
