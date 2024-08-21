[System.Serializable]
public class UserData
{
    public string Login;
    public string Enail;
    public string Password;
    public string ConfirmPassword;
    public UserData(string Login, string Enail, string Password, string ConfirmPassword)
    {
        this.Login = Login;
        this.Enail = Enail;
        this.Password = Password;
        this.ConfirmPassword = ConfirmPassword;
    }
}
