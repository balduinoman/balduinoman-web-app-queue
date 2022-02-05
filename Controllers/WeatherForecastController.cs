using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Configuration; // Namespace for ConfigurationManager
//using Azure.Identity;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using Microsoft.Extensions.Configuration;

namespace balduinoman_web_app_queue.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<PeekedMessage[]> Get()
        {
            PeekedMessage[] peekedMessage = null;
            // Get the connection string from app settings
            string connectionString = _configuration.GetConnectionString("StorageConnectionString");

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, "messages");

            if (queueClient.Exists())
            { 
                // Peek at the next message
                peekedMessage = queueClient.PeekMessages();
            }

            return peekedMessage;
        }

        [HttpPost]
        public void Post()
        {
            // Get the connection string from app settings
            string connectionString = _configuration.GetConnectionString("StorageConnectionString");

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(connectionString, "messages");

            // Create the queue if it doesn't already exist
            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                // Send a message to the queue
                queueClient.SendMessage("message");
            }
        }
    }
}
