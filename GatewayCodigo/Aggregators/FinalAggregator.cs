using GatewayCodigo.Models;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Collections.Generic;

namespace GatewayCodigo
{
    public class FinalAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {

            //La información aquí llega en un json
            var productsContent = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var categoriesContent = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var customersContent = await responses[2].Items.DownstreamResponse().Content.ReadAsStringAsync();



            //Mis listas de objetos
            List<Product>? products = null;
            List<Category>? categories = null;
            List<Customer>? customers = null;


            //Convertir de json=>lista de objetos
            products = JsonConvert.DeserializeObject<List< Product >  >(productsContent);
            categories = JsonConvert.DeserializeObject<List<Category>>(categoriesContent);
            customers = JsonConvert.DeserializeObject<List<Customer>>(customersContent);



            //Usar join de linq con expresiones lambda         

            var productWithDetails = products
            .Join(customers,
                  product => product.CustomerId,
                  customer => customer.Id,
                  (product, customer) => new { product, customer })
            .Join(categories,
                  pc => pc.product.CategoryId,
                  category => category.Id,
                  (pc, category) => new ProductWithDetails
                  {
                      ProductId = pc.product.Id,
                      ProductName = pc.product.Name,
                      CustomerName = pc.customer.FirstName,
                      CategoryDescription = category.Description
                  }).ToList();




            //Convertir de lista de objetos=>json
            var jsonresult = JsonConvert.SerializeObject(productWithDetails);

            var stringContent = new StringContent(jsonresult);
            return new DownstreamResponse(stringContent, System.Net.HttpStatusCode.OK, new List<KeyValuePair<string, IEnumerable<string>>>(), "application/json");
        }
    }
}
