using Microsoft.EntityFrameworkCore;

using Repository.Entities;

namespace Repository.Context
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
