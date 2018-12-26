using BlockChain.Models;
using bcdb = BlockChainDataBase.Entities;
using BlockChainDataBase.SQLite;

using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockChain.Converters
{
	public class BlockChainConverter
	{
		public bcdb.Transaction Convert(Transaction transaction, int maxBlock)
		{
			var newTransaction = new bcdb.Transaction()
			{
				Sender = transaction.Sender,
				Recipient = transaction.Recipient,
				Amount = transaction.Amount,
				Name = transaction.Name,
				BlockId = maxBlock
			};

			return newTransaction;
		}

		public Transaction Convert(bcdb.Transaction transactionDB)
		{
			var newTransaction = new Models.Transaction()
			{
				Sender = transactionDB.Sender,
				Recipient = transactionDB.Recipient,
				Amount = transactionDB.Amount,
				Name = transactionDB.Name,
				BlockId = transactionDB.BlockId
			};

			return newTransaction;
		}

		public async Task<List<bcdb.Transaction>> Convert(List<Transaction> transactions)
		{
			var sqliteRepository = new SQLiteRepository();
			var maxBlock = await sqliteRepository.Blocks.GetLastBlock();

			var transactionList = new List<bcdb.Transaction>();

			transactions.ForEach(t => transactionList.Add(Convert(t, maxBlock.BlockId)));

			return transactionList;
		}

		public List<Transaction> Convert(List<bcdb.Transaction> transactionsDB)
		{
			var transactionList = new List<Transaction>();

			transactionsDB.ForEach(t => transactionList.Add(Convert(t)));

			return transactionList;
		}

		public async Task<bcdb.Block> Convert(Block block)
		{
			var transactionList = await Convert(block.Transactions);

			var newBlock = new bcdb.Block()
			{
				BlockId = block.BlockId,
				Timestamp = block.Timestamp,
				Transactions = transactionList,
				Proof = block.Proof,
				PreviousHash = block.PreviousHash
			};

			return newBlock;
		}

		public Block Convert(bcdb.Block blockDB)
		{
			var transactionList = Convert(blockDB.Transactions.ToList());

			var newBlock = new Block()
			{
				BlockId = blockDB.BlockId,
				Timestamp = blockDB.Timestamp,
				Transactions = transactionList,
				Proof = blockDB.Proof,
				PreviousHash = blockDB.PreviousHash
			};

			return newBlock;
		}

		public List<bcdb.Block> Convert(List<Block> blocks)
		{
			var blockList = new List<bcdb.Block>();

			blocks.ForEach(async (b) => blockList.Add(await Convert(b)));

			return blockList;
		}

		public List<Block> Convert(List<bcdb.Block> blocks)
		{
			var blockList = new List<Block>();

			blocks.ForEach((b) => blockList.Add(Convert(b)));

			return blockList;
		}
	}
}
