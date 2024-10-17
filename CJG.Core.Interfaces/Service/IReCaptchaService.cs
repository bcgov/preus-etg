namespace CJG.Core.Interfaces.Service
{
    public interface IReCaptchaService
    {
        bool IsEnabled();
        string GetSiteKey();
        bool Validate(string encodedResponse, ref string errorCodes);
    }
}
