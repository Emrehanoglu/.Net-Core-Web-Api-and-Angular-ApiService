using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using ServerApp.Models;

namespace ServerApp.Data
{
    public static class SeedDatabase
    {
        public static async Task Seed(UserManager<User> userManager){
            if(!userManager.Users.Any()){//database içerisinde veri yok ise
                //user.json içerisinden verileri okuyup database 'e ekleyeceğim
                var users = System.IO.File.ReadAllText("Data/users.json");

                //json formatında olan user.json dosyası içerisindeki verileri Deserialize ediyorum 
                var listOfUsers = JsonConvert.DeserializeObject<List<User>>(users);

                foreach(var user in listOfUsers){
                    await userManager.CreateAsync(user,"Abc123.");
                }
            }
        }
    }
}