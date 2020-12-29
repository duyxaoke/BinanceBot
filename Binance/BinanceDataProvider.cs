using Binance.Hubs;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Spot;
using Binance.Net.Objects.Spot.MarketStream;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Sockets;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Binance
{
    public interface IBinanceDataProvider
    {
        IBinanceStreamKlineData LastKline { get; }
        Action<IBinanceStreamKlineData> OnKlineData { get; set; }

        Task Start();
        Task Stop();
    }

    public class BinanceDataProvider: IBinanceDataProvider
    {
        private IBinanceSocketClient _socketClient;
        private IBinanceClient _binanceClient;
        private UpdateSubscription _subscription;
        private BinanceClient _client;
        private IHubContext<ChatHub> _hub;

        public IBinanceStreamKlineData LastKline { get; private set; }
        public Action<IBinanceStreamKlineData> OnKlineData { get; set; }
       
        public BinanceDataProvider(IBinanceSocketClient socketClient, IBinanceClient binanceClient, IHubContext<ChatHub> hub)
        {
            _hub = hub;
            _socketClient = socketClient;
            _binanceClient = binanceClient;
            _client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials("24ueXUeZ1umXlbsklA4rJCv3yQUXOGq72JuTtrMaYUcNoMPw6f8c6Gry747LfbR2", "6iqu9YrXDBQUjsAuwBvcos2r4T0Bns4PW3RVEVP8EM0yT7ifTMwYDeK5cHx2Dvte")
            });

            Start().Wait(); 
        }

        public async Task Start()
        {
            var startResult = _client.Spot.UserStream.StartUserStream();

            //var order = _client.FuturesUsdt.Order.GetAllOrders("XRPUSDT");

            var subResult = await _socketClient.Spot.SubscribeToKlineUpdatesAsync("BTCUSDT", KlineInterval.FifteenMinutes, data =>
            {
                LastKline = data;
                //_hub.Clients.All.SendAsync("ReceiveMessage", "Duy", data?.Data.Close.ToString() ?? "not known yet");
                OnKlineData?.Invoke(data);
            });
            if (subResult.Success)            
                _subscription = subResult.Data;            
        }

        public async Task Stop()
        {
            await _socketClient.Unsubscribe(_subscription);
        }
    }
}
