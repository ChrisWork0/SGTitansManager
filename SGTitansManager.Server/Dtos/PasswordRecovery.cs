namespace SGTitansManager.Server.Dtos;

public class PasswordRecovery
{
    public string NewPassword { get; set; } = "";
    public string RecoveryCode { get; set; } = "";
}