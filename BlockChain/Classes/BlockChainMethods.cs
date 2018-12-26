using BlockChain.Models;
using BlockChain.Converters;

using Newtonsoft.Json;

using BlockChainDataBase.SQLite;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlockChain.Classes
{
	public class BlockChainMethods
	{
		private List<Transaction> _currentTransactions = new List<Transaction>();
		private List<Block> _chain = new List<Block>();
		private List<Node> _nodes = new List<Node>();
		private Block _lastBlock => _chain.Last();

		private BlockChainConverter blockChainConverter = new BlockChainConverter();

		public string NodeId { get; private set; }

		public BlockChainMethods()
		{
			NodeId = Guid.NewGuid().ToString().Replace("-", "");

			GetChainFromDB();
		}

		private async void GetChainFromDB()
		{
			var sqliteRepository = new SQLiteRepository();
			var chainDB = await sqliteRepository.Blocks.GetEntityListAsync();
			if(chainDB != null)
			{
				_chain.AddRange(blockChainConverter.Convert(chainDB));
			}
			else
			{
				await CreateNewBlock(proof: 100, previousHash: "1");
			}
		}

		private void RegisterNode(string address)
		{
			_nodes.Add(new Node { Address = new Uri(address) });
		}

		private bool IsValidChain(List<Block> chain)
		{
			Block block = null;
			Block lastBlock = chain.First();
			int currentIndex = 1;
			while (currentIndex < chain.Count)
			{
				block = chain.ElementAt(currentIndex);
				Debug.WriteLine($"{lastBlock}");
				Debug.WriteLine($"{block}");
				Debug.WriteLine("----------------------------");

				//Check that the hash of the block is correct
				if (block.PreviousHash != GetHash(lastBlock))
					return false;

				//Check that the Proof of Work is correct
				if (!IsValidProof(lastBlock.Proof, block.Proof, lastBlock.PreviousHash))
					return false;

				lastBlock = block;
				currentIndex++;
			}

			return true;
		}

		private bool ResolveConflicts()
		{
			List<Block> newChain = null;
			int maxLength = _chain.Count;

			foreach (Node node in _nodes)
			{
				var url = new Uri(node.Address, "/chain");
				var request = (HttpWebRequest)WebRequest.Create(url);
				var response = (HttpWebResponse)request.GetResponse();

				if (response.StatusCode == HttpStatusCode.OK)
				{
					var model = new
					{
						chain = new List<Block>(),
						length = 0
					};
					string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
					var data = JsonConvert.DeserializeAnonymousType(json, model);

					if (data.chain.Count > _chain.Count && IsValidChain(data.chain))
					{
						maxLength = data.chain.Count;
						newChain = data.chain;
					}
				}
			}

			if (newChain != null)
			{
				_chain = newChain;
				return true;
			}

			return false;
		}

		private async Task<Block> CreateNewBlock(int proof, string previousHash = null)
		{
			var block = new Block
			{
				BlockId = _chain.Count,
				Timestamp = DateTime.Now,
				Transactions = _currentTransactions.ToList(),
				Proof = proof,
				PreviousHash = previousHash ?? GetHash(_chain.Last())
			};

			_currentTransactions.Clear();
			_chain.Add(block);

			var sqliteRepository = new SQLiteRepository();

			await sqliteRepository.Blocks.AddAsync(await blockChainConverter.Convert(block));

			return block;
		}

		private int CreateProofOfWork(int lastProof, string previousHash)
		{
			int proof = 0;
			while (!IsValidProof(lastProof, proof, previousHash))
				proof++;

			return proof;
		}

		private bool IsValidProof(int lastProof, int proof, string previousHash)
		{
			string guess = $"{lastProof}{proof}{previousHash}";
			string result = GetSha256(guess);
			return result.StartsWith("0000");
		}

		private string GetHash(Block block)
		{
			string blockText = JsonConvert.SerializeObject(block);
			return GetSha256(blockText);
		}

		private string GetSha256(string data)
		{
			var sha256 = new SHA256Managed();
			var hashBuilder = new StringBuilder();

			byte[] bytes = Encoding.Unicode.GetBytes(data);
			byte[] hash = sha256.ComputeHash(bytes);

			foreach (byte x in hash)
				hashBuilder.Append($"{x:x2}");

			return hashBuilder.ToString();
		}

		//web server calls
		internal async Task<string> Mine()
		{
			int proof = CreateProofOfWork(_lastBlock.Proof, _lastBlock.PreviousHash);

			CreateTransaction(sender: "0", recipient: NodeId, amount: 1, name: null);
			Block block = await CreateNewBlock(proof /*, _lastBlock.PreviousHash*/);

			var response = new
			{
				Message = "New Block Forged",
				Index = block.BlockId,
				Timestamp = DateTime.Now,
				Transactions = block.Transactions.ToArray(),
				Proof = block.Proof,
				PreviousHash = block.PreviousHash
			};

			return JsonConvert.SerializeObject(response);
		}

		internal string GetFullChain()
		{
			var response = new
			{
				Length = _chain.Count,
				Chain = _chain,
			};

			return JsonConvert.SerializeObject(response);
		}

		internal string RegisterNodes(string[] nodes)
		{
			var builder = new StringBuilder();
			foreach (string node in nodes)
			{
				string url = $"http://{node}";
				RegisterNode(url);
				builder.Append($"{url}, ");
			}

			builder.Insert(0, $"{nodes.Count()} new nodes have been added: ");
			string result = builder.ToString();
			return result.Substring(0, result.Length - 2);
		}

		internal string Consensus()
		{
			bool replaced = ResolveConflicts();
			string message = replaced ? "was replaced" : "is authoritive";

			var response = new
			{
				Message = $"Our chain {message}",
				Chain = _chain
			};

			return JsonConvert.SerializeObject(response);
		}

		internal int CreateTransaction(string sender, string recipient, int amount, string name)
		{
			var transaction = new Transaction
			{
				Sender = sender,
				Recipient = recipient,
				Name = name,
				Amount = amount
			};

			_currentTransactions.Add(transaction);

			return _lastBlock != null ? _lastBlock.BlockId + 1 : 0;
		}
	}
}
