using System;
using Libplanet.RocksDBStore;
using Libplanet.Store;

namespace NineChronicles.Headless.Executable.Store
{
    public static class StoreTypeExtensions
    {
        public static IStore CreateStore(this StoreType storeType, string storePath) => storeType switch
        {
            StoreType.MonoRocksDb => new MonoRocksDBStore(storePath),
            StoreType.RocksDb => new RocksDBStore(storePath),
            StoreType.Default => new DefaultStore(storePath),
            _ => throw new ArgumentOutOfRangeException(nameof(storeType))
        };
    }
}
