using System;
using NBitcoin.Crypto;
using NBitcoin.Altcoins.HashX11.Crypto.SHA3;

namespace NBitcoin.Altcoins
{
	public partial class Decred : NetworkSetBase
	{
		public static Decred Instance { get; } = new Decred();
		public override string CryptoCode => "DCR";

		private Decred()
		{

		}

		private static byte[] Blake256(byte[] b, int offset, int length)
		{
			var blake = new Blake256();
			byte[] hash = blake.ComputeBytes(b).GetBytes();
			return blake.ComputeBytes(hash).GetBytes();
		}


		public class DecredConsensusFactory : ConsensusFactory
		{
			private DecredConsensusFactory()
			{
			}

			public static DecredConsensusFactory Instance { get; } = new DecredConsensusFactory();

			public override BlockHeader CreateBlockHeader()
			{
				return new DecredBlockHeader();
			}

			public override Block CreateBlock()
			{
				return new DecredBlock((DecredBlockHeader)CreateBlockHeader());
			}
		}

		public class DecredOutPoint : OutPoint
		{
			protected uint nTree = 0;

			public uint Tree
			{
				get
				{
					return nTree;
				}
				set
				{
					nTree = value;
				}
			}

			protected uint nSequence = 0;

			public uint Sequence
			{
				get
				{
					return nSequence;
				}
				set
				{
					nSequence = value;
				}
			}

		}

		public class DecredTxIn : TxIn
		{

			protected new DecredOutPoint prevout = new DecredOutPoint();

			public new DecredOutPoint PrevOut
			{
				get
				{
					return prevout;
				}
				set
				{
					prevout = value;
				}
			}

			protected ulong nValue = 0;

			public ulong Value
			{
				get
				{
					return nValue;
				}
				set
				{
					nValue = value;
				}
			}

			protected uint nHeight = 0;

			public uint Height
			{
				get
				{
					return nHeight;
				}
				set
				{
					nHeight = value;
				}
			}

			protected uint nIndex = 0;

			public uint Index
			{
				get
				{
					return nIndex;
				}
				set
				{
					nIndex = value;
				}
			}
		}

		public class DecredTxInList : UnsignedList<DecredTxIn>
		{
		}

		public class DecredTxOut : TxOut
		{

			protected uint nVersion = 0;

			public uint Version
			{
				get
				{
					return nVersion;
				}
				set
				{
					nVersion = value;
				}
			}
		}

		public class DecredTxOutList : UnsignedList<DecredTxOut>
		{
		}

		public class DecredTransaction : Transaction
			{

				protected uint nSerType = 0;

				public uint SerType
				{
					get
					{
						return nSerType;
					}
					set
					{
						nSerType = value;
					}
				}

				protected uint nExpiry = 0;

				public uint Expiry
				{
					get
					{
						return nExpiry;
					}
					set
					{
						nExpiry = value;
					}
				}

				protected new DecredTxInList vin;
				protected new DecredTxOutList vout;
				public override void ReadWrite(BitcoinStream stream)
			{
				var versionAndSerTypeB = new byte[4];
				uint txInCount = 0;
				stream.ReadWriteBytes(versionAndSerTypeB, 0, 4);
				var versionB = new byte[2];
				var serTypeB = new byte[2];
				versionB[0] = versionAndSerTypeB[0];
				versionB[1] = versionAndSerTypeB[1];
				this.Version = BitConverter.ToUInt16(versionB, 0);
				serTypeB[0] = versionAndSerTypeB[2];
				serTypeB[1] = versionAndSerTypeB[3];
				this.SerType = BitConverter.ToUInt16(serTypeB, 0);
				stream.ReadWriteAsVarInt(ref txInCount);
				vin = new DecredTxInList();
				for (int i = 0; i < txInCount; i++)
				{
					var input = new DecredTxIn();
					var prevOutIndexB = new byte[4];
					var prevOutTreeB = new byte[1];
					var prevOutSequenceB = new byte[4];
					stream.ReadWrite(input.PrevOut.Hash);
					stream.ReadWriteBytes(prevOutIndexB, 0, 4);
					stream.ReadWriteBytes(prevOutTreeB, 0, 1);
					stream.ReadWriteBytes(prevOutSequenceB, 0, 4);
					input.PrevOut.N = BitConverter.ToUInt32(prevOutIndexB, 0);
					input.PrevOut.Tree = prevOutTreeB[0];
					input.PrevOut.Sequence = BitConverter.ToUInt32(prevOutSequenceB, 0);
					this.vin.Add(input);
				}
				vout = new DecredTxOutList();
				uint txOutCount = 0;
				stream.ReadWriteAsVarInt(ref txOutCount);
				for (int i = 0; i < txOutCount; i++)
				{
					var output = new DecredTxOut();
					var txOutValueB = new byte[8];
					var txOutVersionB = new byte[2];
					//uint txOutScriptLen = 0;
					stream.ReadWriteBytes(txOutValueB, 0, 8);
					stream.ReadWriteBytes(txOutVersionB, 0, 2);
					stream.ReadWrite(output.ScriptPubKey);
					//stream.ReadWriteAsVarInt(ref txOutScriptLen);
					//var txOutScriptB = new byte[txOutScriptLen];
					//stream.ReadWriteBytes(txOutScriptB, 0, (int)txOutScriptLen);
					output.Value = BitConverter.ToUInt64(txOutValueB, 0);
					output.Version = BitConverter.ToUInt16(txOutVersionB, 0);
					//output.ScriptPubKey = txOutScriptB;
					this.vout.Add(output);
				}
				var locktimeB = new byte[4];
				var expiryB = new byte[4];
				stream.ReadWriteBytes(locktimeB, 0, 4);
				stream.ReadWriteBytes(expiryB, 0, 4);
				this.LockTime = BitConverter.ToUInt32(locktimeB, 0);
				this.Expiry = BitConverter.ToUInt32(expiryB, 0);
				uint witnessCount = 0;
				stream.ReadWriteAsVarInt(ref witnessCount);
				for (int i = 0; i < witnessCount; i++)
				{
					var witnessValueB = new byte[8];
					var witnessHeightB = new byte[4];
					var witnessIndexB = new byte[4];
					stream.ReadWriteBytes(witnessValueB, 0, 8);
					stream.ReadWriteBytes(witnessHeightB, 0, 4);
					stream.ReadWriteBytes(witnessIndexB, 0, 4);
					stream.ReadWrite(vin[i].ScriptSig);
					vin[i].Value = BitConverter.ToUInt64(witnessValueB, 0);
					vin[i].Height = BitConverter.ToUInt32(witnessHeightB, 0);
					vin[i].Index = BitConverter.ToUInt32(witnessIndexB, 0);
				}
			}
			}

#pragma warning disable CS0618 // Type or member is obsolete
		public class DecredBlockHeader : BlockHeader
		{
			public override void ReadWrite(BitcoinStream stream)
			{
				var versionB = new byte[4];
				var prevBlockHashB = new byte[32];
				var merkleRootB = new byte[32];
				var stakeRootB = new byte[32];
				var voteBitsB = new byte[2];
				var finalStateB = new byte[6];
				var votersB = new byte[2];
				var freshStakeB = new byte[1];
				var revocationsB = new byte[1];
				var poolSizeB = new byte[4];
				var bitsB = new byte[4];
				var sBitsB = new byte[8];
				var heightB = new byte[4];
				var sizeB = new byte[4];
				var timestampB = new byte[4];
				var nonceB = new byte[4];
				var extraDataB = new byte[32];
				var stakeVersionB = new byte[4];
				stream.ReadWriteBytes(versionB, 0, 4);
				stream.ReadWriteBytes(prevBlockHashB, 0, 32);
				stream.ReadWriteBytes(merkleRootB, 0, 32);
				stream.ReadWriteBytes(stakeRootB, 0, 32);
				stream.ReadWriteBytes(voteBitsB, 0, 2);
				stream.ReadWriteBytes(finalStateB, 0, 6);
				stream.ReadWriteBytes(votersB, 0, 2);
				stream.ReadWriteBytes(freshStakeB, 0, 1);
				stream.ReadWriteBytes(revocationsB, 0, 1);
				stream.ReadWriteBytes(poolSizeB, 0, 4);
				stream.ReadWriteBytes(bitsB, 0, 4);
				stream.ReadWriteBytes(sBitsB, 0, 8);
				stream.ReadWriteBytes(heightB, 0, 4);
				stream.ReadWriteBytes(sizeB, 0, 4);
				stream.ReadWriteBytes(timestampB, 0, 4);
				stream.ReadWriteBytes(nonceB, 0, 4);
				stream.ReadWriteBytes(extraDataB, 0, 32);
				stream.ReadWriteBytes(stakeVersionB, 0, 4);
				var height = BitConverter.ToUInt32(heightB, 0);
			}
			//protected override HashStreamBase CreateHashStream()
			//{
			//	return BufferedHashStream.CreateFrom(Blake256, 32);
			//}
		}

#pragma warning disable CS0618 // Type or member is obsolete
		public class DecredBlock(Decred.DecredBlockHeader header) : Block(header)
		{
			public override void ReadWrite(BitcoinStream stream)
			{
				DecredBlockHeader header = new DecredBlockHeader();
				stream.ReadWrite(ref header);
				uint txCount = 0;
				stream.ReadWriteAsVarInt(ref txCount);
				var txn = new DecredTransaction[txCount];
				for (int i = 0; i < txCount; i++)
				{
					DecredTransaction tx = new DecredTransaction();
					stream.ReadWrite(ref tx);
					txn[i] = tx;
				}
			}
			public override ConsensusFactory GetConsensusFactory()
			{
				return DecredConsensusFactory.Instance;
			}
		}

		protected override NetworkBuilder CreateMainnet()
		{
			return new NetworkBuilder()
			.SetNetworkSet(this)
			.SetConsensus(new Consensus()
			{
				PowLimit = new Target(new uint256("0x00000000ffff0000000000000000000000000000000000000000000000000000")),
				MinimumChainWork = new uint256("0x000000000000000000000000000000000000000000243845fb2fb3d8f20ddfeb"),
				ConsensusFactory = DecredConsensusFactory.Instance,
			}
			)
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, [0x07, 0x3f]) // starts with Ds (pubkey hash)
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, [0x07, 0x1a]) // starts with Dc (script hash)
			.SetBase58Bytes(Base58Type.SECRET_KEY, [0x22, 0xde]) // starts with Pm
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, [0x02, 0xfd, 0xa9, 0x26]) // starts with dpub
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, [0x02, 0xfd, 0xa4, 0xe8]) // starts with dprv
			.SetPort(9108)
			.SetRPCPort(9109)
			.SetName("dcr-main")
			.SetGenesis("0100000000000000000000000000000000000000000000000000000000000000000000000dc101dfc3c6a2eb10ca0c5374e10d28feb53f7eabcc850511ceadb99174aa66000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffff011b00c2eb0b000000000000000000000000a0d7b856000000000000000000000000000000000000000000000000000000000000000000000000000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff00ffffffff010000000000000000000020801679e98561ada96caec2949a5d41c4cab3851eb740d951c10ecbcf265c1fd9000000000000000001ffffffffffffffff00000000ffffffff02000000");
		}

		protected override NetworkBuilder CreateRegtest()
		{
			return new NetworkBuilder()
			.SetNetworkSet(this)
			.SetConsensus(new Consensus()
			{
				PowLimit = new Target(new uint256("0x7fffff0000000000000000000000000000000000000000000000000000000000")),
				ConsensusFactory = DecredConsensusFactory.Instance,
			}
			)
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, [0x0e, 0x91]) // starts with Ss (pubkey hash)
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, [0x0e, 0x6c]) // starts with Sc (script hash)
			.SetBase58Bytes(Base58Type.SECRET_KEY, [0x23, 0x07]) // starts with Ps
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, [0x04, 0x20, 0xbd, 0x3d]) // starts with spub
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, [0x04, 0x20, 0xb9, 0x03]) // starts with sprv
			.SetPort(18555)
			.SetRPCPort(18556)
			.SetName("dcr-reg")
			.SetGenesis("010000000000000000000000000000000000000000000000000000000000000000000000925629c5582bbfc3609d71a2f4a887443c80d54a1fe31e95e95d42f3e288945c000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffff7f200000000000000000000000000000000045068653000000000000000000000000000000000000000000000000000000000000000000000000000000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff00ffffffff0100000000000000000000434104678afdb0fe5548271967f1a67130b7105cd6a828e03909a67962e0ea1f61deb649f6bc3f4cef38c4f35504e51ec112de5c384df7ba0b8d578a4c702b6bf11d5fac000000000000000001000000000000000000000000000000004d04ffff001d0104455468652054696d65732030332f4a616e2f32303039204368616e63656c6c6f72206f6e206272696e6b206f66207365636f6e64206261696c6f757420666f722062616e6b7300");
		}

		protected override NetworkBuilder CreateTestnet()
		{
			return new NetworkBuilder()
			.SetNetworkSet(this)
			.SetConsensus(new Consensus()
			{
				PowLimit = new Target(new uint256("0x000000ffff000000000000000000000000000000000000000000000000000000")),
				ConsensusFactory = DecredConsensusFactory.Instance,
			}
			)
			.SetBase58Bytes(Base58Type.PUBKEY_ADDRESS, [0x0f, 0x21]) // starts with Ts (pubkey hash)
			.SetBase58Bytes(Base58Type.SCRIPT_ADDRESS, [0x0e, 0xfc]) // starts with Tc (script hash)
			.SetBase58Bytes(Base58Type.SECRET_KEY, [0x23, 0x0e]) // starts with Pt
			.SetBase58Bytes(Base58Type.EXT_PUBLIC_KEY, [0x04, 0x35, 0x87, 0xd1]) // starts with tpub
			.SetBase58Bytes(Base58Type.EXT_SECRET_KEY, [0x04, 0x35, 0x83, 0x97]) // starts with tprv
			.SetPort(19108)
			.SetRPCPort(19109)
			.SetName("dcr-test")
			.SetGenesis("0600000000000000000000000000000000000000000000000000000000000000000000002c0ad603d44a16698ac951fa22aab5e7b30293fa1d0ac72560cdfcc9eabcdfe7000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000ffff001e002d3101000000000000000000000000808f675b1aa4ae180000000000000000000000000000000000000000000000000000000000000000060000000101000000010000000000000000000000000000000000000000000000000000000000000000ffffffff00ffffffff010000000000000000000020801679e98561ada96caec2949a5d41c4cab3851eb740d951c10ecbcf265c1fd9000000000000000001ffffffffffffffff00000000ffffffff02000000");
		}

	}
}
