using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Simucraft.Client.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simucraft.Client.Services
{
    public interface IGameHubBuilder
    {
        IGameHubBuilder On<T>(string channel, Action<T> handler);
        Task StartAsync(Guid gameId);
    }

    public class GameHubService : IDisposable, IGameHubBuilder
    {
        private const string CONNECT_TO_GAME = "ConnectToGame";

        private readonly NavigationManager _navigationManager;
        private readonly IAccessTokenProvider _accessTokenProvider;

        private HubConnection _hubConnection;
        private Guid _gameId;

        private IList<Action> _events = new List<Action>();

        public GameHubService(
            NavigationManager navigationManager,
            IAccessTokenProvider accessTokenProvider)
        {
            _navigationManager = navigationManager;
            _accessTokenProvider = accessTokenProvider;
        }

        public event Func<Task> Disconnected;
        public event Func<Task> Connected;

        public async Task StartAsync(Guid gameId)
        {
            _gameId = gameId;

            _hubConnection = new HubConnectionBuilder()
                .WithAutomaticReconnect(
                    new TimeSpan[]
                    {
                        TimeSpan.FromMilliseconds(500),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(10),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(1),
                        TimeSpan.FromMinutes(5),
                        TimeSpan.FromMinutes(5),
                        TimeSpan.FromMinutes(5),
                        TimeSpan.FromMinutes(10),
                        TimeSpan.FromMinutes(10),
                        TimeSpan.FromMinutes(10),
                    })
                .WithUrl(_navigationManager.ToAbsoluteUri("/gameHub"), o =>
                {
                    o.AccessTokenProvider = async () =>
                    {
                        var tokenResult = await _accessTokenProvider.RequestAccessToken();
                        if (tokenResult.TryGetToken(out var token))
                            return token.Value;

                        return null;
                    };
                    o.Headers.Add("GameId", _gameId.ToString());
                })
                .Build();

            _hubConnection.Closed += async e =>
            {
                if (this.Disconnected != null)
                    await this.Disconnected.Invoke();
            };
            _hubConnection.Reconnected += async e =>
            {
                await _hubConnection.SendAsync(CONNECT_TO_GAME, _gameId);

                if (this.Connected != null)
                    await this.Connected.Invoke();
            };

            foreach (var @event in _events)
                @event.Invoke();

            await _hubConnection.StartAsync();

            if(this.Connected != null)
                await this.Connected.Invoke();

            //await _hubConnection.SendAsync(CONNECT_TO_GAME);
        }

        public async Task SendAsync<T>(string name, T message)
        {
            await _hubConnection.SendAsync(name, message);
        }

        public IGameHubBuilder On<T>(string channel, Action<T> handler)
        {
            _events.Add(() => _hubConnection.On<T>(channel, handler));

            return this;
        }

        public async void Dispose()
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
