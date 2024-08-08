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

            //La información aquí llega en un json
            var productsContent = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var customersContent = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();

            //Mis listas de objetos
            List<Product>? products = null;
            List<Customer>? customers = null;


            //Convertir de json=>lista de objetos
            products = JsonConvert.DeserializeObject<List< Product >  >(productsContent);
            customers = JsonConvert.DeserializeObject<List<Customer>>(customersContent);


            //Usar join de linq con expresiones lambda
           var productWithCustomers = products.Join(
           customers,
           product => product.CustomerId,
           customer => customer.Id,
           (product, customer) => new ProductWithCustomer
           {
               ProductId = product.Id,
               ProductName = product.Name,
               CustomerName = customer.FirstName
           }).ToList();


            var combinedContent = $"[Products: {productsContent}, Customers: {customersContent}]";

            var stringContent = new StringContent(combinedContent);
            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "application/json");
        }
    }
}
