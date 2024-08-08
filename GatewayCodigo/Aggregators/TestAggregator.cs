using GatewayCodigo.Models;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Collections.Generic;

namespace GatewayCodigo
{
    public class TestAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {

            //La información aquí llega en un json
            var productsContent = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var categoriesContent = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();

            //Mis listas de objetos
            List<Product>? products = null;
            List<Category>? categories = null;


            //Convertir de json=>lista de objetos
            products = JsonConvert.DeserializeObject<List< Product >  >(productsContent);
            categories = JsonConvert.DeserializeObject<List<Category>>(categoriesContent);


            //Usar join de linq con expresiones lambda
           var productWithCustomers = products.Join(
           categories,
           product => product.CategoryId,
           category => category.Id,
           (product, category) => new ProductWithCategory
           {
               ProductId = product.Id,
               ProductName = product.Name,
               CategoryName = category.Description
           }).ToList();

            //Convertir de lista de objetos=>json
            var jsonresult = JsonConvert.SerializeObject(productWithCustomers);

            var stringContent = new StringContent(jsonresult);
            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "application/json");
        }
    }
}
