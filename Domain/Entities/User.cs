using Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }
        private User() { }
        public static User Define(int Id = default, string Username = default, string PasswordHash = default, string Role = default)
        {
            var value = new User();
            value.ChangeId(Id);
            value.ChangeUsername(Username);
            value.ChangePasswordHash(PasswordHash);
            value.ChangeRole(Role);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeUsername(string value) => Username = EntityRules.Required(value, "Username");
        public void ChangePasswordHash(string value) => PasswordHash = EntityRules.Required(value, "PasswordHash");
        public void ChangeRole(string value) => Role = EntityRules.Required(value, "Role");
    } // Role = "Admin" or "Examiner"
}
