using Microsoft.EntityFrameworkCore;

using BlockChainDataBase.Entities;

namespace BlockChainDataBase.Context
{
	internal class RepositoryContext: DbContext
	{
		private string _databasePath;

		public DbSet<Block> Blocks { get; set; }
		public DbSet<Transaction> Transactions { get; set; }

		public RepositoryContext(string databasePath)
		{
			_databasePath = databasePath;
			Database.EnsureCreated();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Filename={_databasePath}");
		}
	}
}
