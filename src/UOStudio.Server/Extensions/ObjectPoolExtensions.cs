using System;
using CSharpFunctionalExtensions;
using LiteNetLib.Utils;
using Microsoft.Extensions.ObjectPool;

namespace UOStudio.Server.Extensions
{
    public static class ObjectPoolExtensions
    {
        public static void AcquireAndRelease<T>(this ObjectPool<NetDataWriter> pool, Result<T> result, Action<Result<T>, NetDataWriter> writerAction)
        {
            var writer = pool.Get();
            writer.Reset();
            writerAction(result, writer);
            pool.Return(writer);
        }

        public static void AcquireAndRelease(this ObjectPool<NetDataWriter> pool, Result result, Action<Result, NetDataWriter> writerAction)
        {
            var writer = pool.Get();
            writer.Reset();
            writerAction(result, writer);
            pool.Return(writer);
        }
    }
}
