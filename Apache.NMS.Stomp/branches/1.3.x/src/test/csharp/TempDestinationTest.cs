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
using System.Collections;
using Apache.NMS.Util;
using Apache.NMS.Test;
using NUnit.Framework;
using NUnit.Framework.Extensions;

namespace Apache.NMS.Stomp.Test
{
    [TestFixture]
    public class TempDestinationTest : NMSTestSupport
    {
        private IConnection connection;
        private IList connections = ArrayList.Synchronized(new ArrayList());

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.connection = CreateConnection();
            this.connections.Add(connection);
        }

        [TearDown]
        public override void TearDown()
        {
            foreach(IConnection conn in this.connections)
            {
                try
                {
                    conn.Close();
                }
                catch
                {
                }
            }

            connections.Clear();

            base.TearDown();
        }

        [Test]
        public void TestTempDestOnlyConsumedByLocalConn()
        {
            connection.Start();

            ISession tempSession = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            ITemporaryQueue queue = tempSession.CreateTemporaryQueue();
            IMessageProducer producer = tempSession.CreateProducer(queue);
            producer.DeliveryMode = (MsgDeliveryMode.NonPersistent);
            ITextMessage message = tempSession.CreateTextMessage("First");
            producer.Send(message);

            // temp destination should not be consume when using another connection
            IConnection otherConnection = CreateConnection();
            connections.Add(otherConnection);
            ISession otherSession = otherConnection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            ITemporaryQueue otherQueue = otherSession.CreateTemporaryQueue();
            IMessageConsumer consumer = otherSession.CreateConsumer(otherQueue);
            IMessage msg = consumer.Receive(TimeSpan.FromMilliseconds(3000));
            Assert.IsNull(msg);

            // should be able to consume temp destination from the same connection
            consumer = tempSession.CreateConsumer(queue);
            msg = consumer.Receive(TimeSpan.FromMilliseconds(3000));
            Assert.IsNotNull(msg);
        }

        [Test]
        public void TestTempQueueHoldsMessagesWithConsumers()
        {
            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            ITemporaryQueue queue = session.CreateTemporaryQueue();
            IMessageConsumer consumer = session.CreateConsumer(queue);
            connection.Start();

            IMessageProducer producer = session.CreateProducer(queue);
            producer.DeliveryMode = (MsgDeliveryMode.NonPersistent);
            ITextMessage message = session.CreateTextMessage("Hello");
            producer.Send(message);

            IMessage message2 = consumer.Receive(TimeSpan.FromMilliseconds(1000));
            Assert.IsNotNull(message2);
            Assert.IsTrue(message2 is ITextMessage, "Expected message to be a ITextMessage");
            Assert.IsTrue(((ITextMessage)message2).Text == message.Text, "Expected message to be a '" + message.Text + "'");
        }

        [Test]
        public void TestTempQueueHoldsMessagesWithoutConsumers()
        {
            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            ITemporaryQueue queue = session.CreateTemporaryQueue();
            IMessageProducer producer = session.CreateProducer(queue);
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
            ITextMessage message = session.CreateTextMessage("Hello");
            producer.Send(message);

            connection.Start();
            IMessageConsumer consumer = session.CreateConsumer(queue);
            IMessage message2 = consumer.Receive(TimeSpan.FromMilliseconds(3000));
            Assert.IsNotNull(message2);
            Assert.IsTrue(message2 is ITextMessage, "Expected message to be a ITextMessage");
            Assert.IsTrue(((ITextMessage)message2).Text == message.Text, "Expected message to be a '" + message.Text + "'");
        }

        [Test]
        public void TestTmpQueueWorksUnderLoad()
        {
            int count = 250;
            int dataSize = 1024;

            ArrayList list = new ArrayList(count);
            ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);
            ITemporaryQueue queue = session.CreateTemporaryQueue();
            IMessageProducer producer = session.CreateProducer(queue);
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

            byte[] data = new byte[dataSize];
            for(int i = 0; i < count; i++)
            {
                IBytesMessage message = session.CreateBytesMessage();
                message.WriteBytes(data);
                message.Properties.SetInt("c", i);
                producer.Send(message);
                list.Add(message);
            }

            connection.Start();
            IMessageConsumer consumer = session.CreateConsumer(queue);
            for(int i = 0; i < count; i++)
            {
                IMessage message2 = consumer.Receive(TimeSpan.FromMilliseconds(2000));
                Assert.IsTrue(message2 != null);
                Assert.AreEqual(i, message2.Properties.GetInt("c"));
            }
        }
    }
}
