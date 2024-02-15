using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    //localhost:5000/api/Products şeklinde url 'ler ile çalışacağım.
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly SocialContext _socialContext;
        public ProductsController(SocialContext socialContext)
        {
            _socialContext = socialContext;
        } 

        //localhost:5000/api/Products
        [HttpGet]
        public async Task<ActionResult> GetProducts(){
            var products = await _socialContext.Products.ToListAsync();
            return Ok(products);
        }

        //localhost:5000/api/Products/2 --> 2. id 'deki elemanı getirecek
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id){
            var p = await _socialContext.Products.FirstOrDefaultAsync(x=>x.ProductId == id);
            if(p==null)
                return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity){
            _socialContext.Products.Add(entity);
            await _socialContext.SaveChangesAsync();

            //ekleme işleminden sonra ürün eklendi mi kontrolü yapar.
            //id bilgisi ile GetProduct 'a gider
            //eklenen ürünü bulursa "201 Created" döner.
            return CreatedAtAction(nameof(GetProduct), new {id=entity.ProductId}, entity);
        }
    }
}