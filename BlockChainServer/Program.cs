using BlockChain.Classes;
using System;

namespace BlockChainServer
{
	class Program
	{
		static void Main(string[] args)
		{
			var chain = new BlockChainMethods();
			var server = new WebServer(chain);
			Console.Read();
		}
	}
}
