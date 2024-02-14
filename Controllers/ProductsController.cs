using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServerApp.Controllers
{
    //localhost:5000/api/Products şeklinde url 'ler ile çalışacağım.
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static string[] Products = {
            "Samsung S6","Samsung S7","Samsung S8"
        };
        //yukarıdaki Products bilgilerini sanki veritabanından getirir
        //gibi getireceğim.
        //localhost:5000/api/Products
        [HttpGet]
        public string[] GetProducts(){
            return Products;
        }

        //localhost:5000/api/Products/2 --> 2. index 'deki elemanı getirecek
        [HttpGet("{id}")]
        public string GetProduct(int id){
            if(Products.Length-1<id)
                return ""; //girilen index, dizi boyundan fazla ise hata donmek yerıne bos sayfa döndüm şimdilik
            return Products[id]; //Products dizisinin ilgili index 'deki elemanı
        }
    }
}