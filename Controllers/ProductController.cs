using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ThinkBridge.Models;

namespace ThinkBridge.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> addproduct([FromBody] dynamic postData)
        {
            JObject data = JObject.Parse(postData.ToString());
            var name = Convert.ToString(data["name"]).Trim();
            var mrp = Convert.ToDecimal(data["mrp"]);
            var price = Convert.ToDecimal(data["price"]);
            var discount = Convert.ToDecimal(data["discount"]);
            var description = Convert.ToString(data["description"]);

            string status = await new DBProducts().AddProduct(name, mrp, price, discount, description, Request);
            if (status == "")
            {
                return Ok(new { Result = "Saved", success = true });
            }
            else
            {
                return NotFound(new { Result = status, success = false });
            }
        }

        [HttpPut]
        public async Task<ActionResult> updateproduct([FromBody] dynamic postData)
        {
            JObject data = JObject.Parse(postData.ToString());
            var id = Convert.ToInt32(data["id"]);
            var name = Convert.ToString(data["name"]).Trim();
            var mrp = Convert.ToDecimal(data["mrp"]);
            var price = Convert.ToDecimal(data["price"]);
            var discount = Convert.ToDecimal(data["discount"]);
            var description = Convert.ToString(data["description"]);

            dynamic obj = await new DBProducts().UpdateProduct(id, name, mrp, price, discount, description, Request);
            if (obj.status)
            {
                return Ok(new { Result = "Updated", success = true });
            }
            else
            {
                return NotFound(new { Result = obj.error, success = false });
            }
        }

        [HttpDelete]
        public async Task<ActionResult> deleteproduct([FromBody] dynamic postData)
        {
            JObject data = JObject.Parse(postData.ToString());
            var id = Convert.ToInt32(data["id"]);

            dynamic obj = await new DBProducts().DeleteProduct(id);
            if (obj.success)
            {
                return Ok(new { Result = "Deleted", success = true });
            }
            else
            {
                return NotFound(new { Result = obj.error, success = false });
            }
        }


        public async Task<IEnumerable<Product>> getproducts()
        {
            return await new DBProducts().GetProducts();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> getproduct(int id)
        {
            var product = await new DBProducts().GetProducts(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }
    }
}