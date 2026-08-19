namespace PrometheusBot.Helper;

public static class AuthorizationHelper
{
    public static bool IsAuthorized(string[] allowed, string id)
    {
        foreach (string a in allowed)
        {
            if (id == a)
                return true;
        }
        Console.WriteLine("Access for '{0}' denied", id);
        return false;
    }
}