using System;
using System.Collections.Generic;

namespace BlockChainDataBase.Entities
{
	public class Block
	{
		public int BlockId { get; set; }
		public DateTime Timestamp { get; set; }
		public int Proof { get; set; }
		public string PreviousHash { get; set; }

		public virtual ICollection<Transaction> Transactions { get; set; }

		public Block()
		{
			Transactions = new List<Transaction>();
		}
	}
}
