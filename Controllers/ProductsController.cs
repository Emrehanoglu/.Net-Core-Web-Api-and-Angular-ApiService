using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerApp.Data;
using ServerApp.DTO;
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
            var products = await _socialContext.Products.Select(p => new ProductDTO(){
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            }).ToListAsync();
            return Ok(products);
        }

        //localhost:5000/api/Products/2 --> 2. id 'deki elemanı getirecek
        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id){
            var p = await _socialContext.Products.Select(p => new ProductDTO(){
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                IsActive = p.IsActive
            }).FirstOrDefaultAsync(x=>x.ProductId == id);
            if(p==null)
                return NotFound();
            return Ok(p);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product entity){
            _socialContext.Products.Add(entity);
            await _socialContext.SaveChangesAsync();

            //ekleme işleminden sonra ürün eklendi mi kontrolü yapar.
            //id bilgisi ile GetProduct 'a gider
            //eklenen ürünü bulursa "201 Created" döner.
            return CreatedAtAction(nameof(GetProduct), new {id=entity.ProductId}, entity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Product entity){
            //url de gönderdiğim id değeri ile postman üzerindeki body 'den gönderdiğim id 
            //değerini karşılaştırdım.
            if(id!=entity.ProductId)
            {
                return BadRequest();
            }

            var product = await _socialContext.Products.FindAsync(id);
            
            if(product == null){
                return NotFound();
            }

            product.Name = entity.Name;
            product.Price = entity.Price;

            try
            {
                await _socialContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _socialContext.Products.FindAsync(id);
            if(product==null)
                return NotFound();
            _socialContext.Remove(product);
            await _socialContext.SaveChangesAsync();
            return NoContent();
        }
    }
}