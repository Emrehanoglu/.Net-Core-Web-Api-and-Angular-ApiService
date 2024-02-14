using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    //localhost:5000/api/Products şeklinde url 'ler ile çalışacağım.
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products;
        public ProductsController()
        {
            _products = new List<Product>();
            _products.Add(new Product{ProductId=1,Name="Samsung S6", Price=3000, IsActive=false});
            _products.Add(new Product{ProductId=2,Name="Samsung S7", Price=4000, IsActive=true});
            _products.Add(new Product{ProductId=3,Name="Samsung S8", Price=5000, IsActive=true});
            _products.Add(new Product{ProductId=4,Name="Samsung S9", Price=6000, IsActive=false});
            _products.Add(new Product{ProductId=5,Name="Samsung S10", Price=7000, IsActive=true});
        } 
        //yukarıdaki Products bilgilerini sanki veritabanından getirir
        //gibi getireceğim.
        //localhost:5000/api/Products
        [HttpGet]
        public List<Product> GetProducts(){
            return _products;
        }

        //localhost:5000/api/Products/2 --> 2. index 'deki elemanı getirecek
        [HttpGet("{id}")]
        public IActionResult GetProduct(int id){
            var p = _products.FirstOrDefault(x=>x.ProductId == id);
            if(p==null)
                return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public IActionResult CreateProduct(Product p){
            _products.Add(p); //Server tarafına ürün eklendi.
            return Ok(p); //eklediğim ürün bilgisini Client tarafına gönderdim.
        }
    }
}