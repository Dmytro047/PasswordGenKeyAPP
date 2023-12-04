using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PasswordGenKeyAPP.Pages
{
    public class IndexModel : PageModel
    {
        public static string Password { get; private set; }
        private static string secretValue { get; set; }

        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task OnGetAsync()
        {
            await GetPasswordGeneratorCode();
        }

        public async Task GetPasswordGeneratorCode()
        {
            var url = _configuration.GetValue<string>("KeyVaultUrl");
            var keyVaultUri = new Uri(url);

            var secretClient = new SecretClient(keyVaultUri, new DefaultAzureCredential());


            var secretName = _configuration.GetValue<string>("KeyName");
            var secretResponse = await secretClient.GetSecretAsync(secretName);
            secretValue = secretResponse.Value.Value;
        }

        public async Task OnPostAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var url = _configuration.GetValue<string>("PasswordFunctionUrl");
                var functionUrl = $"{url}?code={secretValue}";
                var response = await httpClient.PostAsync(functionUrl, null);
                IndexModel.Password = await response.Content.ReadAsStringAsync();
            }
        }
    }
}
