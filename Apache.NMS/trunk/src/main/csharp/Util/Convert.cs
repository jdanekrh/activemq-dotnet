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
using System.Reflection;

namespace Apache.NMS.Util
{
	public class NMSConvert
	{
		/// <summary>
		/// Convert the acknowledgment mode string into AcknowledgementMode enum.
		/// </summary>
		/// <param name="ackText"></param>
		/// <returns>Equivalent enum value.  If unknown string is encounted, it will default to AutoAcknowledge.</returns>
		public static AcknowledgementMode ToAcknowledgementMode(string ackText)
		{
			if(String.Compare(ackText, "AutoAcknowledge", true) == 0)
			{
				return AcknowledgementMode.AutoAcknowledge;
			}
			else if(String.Compare(ackText, "ClientAcknowledge", true) == 0)
			{
				return AcknowledgementMode.ClientAcknowledge;
			}
			else if(String.Compare(ackText, "DupsOkAcknowledge", true) == 0)
			{
				return AcknowledgementMode.DupsOkAcknowledge;
			}
			else if(String.Compare(ackText, "Transactional", true) == 0)
			{
				return AcknowledgementMode.Transactional;
			}
			else
			{
				return AcknowledgementMode.AutoAcknowledge;
			}
		}

		/// <summary>
		/// Convert an object into a text message.  The object must be serializable to XML.
		/// </summary>
		public static ITextMessage ToXmlMessage(IMessageProducer producer, object obj)
		{
			ITextMessage message = producer.CreateTextMessage(XmlUtil.Serialize(obj));

			// Embed the type into the message
			message.NMSType = obj.GetType().FullName;
			return message;
		}

		/// <summary>
		/// Convert an object into a text message.  The object must be serializable to XML.
		/// </summary>
		public static ITextMessage ToXmlMessage(ISession session, object obj)
		{
			ITextMessage message = session.CreateTextMessage(XmlUtil.Serialize(obj));

			// Embed the type into the message
			message.NMSType = obj.GetType().FullName;
			return message;
		}

		/// <summary>
		/// Convert a text message into an object.  The object must be serializable from XML.
		/// </summary>
		public static object FromXmlMessage(IMessage message)
		{
			ITextMessage textMessage = message as ITextMessage;

			if(null == textMessage)
			{
				return null;
			}

			Type objType = GetRuntimeType(textMessage.NMSType);
			if(null == objType)
			{
				Tracer.ErrorFormat("Could not load type for {0} while deserializing XML object.", textMessage.NMSType);
				return null;
			}

			return XmlUtil.Deserialize(objType, textMessage.Text);
		}

		/// <summary>
		/// Get the runtime type for the class name.  This routine will search all loaded
		/// assemblies in the current App Domain to find the type.
		/// </summary>
		/// <param name="typeName">Full name of the type.</param>
		/// <returns>Type object if found, or null if not found.</returns>
		private static Type GetRuntimeType(string typeName)
		{
			Type objType = null;

			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				objType = assembly.GetType(typeName, false, true);
				if(null != objType)
				{
					break;
				}
			}

			return objType;
		}
	}
}
