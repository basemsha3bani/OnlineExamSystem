namespace Domain.DTOs
{
    public class LoginDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; } 
        [System.Text.Json.Serialization.JsonInclude]
        public string Username { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string Password { get; private set; }

        [System.Text.Json.Serialization.JsonInclude]
        public string Role { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private LoginDTO() { }
        protected LoginDTO(bool registration) { }
        public static LoginDTO Define(int Id = default, string Username = default, string Password = default, string Role = default)
        {
            var value = new LoginDTO();
            value.Id = Id;
            value.Username = Username;
            value.Password = Password;
            value.Role = Role;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeUsername( string value) => Username = value;
        public void ChangePassword( string value) => Password = value;
        public void ChangeRole( string value) => Role = value;
}

}