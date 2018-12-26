using System;
using System.Collections.Generic;

namespace BlockChain.Models
{
	public class Block
	{
		public int BlockId { get; set; }
		public DateTime Timestamp { get; set; }
		public List<Transaction> Transactions { get; set; }
		public int Proof { get; set; }
		public string PreviousHash { get; set; }

		public override string ToString()
		{
			return $"{BlockId} [{Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}] Proof: {Proof} | PrevHash: {PreviousHash} | Trx: {Transactions.Count}";
		}
	}
}