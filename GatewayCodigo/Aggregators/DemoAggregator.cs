using GatewayCodigo.Models;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Collections.Generic;

namespace GatewayCodigo
{
    public class DemoAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var productContent = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var customerContent = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();


            ICollection<Product>? products = null;
            ICollection<Customer>? customers = null;

            products = JsonConvert.DeserializeObject< ICollection < Product >  >(productContent);

            customers = JsonConvert.DeserializeObject<ICollection<Customer>>(customerContent);


            var combinedContent = $"[Products: {productContent}, Customers: {customerContent}]";

            var stringContent = new StringContent(combinedContent);
            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "application/json");
        }
    }
}
