using Newtonsoft.Json;

namespace ServerApp.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            //gelen error bilgilerini json yapıya cevirdim
            //daha sonra bunu kullanıcıya göstereceğim.
            return JsonConvert.SerializeObject(this);
        }
    }
}