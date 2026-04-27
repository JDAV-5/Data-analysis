using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETLService.Security.Model
{
    public class UserModel
    {
        // Usuario para login
        public string username { get; set; } = "";

        // ontraseña para login
        public string password { get; set; } = "";
    }
}