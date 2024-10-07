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

namespace Betsson.OnlineWallets.Tests.API_BDD_tests.Steps
{

[Binding]
    public class WithdrawSteps
    {
        private RestClient _client;
        private RestRequest _request;
        private RestResponse _response;
        private decimal _currentBalance;

        public WithdrawSteps()
        {
            _client = new RestClient("http://localhost:8080");
        }

        // Scenario: [scenario name1]
        [Given(@"I have a wallet with balance zero")]
        public void GivenIHaveAWalletWithBalanceZero()
        {
            // Initialize the wallet with zero balance for this scenario
            _currentBalance = 0;
        }

        [When(@"I withdraw valid value")]
        public async Task WhenIWithdrawValidValue()
        {
            // Example: Attempt to withdraw 100, which is more than the current balance (0)
            var withdrawalAmount = 100;

            // Prepare the withdrawal request
            var withdrawal = new { Amount = withdrawalAmount };
            _request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            _request.AddJsonBody(withdrawal);

            // Execute the request
            _response = await _client.ExecuteAsync(_request);
        }

        [Then(@"I get response of ""no sufficient funds""")]
        public void ThenIGetResponseOfNoSufficientFunds()
        {
            // Ensure the response status indicates a problem (like BadRequest or custom error code)
            Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);

            // Optionally check the response content for the specific error message
            var errorResponse = _response.Content;
            Assert.True(errorResponse.Contains("no sufficient funds"), "Expected 'no sufficient funds' message.");
        }

        // Scenario: [scenario name2]
        [When(@"I withdraw invalid value")]
        public async Task WhenIWithdrawInvalidValue()
        {
            // Example: Withdraw an invalid amount (e.g., negative value)
            var invalidWithdrawalAmount = -50;

            // Prepare the withdrawal request
            var withdrawal = new { Amount = invalidWithdrawalAmount };
            _request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            _request.AddJsonBody(withdrawal);

            // Execute the request
            _response = await _client.ExecuteAsync(_request);
        }

        [Then(@"I get response of validation/error message")]
        public void ThenIGetResponseOfValidationErrorMessage()
        {
            // Ensure the response is a validation error or bad request
            Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);

            // Optionally check for the specific error message content
            var errorResponse = _response.Content;
            Assert.True(errorResponse.Contains("Invalid withdrawal amount"), "Expected 'Invalid withdrawal amount' message.");
        }

        // Scenario: [scenario name3]
        [Given(@"I have a wallet with balance bigger than zero")]
        public void GivenIHaveAWalletWithBalanceBiggerThanZero()
        {
            // Set the initial wallet balance to a value greater than zero
            _currentBalance = 500;  // Example: Wallet starts with 500
        }

        [When(@"I withdraw valid value")]
        public async Task WhenIWithdrawValidValueScenario3()
        {
            // Example: Withdraw a valid amount of 100
            var withdrawalAmount = 100;

            // Prepare the withdrawal request
            var withdrawal = new { Amount = withdrawalAmount };
            _request = new RestRequest("/onlinewallet/withdraw", Method.Post);
            _request.AddJsonBody(withdrawal);

            // Execute the request
            _response = await _client.ExecuteAsync(_request);
        }

        [Then(@"I get response of correct new balance")]
        public void ThenIGetResponseOfCorrectNewBalance()
        {
            // Ensure the response is OK
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);

            // Deserialize the response and validate the new balance
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_response.Content);
            var expectedBalance = _currentBalance - 100; // Expected new balance after withdrawal

            Assert.Equal(expectedBalance, balanceResponse.Amount);
        }
    }
}


