using Newtonsoft.Json;

namespace Pokemon.Data.Models
{
    public class Error
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string LogReferenceId { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
