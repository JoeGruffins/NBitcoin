using System;

namespace NBitcoin.RPC
{
    public class DecredRPCClient : RPCClient
    {
		public DecredRPCClient(string authenticationString, Uri address, Network network = null) : base(authenticationString, address, network)
		{
		}
	}
}