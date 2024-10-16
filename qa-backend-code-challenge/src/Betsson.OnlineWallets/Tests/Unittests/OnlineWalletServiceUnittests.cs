﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Betsson.OnlineWallets.Services;
using Moq;
using Xunit;
using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Exceptions;
using Betsson.OnlineWallets.Models;
using System.Runtime.CompilerServices;
using FluentAssertions;
using static System.Collections.Specialized.BitVector32;

namespace Betsson.OnlineWallets.Tests.Unittests
{
    public class OnlineWalletServiceUnittests

    {
        private readonly Mock<IOnlineWalletRepository> _mockRepository;
        private readonly OnlineWalletService _walletService;
        private readonly Deposit deposit;
        private readonly Withdrawal withdrawalAmount;

        public OnlineWalletServiceUnittests()
        {
            // Initialize Moq repository and the service to be tested
            _mockRepository = new Mock<IOnlineWalletRepository>();
            _walletService = new OnlineWalletService(_mockRepository.Object);
            deposit = new Deposit();
            withdrawalAmount = new Withdrawal();
        }

        // Test case 1: Repository returns a valid OnlineWalletEntry when balance is zero
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 0,
                Amount = 0
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(0, result.Amount);
        }

        // Test case 2: Repository returns a valid OnlineWalletEntry when BalanceBefore is bigger than zero
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_BalanceBeforeBiggerThanZero()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 10,
                Amount = 0
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(10, result.Amount);
        }

        // Test case 3: Repository returns a valid OnlineWalletEntry when Amount is bigger than zero
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_AmountIsBiggerThanZero()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 0,
                Amount = 9
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(9, result.Amount);
        }

        // Test case 4: Repository returns a valid OnlineWalletEntry when BalanceBefore and Amount are bigger than zero
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_BalanceBeforeAndAmountAreBiggerThanZero()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 3,
                Amount = 9
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(12, result.Amount);
        }

        // Test case 5: negative numbers
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_NegativeNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = -1,
                Amount = -9
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(-10, result.Amount);
        }

        // Test case 6: leading zero in decimal value
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_NegativeNumbers_LeadingZero()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = -01,
                Amount = -09
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(-10, result.Amount);
        }

        // Test case 6: fractions
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_Fractions()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 1.11m,
                Amount = 3.12m
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(4.23m, result.Amount);
        }

        // Test case 6: huge numbers
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_HugeNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 100000000000000.01m,
                Amount = 100000000000000.01m
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(200000000000000.02m, result.Amount);
        }

        // Test case 6: tiny numbers
        [Fact]
        public async Task GetBalance_ReturnsCorrectInitialBalance_TinyNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 0.0000000000000001m,
                Amount = 0.0000000000000001m
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(0.0000000000000002m, result.Amount);
        }


        // Test case 6: data driven test
        [Theory]
        [InlineData(-2, 7, 5)]
        [InlineData(-1100, 1105, 5)]
        [InlineData(123433.41, -123428.41, 5)]
        public async Task GetBalance_ReturnsCorrectInitialBalance_DifferentCombinationsOfAdditions(int a, int b, int expected)
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = a,
                Amount = b
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.GetBalanceAsync();

            //Assert
            Assert.Equal(expected, result.Amount);
        }


        // Test case 7: 
        [Fact]
        public async Task DepositFunds_wholeNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 1,
                EventTime = DateTimeOffset.UtcNow
            };

            Deposit deposit = new Deposit
            {
                Amount = 2
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.DepositFundsAsync(deposit);

            //Assert
            Assert.Equal(3, result.Amount);
        }


        // Test case 8: 
        [Fact]
        public async Task DepositFunds_positiveAmountDepositedWhenBalanceIsZero()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 0,
                EventTime = DateTimeOffset.UtcNow
            };

            Deposit deposit = new Deposit
            {
                Amount = 2
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.DepositFundsAsync(deposit);

            //Assert
            Assert.Equal(2, result.Amount);
        }


        // Test case 8: 
        [Fact]
        public async Task DepositFunds_positiveAmountDepositedWhenBalanceIsNegative()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = -10,
                EventTime = DateTimeOffset.UtcNow
            };

            Deposit deposit = new Deposit
            {
                Amount = 2
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.DepositFundsAsync(deposit);

            //Assert
            Assert.Equal(-8, result.Amount);
        }


        // Test case 9: 
        [Fact]
        public async Task DepositFunds_BigNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 999999,
                EventTime = DateTimeOffset.UtcNow
            };

            Deposit deposit = new Deposit
            {
                Amount = 1
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.DepositFundsAsync(deposit);

            //Assert
            Assert.Equal(1000000, result.Amount);
        }


        // Test case 9: 
        [Fact]
        public async Task DepositFunds_FractionalNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 0.999999m,
                EventTime = DateTimeOffset.UtcNow
            };

            Deposit deposit = new Deposit
            {
                Amount = 0.000002m
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.DepositFundsAsync(deposit);

            //Assert
            Assert.Equal(1.000001m, result.Amount);
        }


        // Test case 10: 
        [Fact]
        public async Task WithdrawFunds_AmountSmallerThanBalance_WholeNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 1000
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = 1
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            Assert.Equal(999, result.Amount);
        }


        // Test case 11: 
        [Fact]
        public async Task WithdrawFunds_AmountSmallerThanBalance_FractionalNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 1.001m
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = 0.002m
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Balance result = await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            Assert.Equal(0.999m, result.Amount);
        }


        // Test case 11: 
        [Fact]
        public async Task WithdrawFunds_AmountSmallerThanBalance_NegativeAmount()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 1.1m
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = -0.1m
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            var result = await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            Assert.Equal(1.2m, result.Amount);
        }


        // Test case 12: 
        [Fact]
        public async Task WithdrawFunds_AmountSmallerThanBalance_NegativeBalance_InvalidScenario()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = -1
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = -2
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            var result = await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            Assert.Equal(1, result.Amount);
        }


        // Test case 13: 
        [Fact]
        public async Task WithdrawFunds_AmountSmallerThanBalance_NegativeBalance_FractionalNumber_InvalidScenario()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = -1
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = -1.01m
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            var result = await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            Assert.Equal(0.01m, result.Amount);
        }


        // Test case 14: 
        [Fact]
        public async Task WithdrawFunds_AmountBiggerThanBalance_WholeNumber()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 1000
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = 2000
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Func<Task> action = async () => await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            await action.Should().ThrowExactlyAsync<InsufficientBalanceException>()
            .WithMessage("Invalid withdrawal amount. There are insufficient funds.");
        }


        // Test case 15: 
        [Fact]
        public async Task WithdrawFunds_AmountBiggerThanBalance_FractionalNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 1000.001m
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = 1000.0011m
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Func<Task> action = async () => await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            await action.Should().ThrowExactlyAsync<InsufficientBalanceException>()
            .WithMessage("Invalid withdrawal amount. There are insufficient funds.");
        }


        // Test case 15: 
        [Fact]
        public async Task WithdrawFunds_BalanceZero_FractionalNumbers()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 0
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = 0.1m
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            Func<Task> action = async () => await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            await action.Should().ThrowExactlyAsync<InsufficientBalanceException>()
            .WithMessage("Invalid withdrawal amount. There are insufficient funds.");
        }


        // Test case 15: 
        [Fact]
        public async Task WithdrawFunds_BalanceAndAmountToBeWithdrawedAreTheSame()
        {
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 100.01m
            };

            Withdrawal withdrawalAmount = new Withdrawal
            {
                Amount = 100.01m
            };

            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

            //Call method
            var result = await _walletService.WithdrawFundsAsync(withdrawalAmount);

            //Assert
            Assert.Equal(0, result.Amount);
        }

    }
}
