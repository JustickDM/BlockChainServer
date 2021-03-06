﻿using BlockChain.Models;

using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;

namespace BlockChain.Classes
{
	public class WebServer
	{
		public WebServer(BlockChainMethods chain)
		{
			var settings = ConfigurationManager.AppSettings;
			string host = settings["host"]?.Length > 1 ? settings["host"] : "localhost";
			string port = settings["port"]?.Length > 1 ? settings["port"] : "12345";

			var server = new TinyWebServer.WebServer(request =>
				{
					string path = request.Url.PathAndQuery.ToLower();
					string query = "";
					string json = "";
					if (path.Contains("?"))
					{
						string[] parts = path.Split('?');
						path = parts[0];
						query = parts[1];
					}

					switch (path)
					{
						//GET: http://localhost:12345/mine
						case "/mine":
							Console.WriteLine("Mine");
							return chain.Mine().Result;

						//POST: http://localhost:12345/transactions/new
						//{ "Amount":123, "Recipient":"ebeabf5cc1d54abdbca5a8fe9493b479", "Sender":"31de2e0ef1cb4937830fcfd5d2b3b24f" }
						case "/transactions/new":
							if (request.HttpMethod != HttpMethod.Post.Method)
								return $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";

							json = new StreamReader(request.InputStream).ReadToEnd();
							Transaction trx = JsonConvert.DeserializeObject<Transaction>(json);
							int blockId = chain.CreateTransaction(trx.Sender, trx.Recipient, trx.Amount, trx.Name);
							Console.WriteLine(trx.ToString());
							return $"Ваша транзакция будет включена в блок {blockId}";

						//GET: http://localhost:12345/chain
						case "/chain":
							Console.WriteLine("GetFullChain");
							return chain.GetFullChain();

						//POST: http://localhost:12345/nodes/register
						//{ "Urls": ["localhost:54321", "localhost:54345", "localhost:12321"] }
						case "/nodes/register":
							if (request.HttpMethod != HttpMethod.Post.Method)
								return $"{new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)}";

							json = new StreamReader(request.InputStream).ReadToEnd();
							var urlList = new { Urls = new string[0] };
							var obj = JsonConvert.DeserializeAnonymousType(json, urlList);
							return chain.RegisterNodes(obj.Urls);

						//GET: http://localhost:12345/nodes/resolve
						case "/nodes/resolve":
							return chain.Consensus();
					}

					return "";
				},
				$"http://{host}:{port}/mine/",
				$"http://{host}:{port}/transactions/new/",
				$"http://{host}:{port}/chain/",
				$"http://{host}:{port}/nodes/register/",
				$"http://{host}:{port}/nodes/resolve/"
			);

			server.Run();
		}
	}
}
