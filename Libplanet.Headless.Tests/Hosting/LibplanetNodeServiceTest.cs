using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.Loader;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Headless.Hosting;
using Libplanet.Action.State;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Nekoyume.Action.Loader;
using Nekoyume.Blockchain.Policy;
using Xunit;

namespace Libplanet.Headless.Tests.Hosting
{
    public class LibplanetNodeServiceTest
    {
        [Fact]
        public void TempTest()
        {
            var stagePolicy = new VolatileStagePolicy();
            var actionLoader = new NCActionLoader();
            var policy = new BlockPolicySource(actionLoader).GetPolicy();
            var service = new LibplanetNodeService(
                new LibplanetNodeServiceProperties()
                {
                    AppProtocolVersion = new AppProtocolVersion(),
                    GenesisBlockPath = "C:\\Users\\lime_\\planetarium\\local-test\\genesis-block",
                    SwarmPrivateKey = new PrivateKey(),
                    StoreStatesCacheSize = 2,
                    StorePath = "C:\\Users\\lime_\\planetarium\\local-test\\pbft-store\\pbft-33000",
                    Host = IPAddress.Loopback.ToString(),
                    IceServers = new List<IceServer>(),
                },
                blockPolicy: policy,
                stagePolicy: stagePolicy,
                renderers: null,
                preloadProgress: null,
                exceptionHandlerAction: (code, msg) => throw new Exception($"{code}, {msg}"),
                preloadStatusHandlerAction: isPreloadStart => { },
                actionLoader: actionLoader
            );

            BlockChain blockChain = service.BlockChain;
            Assert.NotNull(blockChain);

            Assert.NotNull(service);
        }
        
        [Fact]
        public void Constructor()
        {
            var policy = new BlockPolicy();
            var stagePolicy = new VolatileStagePolicy();
            var stateStore = new TrieStateStore(new MemoryKeyValueStore());
            var blockChainStates = new BlockChainStates(
                new MemoryStore(),
                stateStore);
            var actionLoader = new SingleActionLoader(typeof(DummyAction));
            var genesisBlock = BlockChain.ProposeGenesisBlock();
            var service = new LibplanetNodeService(
                new LibplanetNodeServiceProperties()
                {
                    AppProtocolVersion = new AppProtocolVersion(),
                    GenesisBlock = genesisBlock,
                    SwarmPrivateKey = new PrivateKey(),
                    StoreStatesCacheSize = 2,
                    StorePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()),
                    Host = IPAddress.Loopback.ToString(),
                    IceServers = new List<IceServer>(),
                },
                blockPolicy: policy,
                stagePolicy: stagePolicy,
                renderers: null,
                preloadProgress: null,
                exceptionHandlerAction: (code, msg) => throw new Exception($"{code}, {msg}"),
                preloadStatusHandlerAction: isPreloadStart => { },
                actionLoader: actionLoader
            );

            Assert.NotNull(service);
        }

        [Fact]
        public void PropertiesMustContainGenesisBlockOrPath()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                IActionLoader actionLoader = new SingleActionLoader(typeof(DummyAction));
                var service = new LibplanetNodeService(
                    new LibplanetNodeServiceProperties()
                    {
                        AppProtocolVersion = new AppProtocolVersion(),
                        SwarmPrivateKey = new PrivateKey(),
                        ConsensusPrivateKey = new PrivateKey(),
                        StoreStatesCacheSize = 2,
                        Host = IPAddress.Loopback.ToString(),
                        IceServers = new List<IceServer>(),
                    },
                    blockPolicy: new BlockPolicy(),
                    stagePolicy: new VolatileStagePolicy(),
                    renderers: null,
                    preloadProgress: null,
                    exceptionHandlerAction: (code, msg) => throw new Exception($"{code}, {msg}"),
                    preloadStatusHandlerAction: isPreloadStart => { },
                    actionLoader: actionLoader
                );
            });
        }

        private class DummyAction : IAction
        {
            IValue IAction.PlainValue => Dictionary.Empty;

            IWorld IAction.Execute(IActionContext context)
            {
                return context.PreviousState;
            }

            void IAction.LoadPlainValue(IValue plainValue)
            {
            }
        }
    }
}
