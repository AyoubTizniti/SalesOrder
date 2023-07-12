using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HTTP_trigger_LA
{
    public static class Function1
    {
        private static readonly string EndpointUrl = "https://cosmos-ayoub.documents.azure.com:443/";
        private static readonly string PrimaryKey = "nvknHbYlSTJxjMK7PI5dftsLNtpc28s6kRUmUGIDbiYv0HHpsLFsaBM1HHzKjBYVhpPMFnoCy8OTACDbUHe62w==";
        private static readonly string DatabaseId = "Integration";
        private static readonly string ContainerId = "salesorders";

        private static readonly Lazy<CosmosClient> CosmosClientLazy = new Lazy<CosmosClient>(() =>
        {
            return new CosmosClient(EndpointUrl, PrimaryKey);
        });

        private static CosmosClient CosmosClient => CosmosClientLazy.Value;

        [FunctionName("post_request_create_record")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                JObject payload = JObject.Parse(requestBody);

                JToken orderToken = payload.SelectToken("value.$content.CDM_Order.RootNode");
                if (orderToken != null)
                {
                    string orderId = orderToken.Value<string>("Id");
                    DateTime orderDate = orderToken.Value<DateTime>("Date");
                    string customerId = orderToken.Value<string>("CustomerId");
                    string customerName = orderToken.Value<string>("Name");

                    Order order = new Order
                    {
                        Id = orderId,
                        PartitionKey = customerId,
                        SalesOrder = new SalesOrder
                        {
                            RootNode = new RootNode
                            {
                                Id = orderId,
                                Date = orderDate,
                                CustomerId = customerId,
                                Name = customerName,
                                Parties = new Parties
                                {
                                    Party = new Party
                                    {
                                        Function = orderToken.SelectToken("Parties.Party.Function")?.Value<string>(),
                                        StreetAndNumber = orderToken.SelectToken("Parties.Party.StreetAndNumber")?.Value<string>(),
                                        PostalCode = orderToken.SelectToken("Parties.Party.PostalCode")?.Value<string>(),
                                        City = orderToken.SelectToken("Parties.Party.City")?.Value<string>(),
                                        Country = orderToken.SelectToken("Parties.Party.Country")?.Value<string>()
                                    }
                                },
                                Lines = new Lines
                                {
                                    Line = orderToken.SelectToken("Lines.Line")
                                        .ToObject<List<Line>>()
                                }
                            }
                        }
                    };

                    Container container = CosmosClient.GetContainer(DatabaseId, ContainerId);
                    ItemResponse<Order> response = await container.UpsertItemAsync(order, new PartitionKey(order.PartitionKey));

                    if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // Item created or updated successfully
                        log.LogInformation("Order record stored successfully in Cosmos DB.");
                        return new OkObjectResult(order.SalesOrder.RootNode.CustomerId);
                    }
                    else
                    {
                        // Failed to create or update item in Cosmos DB
                        log.LogError("Failed to store order record in Cosmos DB.");
                    }
                }
                else
                {
                    log.LogError("Invalid payload format. Missing order information.");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing request");
            }

            return new BadRequestResult();
        }
    }

    public class Order
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty("salesorder")]
        public SalesOrder SalesOrder { get; set; }
    }

    public class SalesOrder
    {
        [JsonProperty("RootNode")]
        public RootNode RootNode { get; set; }
    }

    public class RootNode
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("CustomerId")]
        public string CustomerId { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Parties")]
        public Parties Parties { get; set; }

        [JsonProperty("Lines")]
        public Lines Lines { get; set; }
    }

    public class Parties
    {
        [JsonProperty("Party")]
        public Party Party { get; set; }
    }

    public class Party
    {
        [JsonProperty("Function")]
        public string Function { get; set; }

        [JsonProperty("StreetAndNumber")]
        public string StreetAndNumber { get; set; }

        [JsonProperty("PostalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }
    }

    public class Lines
    {
        [JsonProperty("Line")]
        public List<Line> Line { get; set; }
    }

    public class Line
    {
        [JsonProperty("Sequence")]
        public string Sequence { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("ProductNumber")]
        public string ProductNumber { get; set; }

        [JsonProperty("UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        [JsonProperty("Quantity")]
        public string Quantity { get; set; }

        [JsonProperty("TotalPrice")]
        public string TotalPrice { get; set; }
    }
}
