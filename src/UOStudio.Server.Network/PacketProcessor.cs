using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using UOStudio.Server.Network.PacketHandlers;

namespace UOStudio.Server.Network
{
    public sealed class PacketProcessor : IPacketProcessor
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        public PacketProcessor(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger.ForContext<PacketProcessor>();
            _serviceProvider = serviceProvider;
        }

        public Task<Result> Process<TPacket>(IPacket packet)
            where TPacket : IPacket =>
            HandleActionInternal(typeof(IPacketHandler<TPacket>), packet);

        public Task<Result<TResult>> Process<TPacket, TResult>(IPacket packet)
            where TPacket : IPacket =>
            HandleActionInternal(typeof(IPacketHandler<TPacket, TResult>), packet);

        private dynamic HandleActionInternal<TAction>(Type handlerType, TAction action)
            where TAction : notnull
        {
            var actionName = action.GetType().Name;
            IEnumerable<dynamic> handlers = _serviceProvider.GetServices(handlerType).ToImmutableArray();

            if (!handlers.Any())
            {
                _logger.Error("Could not instantiate handler for type {HandlerType} and action {ActionName}", handlerType, actionName);
                throw new HandlerNotFoundException(handlerType);
            }

            if (handlers.Count() > 1)
            {
                var handlerNames = handlers.Select(x => ((Type)x.GetType()).Name);
                _logger.Error("More than one handler registered for action {ActionName}, found handlers: {HandlerNames}", actionName, handlerNames);
                throw new AmbiguousHandlerException(actionName);
            }

            return handlers.First().Handle((dynamic)action);
        }
    }
}
