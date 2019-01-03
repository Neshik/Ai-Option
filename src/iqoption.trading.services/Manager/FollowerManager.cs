﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Queries;
using iqoption.core.Collections;
using iqoption.domain.IqOption;
using iqoption.domain.IqOption.Commands;
using iqoption.domain.IqOption.Queries;
using iqoption.domain.Users;
using IqOptionApi.Models;
using IqOptionApi.ws;
using Microsoft.Extensions.Logging;

namespace iqoption.trading.services.Manager {
    public interface IFollowerManager {
        ConcurrencyReactiveCollection<IqOptionApiClient> Followers { get; }
        Task AppendUser(IqAccount account, IObservable<InfoData> infObservable);
        void RemoveByUserId(int userId);

        Task<List<IqAccount>> GetActiveAccountNotOnFollowersTask();
    }

    public class FollowerManager : IFollowerManager {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandBus _commandBus;
        private readonly ILogger<FollowerManager> _logger;

        public ConcurrencyReactiveCollection<IqOptionApiClient> Followers { get; }

        public FollowerManager(
            IQueryProcessor queryProcessor,
            ICommandBus commandBus,
            ILogger<FollowerManager> logger) {
            _queryProcessor = queryProcessor;
            _commandBus = commandBus;
            _logger = logger;
            Followers = new ConcurrencyReactiveCollection<IqOptionApiClient>();
        }


        public async Task AppendUser(IqAccount account, IObservable<InfoData> openedPositionObservable) {
            var ct = new CancellationToken();

            //check if not existing
            if (Followers.All(x => x.Account.IqOptionUserName != account.IqOptionUserName)) {
                
                var client = new IqOptionApiClient(account);
                if (!client.Client.IsConnected) {
                    
                    //if ssid not working -re get ssid
                    var loginResult = await _commandBus.PublishAsync(
                        new IqLoginCommand(IqIdentity.New, account.IqOptionUserName, account.Password), ct);

                    if (!loginResult.IsSuccess) {
                        _logger.LogWarning(new StringBuilder($"Skipped {account.IqOptionUserName} due can't not loggin {loginResult.Message}")
                            .ToString());
                        client.Dispose();

                        await _commandBus.PublishAsync(new SetActiveAccountcommand(IqIdentity.New, new SetActiveAccountStatusItem(false, account.IqOptionUserId)), ct);
                        return;
                    }

                    await _commandBus.PublishAsync(
                        new StoreSsidCommand(IqIdentity.New, account.IqOptionUserName, loginResult.Ssid), ct);
                }

                client.SubScribeForTraderStream(openedPositionObservable);


                Followers.Add(client);


                _logger.LogInformation(new StringBuilder($"Add {account.IqOptionUserName},")
                    .AppendLine($"Now trading-followers account = {Followers.Count} Account(s).").ToString());
            }
        }

        public void RemoveByEmailAddress(string emailAddress) {
            Followers.Remove(x => x.Account.IqOptionUserName == emailAddress);

            _logger.LogInformation(new StringBuilder($"Remove {emailAddress},")
                .AppendLine($"Now trading-followers account  = {Followers.Count} Accout(s).")
                .ToString());
        }

        public void RemoveByUserId(int userId) {
            Followers.Remove(x => x.Account.IqOptionUserId == userId);
            _logger.LogInformation(new StringBuilder($"Remove userId : {userId},")
                .AppendLine($"Now trading-followers account  = {Followers.Count} Accout(s).")
                .ToString());
        }


        public Task<List<IqAccount>> GetActiveAccountNotOnFollowersTask() {
            return _queryProcessor.ProcessAsync(new ActiveAccountQuery(), CancellationToken.None)
                .ContinueWith(
                    t => t.Result.Where(x =>
                        !Followers.Select(y => y.Account.IqOptionUserName).Contains(x.IqOptionUserName)).ToList());
            
        }

        
    }
}