//
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreements.  See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the License.  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
using System;
using System.IO;
using Apache.NMS.MQTT.Transport;
using Apache.NMS.MQTT.Protocol;

namespace Apache.NMS.MQTT.Commands
{
	public class UNSUBSCRIBE : BaseCommand
	{
		public const byte TYPE = 10;
		public const byte DEFAULT_HEADER = 0xA2;

		public UNSUBSCRIBE() : base(new Header(DEFAULT_HEADER))
		{
		}

		public UNSUBSCRIBE(Header header) : base(header)
		{
		}

		public override int CommandType
		{
			get { return TYPE; }
		}

		public override bool IsUNSUBSCRIBE
		{
			get { return true; }
		}

		private string[] topics;
		public string[] Topics
		{
			get { return this.topics; }
			set { this.topics = value; }
		}
	}
}

