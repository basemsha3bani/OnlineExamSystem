using System;
using System.Collections.Generic;
using System.Text;

namespace DataRepository.DataRepositoryEntities
{
    public class User:IRepository { 
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } 
        public string Role { get; set; } } // Role = "Admin" or "Examiner"
}
