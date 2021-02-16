using AspMongo.Models;
using AspMongo.Shares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspMongo.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly string _connectionString = "mongodb://root:Hpbvn123@localhost:27777";
		private static readonly MongoDB.Bson.IO.JsonWriterSettings _confgJson = new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true };
		private Action<MongoDB.Driver.Core.Configuration.ClusterBuilder> _actionDelegateHandleCommandStartedEvent = 
			new Action<MongoDB.Driver.Core.Configuration.ClusterBuilder>(cb=> {
				cb.Subscribe<CommandStartedEvent>(e => {
					MyConsole.Write("Start query name ", ConsoleColor.Red);
					MyConsole.Write(e.CommandName, ConsoleColor.Yellow);
					MyConsole.Write(": ", ConsoleColor.Red);
					MyConsole.NewLine();
					MyConsole.WriteLine($"{e.Command.ToJson(_confgJson)}");
					MyConsole.WriteLine("End query db", ConsoleColor.Yellow);
				});
			});
		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			var mongoConnectionUrl = new MongoUrl(_connectionString);
			var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
			mongoClientSettings.ClusterConfigurator = _actionDelegateHandleCommandStartedEvent;
			var client = new MongoClient(mongoClientSettings);
			var database = client.GetDatabase("test");
			var collection = database.GetCollection<BsonDocument>("inventory");
			var filterBuilder = Builders<BsonDocument>.Filter;
			var filter = filterBuilder.Eq("status", "A");
			filter = filter & filterBuilder.Lt("qty", 30);
			var document = await collection.FindAsync(filter);
			MyConsole.WriteLine(document.ToList().ToJson(_confgJson));
			return View();
		}

		public async Task<IActionResult> Privacy()
		{
			var mongoConnectionUrl = new MongoUrl(_connectionString);
			var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
			mongoClientSettings.ClusterConfigurator = _actionDelegateHandleCommandStartedEvent;
			var client = new MongoClient(mongoClientSettings);
			var database = client.GetDatabase("testDB");
			var collection = database.GetCollection<BsonDocument>("user");
			var count = await collection.CountDocumentsAsync(new BsonDocument());
			ViewData["count"] = count;
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

	}
}
