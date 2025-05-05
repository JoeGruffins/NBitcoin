using NBitcoin.Altcoins.Elements;
using NBitcoin.RPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NBitcoin.Altcoins;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;
using Encoders = NBitcoin.DataEncoders.Encoders;
using System.Threading;
using NBitcoin.Logging;
using NBitcoin.Protocol;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace NBitcoin.Tests
{
	[Trait("Decred", "Decred")]
	public class DecredTests()
	{
		[Fact]
		public void DeDecred()
		{

			Network network = NBitcoin.Altcoins.Decred.Instance.Mainnet;
			Console.WriteLine((new Key().PubKey.GetAddress(ScriptPubKeyType.Legacy, network)).ToString());
		}
	}
}
