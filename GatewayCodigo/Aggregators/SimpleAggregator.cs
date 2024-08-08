using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace GatewayCodigo
{
    public class SimpleAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var productContent = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var customerContent = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();



            var combinedContent = $"[Products: {productContent}, Customers: {customerContent}]";

            var stringContent = new StringContent(combinedContent);
            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, 
                new List<KeyValuePair<string, IEnumerable<string>>>(), "application/json");
        }
    }
}
