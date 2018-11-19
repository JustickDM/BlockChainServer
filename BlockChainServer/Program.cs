using bc = BlockChain.Classes;
using System;

namespace BlockChainServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var chain = new bc.BlockChain();
			var server = new bc.WebServer(chain);
			Console.Read();
		}
	}
}
