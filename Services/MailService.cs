using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public class MailService : IMailService
{
    private readonly string _apiKey;
    private readonly string _domain;

    public MailService(IConfiguration configuration)
    {
        _apiKey = configuration["Mail:ApiKey"]; // Asegúrate de tener esto en tu archivo de configuración
        _domain = configuration["Mail:Domain"];
    }

    public async Task SendEmailAsync(string to, string subject, string message, string from)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new System.Uri($"https://api.mailgun.net/v3/{_domain}/messages");
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_apiKey}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("from", from),
                new KeyValuePair<string, string>("to", to),
                new KeyValuePair<string, string>("subject", subject),
                new KeyValuePair<string, string>("html", message) // Cambia "text" a "html" para contenido HTML
            });

            var response = await client.PostAsync("", content);
            response.EnsureSuccessStatusCode();
        }
    }
 }
