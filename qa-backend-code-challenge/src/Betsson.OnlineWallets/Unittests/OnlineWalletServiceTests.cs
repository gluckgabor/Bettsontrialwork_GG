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

        // Test case 1: Repository returns a valid OnlineWalletEntry
        [Fact]
        public async Task GetBalanceAsync_WhenLastOnlineWalletEntryExists_ReturnsCorrectBalance()
        {            
            var onlineWalletEntry = new OnlineWalletEntry
            {
                BalanceBefore = 0,
                Amount = 0
            };
            _mockRepository
                .Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(onlineWalletEntry);

           
        }

    }
}
