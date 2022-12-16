namespace IdentityJwtPoc.Application.Services.Interfaces
{
    public interface ICookieService
    {
        bool IsJwtFingerPrintValid(string claimFingerPrint);
        void CreateCookie(string key, string value, int expirationTime);
        string GetCookieValue(string key);
        void DeleteCookie(string key);
    }
}
