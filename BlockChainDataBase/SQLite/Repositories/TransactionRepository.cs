using BlockChainDataBase.Context;
using BlockChainDataBase.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockChainDataBase.SQLite.Repositories
{
	public class TransactionRepository
	{
		private RepositoryContext _db;

		public TransactionRepository(string databasePath)
		{
			_db = new RepositoryContext(databasePath);
		}

		public async Task<List<Transaction>> GetEntityListAsync()
		{
			try
			{
				var Transactions = await _db.Transactions.Include(c => c.Block).ToListAsync();
				return Transactions;
			}
			catch (Exception ex)
			{
				return new List<Transaction>();
			}
		}

		public async Task<Transaction> GetEntityAsync(int TransactionId)
		{
			try
			{
				var Transaction = await _db.Transactions.FindAsync(TransactionId);
				return Transaction;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<bool> AddAsync(Transaction Transaction)
		{
			try
			{
				var tracking = await _db.Transactions.AddAsync(Transaction);
				await _db.SaveChangesAsync();
				var isAdded = tracking.State == EntityState.Added;
				return isAdded;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> AddRangeAsync(List<Transaction> Transactions)
		{
			try
			{
				await _db.Transactions.AddRangeAsync(Transactions);
				await _db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		private bool disposed = false;

		public virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					_db.Dispose();
				}
			}
			this.disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
