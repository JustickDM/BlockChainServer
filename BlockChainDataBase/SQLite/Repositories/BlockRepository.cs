using BlockChainDataBase.Context;
using BlockChainDataBase.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockChainDataBase.SQLite.Repositories
{
	public class BlockRepository
	{
		private RepositoryContext _db;

		public BlockRepository(string databasePath)
		{
			_db = new RepositoryContext(databasePath);
		}

		public async Task<List<Block>> GetEntityListAsync()
		{
			try
			{
				var Blocks = await _db.Blocks.Include(c => c.Transactions).ToListAsync();
				return Blocks;
			}
			catch (Exception ex)
			{
				return new List<Block>();
			}
		}

		public async Task<Block> GetEntityAsync(int BlockId)
		{
			try
			{
				var Block = await _db.Blocks.FindAsync(BlockId);
				return Block;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<bool> AddAsync(Block Block)
		{
			try
			{
				var tracking = await _db.Blocks.AddAsync(Block);
				await _db.SaveChangesAsync();
				var isAdded = tracking.State == EntityState.Added;
				return isAdded;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> AddRangeAsync(List<Block> Blocks)
		{
			try
			{
				await _db.Blocks.AddRangeAsync(Blocks);
				await _db.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<Block> GetLastBlock()
		{
			try
			{
				var blockId = await _db.Blocks.MaxAsync(m => m.BlockId);
				var block = await _db.Blocks.FindAsync(blockId);
				return block ?? null;
			}
			catch (Exception ex)
			{
				return null;
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
