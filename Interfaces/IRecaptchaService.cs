public interface IRecaptchaService
{
    public Task<bool> VerifyRecaptchaAsync(string recaptchaToken);
}