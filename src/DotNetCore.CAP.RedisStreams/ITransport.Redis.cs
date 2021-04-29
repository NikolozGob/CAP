﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using DotNetCore.CAP.Messages;
using DotNetCore.CAP.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DotNetCore.CAP.RedisStreams
{
    class RedisTransport : ITransport
    {
        private readonly IRedisStreamManager redis;
        private readonly ILogger<RedisTransport> logger;
        private readonly MethodMatcherCache selector;
        private readonly CapRedisOptions options;

        public RedisTransport(IRedisStreamManager redis, MethodMatcherCache selector, IOptions<CapRedisOptions> options, ILogger<RedisTransport> logger)
        {
            this.redis = redis;
            this.selector = selector;
            this.options = options.Value;
            this.logger = logger;
        }

        public BrokerAddress BrokerAddress => new BrokerAddress("redis", options.Endpoint);

        public async Task<OperateResult> SendAsync(TransportMessage message)
        {
            try
            {
                await redis.PublishAsync(message.GetName(), message.AsStreamEntries());

                logger.LogDebug($"Redis message [{message.GetName()}] has been published.");

                return OperateResult.Success;
            }
            catch (Exception ex)
            {
                var wrapperEx = new PublisherSentFailedException(ex.Message, ex);

                return OperateResult.Failed(wrapperEx);
            }
        }
    }
}