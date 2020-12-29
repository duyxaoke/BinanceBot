using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Binance.Models;
using Binance.Net.Interfaces;
using Binance.Net.Enums;
using Binance.Net;
using Binance.Net.Objects.Spot;
using CryptoExchange.Net.Authentication;
using Serverless.FunctionApp;

namespace Binance.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBinanceClient _binanceClient;
        private readonly IBinanceDataProvider _dataProvider;

        public HomeController(ILogger<HomeController> logger, IBinanceClient client, IBinanceDataProvider dataProvider)
        {
            _logger = logger;
            _binanceClient = client;
            _dataProvider = dataProvider;
        }

        public IActionResult Index()
        {
            var client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials("24ueXUeZ1umXlbsklA4rJCv3yQUXOGq72JuTtrMaYUcNoMPw6f8c6Gry747LfbR2", "6iqu9YrXDBQUjsAuwBvcos2r4T0Bns4PW3RVEVP8EM0yT7ifTMwYDeK5cHx2Dvte")
            });
            var startResult = client.Spot.UserStream.StartUserStream();

            var order = client.FuturesUsdt.Order.GetAllOrders("XRPUSDT");
            ViewBag.Order = order.Data;
            return View(_dataProvider.LastKline);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
