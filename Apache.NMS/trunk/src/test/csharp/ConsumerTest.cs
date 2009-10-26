/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Threading;
using Apache.NMS;
using Apache.NMS.Util;
using NUnit.Framework;
using NUnit.Framework.Extensions;

namespace Apache.NMS.Test
{
	[TestFixture]
	public class ConsumerTest : NMSTestSupport
	{
		protected static string TEST_CLIENT_ID = "TestConsumerClientId";

// The .NET CF does not have the ability to interrupt threads, so this test is impossible.
#if !NETCF
		[RowTest]
		[Row(AcknowledgementMode.AutoAcknowledge)]
		[Row(AcknowledgementMode.ClientAcknowledge)]
		[Row(AcknowledgementMode.DupsOkAcknowledge)]
		[Row(AcknowledgementMode.Transactional)]
		public void TestNoTimeoutConsumer(AcknowledgementMode ackMode)
		{
			// Launch a thread to perform IMessageConsumer.Receive().
			// If it doesn't fail in less than three seconds, no exception was thrown.
			Thread receiveThread = new Thread(new ThreadStart(TimeoutConsumerThreadProc));
			using(IConnection connection = CreateConnection(TEST_CLIENT_ID))
			{
				connection.Start();
				using(ISession session = connection.CreateSession(ackMode))
				{
					ITemporaryQueue queue = session.CreateTemporaryQueue();
					using(this.timeoutConsumer = session.CreateConsumer(queue))
					{
						receiveThread.Start();
						if(receiveThread.Join(3000))
						{
							Assert.Fail("IMessageConsumer.Receive() returned without blocking.  Test failed.");
						}
						else
						{
							// Kill the thread - otherwise it'll sit in Receive() until a message arrives.
							receiveThread.Interrupt();
						}
					}
				}
			}
		}

		protected IMessageConsumer timeoutConsumer;
		
		protected void TimeoutConsumerThreadProc()
		{
			try
			{
				timeoutConsumer.Receive();
			}
			catch(ArgumentOutOfRangeException e)
			{
				// The test failed.  We will know because the timeout will expire inside TestNoTimeoutConsumer().
				Assert.Fail("Test failed with exception: " + e.Message);
			}
			catch(ThreadInterruptedException)
			{
				// The test succeeded!  We were still blocked when we were interrupted.
			}
			catch(Exception e)
			{
				// Some other exception occurred.
				Assert.Fail("Test failed with exception: " + e.Message);
			}
		}

		[RowTest]
		[Row(AcknowledgementMode.AutoAcknowledge)]
		[Row(AcknowledgementMode.ClientAcknowledge)]
		[Row(AcknowledgementMode.DupsOkAcknowledge)]
		[Row(AcknowledgementMode.Transactional)]
		public void TestSyncReceiveConsumerClose(AcknowledgementMode ackMode)
		{
			// Launch a thread to perform IMessageConsumer.Receive().
			// If it doesn't fail in less than three seconds, no exception was thrown.
			Thread receiveThread = new Thread(new ThreadStart(TimeoutConsumerThreadProc));
			using (IConnection connection = CreateConnection(TEST_CLIENT_ID))
			{
				connection.Start();
				using (ISession session = connection.CreateSession(ackMode))
				{
					ITemporaryQueue queue = session.CreateTemporaryQueue();
					using (this.timeoutConsumer = session.CreateConsumer(queue))
					{
						receiveThread.Start();
						if (receiveThread.Join(3000))
						{
							Assert.Fail("IMessageConsumer.Receive() returned without blocking.  Test failed.");
						}
						else
						{
							// Kill the thread - otherwise it'll sit in Receive() until a message arrives.
							this.timeoutConsumer.Close();
							receiveThread.Join(10000);
							if (receiveThread.IsAlive)
							{
								// Kill the thread - otherwise it'll sit in Receive() until a message arrives.
								receiveThread.Interrupt();
								Assert.Fail("IMessageConsumer.Receive() thread is still alive, close should have killed it.");
							}
						}
					}
				}
			}
		}

        internal class ThreadArg
        {
            internal IConnection connection = null;
            internal ISession session = null;
            internal IDestination destination = null;
        }

        protected void DelayedProducerThreadProc(Object arg)
        {
            try
            {
                ThreadArg args = arg as ThreadArg;

                using(ISession session = args.connection.CreateSession())
                {
                    using(IMessageProducer producer = session.CreateProducer(args.destination))
                    {
                        // Give the consumer time to enter the receive.
                        Thread.Sleep(5000);
        
                        producer.Send(args.session.CreateTextMessage("Hello World"));
                    }
                }
            }
            catch(Exception e)
            {
                // Some other exception occurred.
                Assert.Fail("Test failed with exception: " + e.Message);
            }
        }
        
        [RowTest]
        [Row(AcknowledgementMode.AutoAcknowledge)]
        [Row(AcknowledgementMode.ClientAcknowledge)]
        [Row(AcknowledgementMode.DupsOkAcknowledge)]
        [Row(AcknowledgementMode.Transactional)]
        public void TestConsumerReceiveBeforeMessageDispatched(AcknowledgementMode ackMode)
        {
            // Launch a thread to perform a delayed send.
            Thread sendThread = new Thread(DelayedProducerThreadProc);
            using(IConnection connection = CreateConnection(TEST_CLIENT_ID))
            {
                connection.Start();
                using(ISession session = connection.CreateSession(ackMode))
                {
                    ITemporaryQueue queue = session.CreateTemporaryQueue();
                    using(IMessageConsumer consumer = session.CreateConsumer(queue))
                    {
                        ThreadArg arg = new ThreadArg();

                        arg.connection = connection;
                        arg.session = session;
                        arg.destination = queue;
                        
                        sendThread.Start(arg);
                        IMessage message = consumer.Receive(TimeSpan.FromMinutes(1));
                        Assert.IsNotNull(message);
                    }
                }
            }
        }

        [RowTest]
        [Row(MsgDeliveryMode.NonPersistent, DestinationType.Queue)]        
        [Row(MsgDeliveryMode.NonPersistent, DestinationType.Topic)]             
        public void TestDontStart(MsgDeliveryMode deliveryMode, DestinationType destinationType )
        {
            using(IConnection connection = CreateConnection(TEST_CLIENT_ID))
            {
                ISession session = connection.CreateSession();
                IDestination destination = CreateDestination(session, destinationType);
                IMessageConsumer consumer = session.CreateConsumer(destination);
        
                // Send the messages
                SendMessages(session, destination, deliveryMode, 1);
        
                // Make sure no messages were delivered.
                Assert.IsNull(consumer.Receive(TimeSpan.FromMilliseconds(1000)));
            }
        }
        
#endif

	}
}
