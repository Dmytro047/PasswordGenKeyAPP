using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PasswordGenKeyAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PasswordGenKeyAPP.Pages
{
    public class IndexModel : PageModel
    {
        //public static string Password { get; private set; }
        private static string secretValue { get; set; }

        private readonly IConfiguration _configuration;
        private readonly DBContext _dBContext;
        [BindProperty]
        public static UserEntity User { get; private set; }

        public static string ErrorMSG { get; private set; }

        public IndexModel(IConfiguration configuration, DBContext dBContext)
        {
            _configuration = configuration;
            _dBContext = dBContext;
        }

        public async Task OnGetAsync()
        {
            //await GetPasswordGeneratorCode();
            if (!string.IsNullOrEmpty(ErrorMSG)) { return; }

            User = new UserEntity();
            if (string.IsNullOrEmpty(secretValue))
            {
                await GetPasswordGeneratorCode();
            }
            await GetPassword();
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

        public async Task GetPassword()
        {
            using (var httpClient = new HttpClient())
            {
                var url = _configuration.GetValue<string>("PasswordFunctionUrl");
                var functionUrl = $"{url}?code={secretValue}";
                var response = await httpClient.PostAsync(functionUrl, null);
                User.Password = await response.Content.ReadAsStringAsync();
            }
        }

        public IActionResult OnPost() 
        {
            User.UserName = Request.Form["UserName"];
            if(string.IsNullOrEmpty(User.UserName))
            {
                ErrorMSG = "Por favor ingrese su nombre";
            }
            else
            {
                _dBContext.Users.Add(User);
                _dBContext.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
