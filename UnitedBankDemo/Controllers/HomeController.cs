using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UnitedBankDemo.Models;

namespace UnitedBankDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<UnitedBankConfiguration> _configuration;

        public HomeController(IOptions<UnitedBankConfiguration> configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.BaseUrl = Request.Scheme + "://" + Request.Host.Value;

            return View();
        }

        [Route("startPayment")]
        [HttpPost]
        public async Task<object> StartPayment(StartPaymentRequest model)
        {
            if (model.UseServerCredentials)
            {
                model.UserName = _configuration.Value.Username;
                model.Password = _configuration.Value.Password;
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            var response = await client.PostAsync("https://demo-ipg.ctdev.comtrust.ae:2443", 
                new StringContent(
                JsonConvert.SerializeObject(new { Registration = model }),
                Encoding.UTF8,
                "application/json"));
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();


            return Json(JsonConvert.DeserializeObject(responseBody));
        }

        [Route("finalizePayment")]
        [HttpPost]
        public async Task<object> FinalizePayment(FinalizePaymentRequest model)
        {
            if (model.UseServerCredentials)
            {
                model.UserName = _configuration.Value.Username;
                model.Password = _configuration.Value.Password;
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");


            var response = await client.PostAsync("https://demo-ipg.ctdev.comtrust.ae:2443",
                new StringContent(
                JsonConvert.SerializeObject(new { Finalization = model }),
                Encoding.UTF8,
                "application/json"));
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();


            return Json(JsonConvert.DeserializeObject(responseBody));
        }

        [HttpPost]
        public IActionResult ReturnPage(ReturnPageRequest model)
        {
            ViewBag.TransactionId = model.TransactionID;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class StartPaymentRequest
    {
        public string Customer { get; set; }
        public string Store { get; set; }
        public string Terminal { get; set; }
        public string Channel { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string OrderID { get; set; }
        public string OrderName { get; set; }
        public string OrderInfo { get; set; }
        public string TransactionHint { get; set; }
        public string ReturnPath { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseServerCredentials { get; set; }
    }

    public class StartPaymentResponse
    {
    }

    public class FinalizePaymentRequest
    {
        public string Customer { get; set; }
        public string TransactionID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool UseServerCredentials { get; set; }
    }

    public class ReturnPageRequest
    {
        public string TransactionID { get; set; }
    }

    public class UnitedBankConfiguration
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
