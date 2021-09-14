using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Models
{
    public class LoginModels
    {
    }

    //Login Request
    public class LoginRequestModels
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    //Login Response
    public class LoginResponseModels
    {
        public int rcode { get; set; }
        public string rmessage { get; set; }
        public int Internalid { get; set; }
        public int UniqueId { get; set; }
        public string FullName { get; set; }
        public string Token { get; set; }
    }
    //Client List Response
    public class ClientListResponseModels
    {
        public int rcode { get; set; }
        public string rmessage { get; set; }
        public List<ClientList> Result {get; set;}
    }

    public class ClientList
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
    }


}
