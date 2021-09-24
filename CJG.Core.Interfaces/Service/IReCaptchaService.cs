namespace CJG.Core.Interfaces.Service
{
    public interface IReCaptchaService
    {
        bool Validate(string encodedResponse, ref string errorCodes);
    }
}
