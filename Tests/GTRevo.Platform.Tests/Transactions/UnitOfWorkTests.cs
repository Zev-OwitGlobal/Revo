﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTRevo.Platform.Transactions;
using NSubstitute;
using NSubstitute.Core;
using Xunit;

namespace GTRevo.Platform.Tests.Transactions
{
    public class UnitOfWorkTests
    {
        private readonly List<ITransactionProvider> transactionProviders = new List<ITransactionProvider>();
        private readonly List<Tuple<ITransactionProvider, ITransaction>> transactions = new List<Tuple<ITransactionProvider, ITransaction>>();
        private readonly List<IUnitOfWorkListener> unitOfWorkListeners = new List<IUnitOfWorkListener>();
        private readonly IUnitOfWork sut;

        public UnitOfWorkTests()
        {
            CreateTransactionProvider();
            CreateTransactionProvider();

            CreateUnitOfWorkListener();
            CreateUnitOfWorkListener();

            sut = new UnitOfWork(transactionProviders.ToArray(), unitOfWorkListeners.ToArray());
        }

        [Fact]
        public void CreateTransaction_CreatesAllSubtransactions()
        {
            using (var tx = sut.CreateTransaction())
            {
                Assert.Equal(2, transactions.Count);
                Assert.Equal(transactionProviders[0], transactions[0].Item1);
                Assert.Equal(transactionProviders[1], transactions[1].Item1);
            }
        }

        [Fact]
        public async Task CreateTransaction_CommitAsync_CommitsAllSubtransactions()
        {
            using (var tx = sut.CreateTransaction())
            {
                await tx.CommitAsync();

                transactions[0].Item2.Received(1).CommitAsync();
                transactions[1].Item2.Received(1).CommitAsync();
            }
        }

        [Fact]
        public void CreateTransaction_NotifiesListeners()
        {
            using (var tx = sut.CreateTransaction())
            {
                unitOfWorkListeners[0].Received(1).OnTransactionBeginned(tx);
                unitOfWorkListeners[1].Received(1).OnTransactionBeginned(tx);
            }
        }

        [Fact]
        public async Task CreateTransaction_CommitAsync_NotifiesListeners()
        {
            using (var tx = sut.CreateTransaction())
            {
                await tx.CommitAsync();

                unitOfWorkListeners[0].Received(1).OnTransactionSucceededAsync(tx);
                unitOfWorkListeners[1].Received(1).OnTransactionSucceededAsync(tx);
            }
        }

        private ITransactionProvider CreateTransactionProvider()
        {
            var txProvider = Substitute.For<ITransactionProvider>();
            txProvider.CreateTransaction().Returns(callInfo => CreateTransaction(txProvider));

            transactionProviders.Add(txProvider);

            return txProvider;
        }

        private ITransaction CreateTransaction(ITransactionProvider transactionProvider)
        {
            var tx = Substitute.For<ITransaction>();
            transactions.Add(new Tuple<ITransactionProvider, ITransaction>(transactionProvider, tx));
            return tx;
        }

        private IUnitOfWorkListener CreateUnitOfWorkListener()
        {
            var listener = Substitute.For<IUnitOfWorkListener>();
            unitOfWorkListeners.Add(listener);
            return listener;
        }
    }
}
