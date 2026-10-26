namespace Domain.DTOs;
public sealed class RegisterDTO : LoginDTO
{
    [System.Text.Json.Serialization.JsonConstructor]
    private RegisterDTO() : base(true) { }
    public new static RegisterDTO Define(int Id = 0, string Username = null, string Password = null, string Role = null)
    {
        var result = new RegisterDTO();
        result.ChangeId(Id); result.ChangeUsername(Username); result.ChangePassword(Password); result.ChangeRole(Role);
        return result;
    }
}
