using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Data.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;

namespace Betsson.OnlineWallets.Tests.API_BDD_tests.Steps
{
    [Binding]
    public class WithdrawSteps
        {
        private RestClient _client;

        private RestRequest _get_balance_request;
        private RestResponse _get_balance_response;

        private static RestRequest _post_deposit_request;
        private static RestResponse _post_deposit_response;

        private static RestRequest _post_withdraw_request;
        private static RestResponse _post_withdraw_response;

        private decimal _depositAmount;

        private decimal _currentBalance;

        private readonly IOnlineWalletRepository _onlineWalletRepository;

        public WithdrawSteps()
        {
            _client = new RestClient("http://localhost:8080");
        }

        // Setup before each scenario
        [BeforeScenario]
        public static void SetUp()
        {
            RestClient _client = new RestClient("http://localhost:8080");

            _post_deposit_request = null;
            _post_deposit_response = null;

            _post_withdraw_request = null;
            _post_withdraw_response = null;

            //getbalance, if balance greater than zero, do a withdraw with same amount to achieve zero balance
            RestRequest _get_balance_request = new RestRequest("/onlinewallet/balance", Method.Get);
            RestResponse _get_balance_response = _client.Execute(_get_balance_request);
            // Deserialize the response content into a Balance object
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_get_balance_response.Content);

            if (balanceResponse.Amount > 0)
            {
                RestRequest _post_withdraw_request = new RestRequest("/onlinewallet/withdraw", Method.Post);
                _post_withdraw_request.AddJsonBody(balanceResponse);

                // Execute the request
                RestResponse _post_withdraw_response = _client.Execute(_post_withdraw_request);
            }
        }

        [AfterScenario]
        public static void TearDown()
        {
            RestClient _client = new RestClient("http://localhost:8080");

            //getbalance, if balance greater than zero, do a withdraw with same amount to achieve zero balance
            RestRequest _get_balance_request = new RestRequest("/onlinewallet/balance", Method.Get);
            RestResponse _get_balance_response = _client.Execute(_get_balance_request);
            // Deserialize the response content into a Balance object
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_get_balance_response.Content);

            if (balanceResponse.Amount > 0)
            {
                RestRequest _post_withdraw_request = new RestRequest("/onlinewallet/withdraw", Method.Post);
                _post_withdraw_request.AddJsonBody(balanceResponse);

                // Execute the request
                RestResponse _post_withdraw_response = _client.Execute(_post_withdraw_request);
            }
        }

        // Scenario: Withdrawing valid amount being smaller than balance
        [Given(@"I have a wallet with balance bigger than zero")]
        public async void GivenIHaveAWalletWithBalanceBiggerThanZero()
        {
        // Example: Deposit a valid amount of 100
        _depositAmount = 100.00m;

        // Prepare the deposit request
        var deposit = new { Amount = _depositAmount };
        _post_deposit_request = new RestRequest("/onlinewallet/deposit", Method.Post);
        _post_deposit_request.AddJsonBody(deposit);

        // Execute the request
        _post_deposit_response = await _client.ExecuteAsync(_post_deposit_request);
        }

        [When(@"I withdraw valid value")]
        public async Task WhenIWithdrawValidValue()
        {
            var withdrawalAmount = 50;

            var withdrawal = new { Amount = withdrawalAmount };
            _post_withdraw_request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            _post_withdraw_request.AddJsonBody(withdrawal);

            // Execute the request           
            do
            {
                _post_withdraw_response = await _client.ExecuteAsync(_post_withdraw_request);
            } while (_post_withdraw_response.StatusCode != HttpStatusCode.OK);
        }

        [Then(@"I get response of correct new balance bigger than zero")]
        public async void ThenIGetResponseOfCorrectNewBalance()
        {
            _get_balance_request = new RestRequest("/onlinewallet/balance", Method.Get);
            _get_balance_response = await _client.ExecuteAsync(_get_balance_request);
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_get_balance_response.Content);
            var expectedBalance = balanceResponse.Amount;  // Expected new balance after withdrawal

            Assert.Equal(HttpStatusCode.OK, _post_withdraw_response.StatusCode);

            // Deserialize the response and validate the new balance
            var withdrawResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_post_withdraw_response.Content);
                

            Assert.Equal(expectedBalance, withdrawResponse.Amount);
        }

        // Scenario: Withdrawing same amount as balance
        [When(@"I withdraw same amount as balance")]
        public async Task WhenIWithdrawSameAmountAsBalance()
        {
            _get_balance_request = new RestRequest("/onlinewallet/balance", Method.Get);
            _get_balance_response = await _client.ExecuteAsync(_get_balance_request);
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_get_balance_response.Content);

            var withdrawalAmount = balanceResponse.Amount;

            var withdrawal = new { Amount = withdrawalAmount };
            _post_withdraw_request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            _post_withdraw_request.AddJsonBody(withdrawal);

            // Execute the request
            _post_withdraw_response = await _client.ExecuteAsync(_post_withdraw_request);
        }

        [Then(@"I get response of correct new balance as zero")]
        public void ThenIGetResponseOfCorrectNewBalanceAfterFullWithdrawal()
        {
            Assert.Equal(HttpStatusCode.OK, _post_withdraw_response.StatusCode);

            // Deserialize the response and validate the new balance
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_post_withdraw_response.Content);
            var expectedBalance = 0;  // Balance must be zero after full withdrawal

            Assert.Equal(expectedBalance, balanceResponse.Amount);
        }

        // Scenario: Attempting to withdraw more than the balance
        [Given(@"I have a wallet with balance zero")]
        public void GivenIHaveAWalletWithBalanceZero()
        {
            // Initialize wallet with zero balance
            _currentBalance = 0;
        }

        [When(@"I attempt to withdraw positive value")]
        public async Task WhenIWithdrawValidValueWithZeroBalance()
        {
            // Attempt to withdraw 100 with zero balance
            var withdrawalAmount = 100;

            var withdrawal = new { Amount = withdrawalAmount };
            _post_withdraw_request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            _post_withdraw_request.AddJsonBody(withdrawal);

            // Execute the request
            _post_withdraw_response = await _client.ExecuteAsync(_post_withdraw_request);
        }

        [Then(@"I get response of no sufficient funds")]
        public void ThenIGetResponseOfNoSufficientFunds()
        {
            Assert.Equal(HttpStatusCode.BadRequest, _post_withdraw_response.StatusCode);

            // Check for specific error message
            var errorResponse = _post_withdraw_response.Content;
            Assert.True(errorResponse.Contains("Invalid withdrawal amount. There are insufficient funds."), "Expected 'no sufficient funds' message.");
        }

        // Scenario: Attempting to withdraw invalid value
        [When(@"I withdraw invalid value")]
        public async Task WhenIWithdrawInvalidValue()
        {
            // Attempt to withdraw an invalid amount (e.g., negative value)
            var invalidWithdrawalAmount = -50;

            var withdrawal = new { Amount = invalidWithdrawalAmount };
            _post_withdraw_request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            _post_withdraw_request.AddJsonBody(withdrawal);

            // Execute the request
            _post_withdraw_response = await _client.ExecuteAsync(_post_withdraw_request);
        }

        [Then(@"I get response of validation/error message")]
        public void ThenIGetResponseOfValidationErrorMessage()
        {
            Assert.Equal(HttpStatusCode.BadRequest, _post_withdraw_response.StatusCode);

            // Optionally check for the specific error message content
            var errorResponse = _post_withdraw_response.Content;
            Assert.True(errorResponse.Contains("'Amount' must be greater than or equal to '0'."), "Error message does not match expected.");
        }
    }
}
