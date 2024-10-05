using System;
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

namespace Betsson.OnlineWallets.Services
{
    public class OnlineWalletServiceTests

    {
        private readonly Mock<IOnlineWalletRepository> _mockRepository;
        private readonly OnlineWalletService _walletService;

        public OnlineWalletServiceTests()
        {
            // Initialize Moq repository and the service to be tested
            _mockRepository = new Mock<IOnlineWalletRepository>();
            _walletService = new OnlineWalletService(_mockRepository.Object);
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


    }
}
