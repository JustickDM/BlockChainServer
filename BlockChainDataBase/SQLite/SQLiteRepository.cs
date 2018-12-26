using BlockChainDataBase.SQLite.Repositories;

namespace BlockChainDataBase.SQLite
{
	public class SQLiteRepository
	{
		private readonly string Path = "DataBase.db";

		public TransactionRepository Transactions;
		public BlockRepository Blocks;

		public SQLiteRepository()
		{
			Transactions = new TransactionRepository(Path);
			Blocks = new BlockRepository(Path);
		}
	}
}
