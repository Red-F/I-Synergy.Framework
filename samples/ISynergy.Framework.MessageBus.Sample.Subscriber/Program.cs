﻿using System;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.MessageBus.Extensions;
using ISynergy.Framework.MessageBus.Sample.Models;
using ISynergy.Framework.MessageBus.Sample.Subscriber.Options;
using ISynergy.Framework.MessageBus.Sample.Subscriber.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.MessageBus.Sample.Subscriber
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

                var serviceProvider = new ServiceCollection()
                    .AddLogging()
                    .AddOptions()
                    .Configure<TestQueueOptions>(config.GetSection(nameof(TestQueueOptions)).BindWithReload)
                    .AddSubscribingQueueMessageBus<TestDataModel, SubscribeToQueueMessageBusService>()
                    .AddScoped<Startup>()
                    .BuildServiceProvider();

                var application = serviceProvider.GetRequiredService<Startup>();
                application.RunAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }

            return 0;
        }
    }
}
