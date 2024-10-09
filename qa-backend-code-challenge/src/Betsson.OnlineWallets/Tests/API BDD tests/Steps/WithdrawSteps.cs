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

            // Scenario: Withdrawing valid amount being smaller than balance
            [Given(@"I have a wallet with balance bigger than zero")]
            public void GivenIHaveAWalletWithBalanceBiggerThanZero()
            {
                // Assume the initial balance is 500 for the sake of the scenario
                _currentBalance = 500;
            }

            [When(@"I withdraw valid value")]
            public async Task WhenIWithdrawValidValue()
            {
                // Assume withdrawing 100 for this scenario
                var withdrawalAmount = 100;

                var withdrawal = new { Amount = withdrawalAmount };
                _request = new RestRequest("/onlinewallet/withdraw", Method.Post);
                _request.AddJsonBody(withdrawal);

                // Execute the request
                _response = await _client.ExecuteAsync(_request);
            }

            [Then(@"I get response of correct new balance")]
            public void ThenIGetResponseOfCorrectNewBalance()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);

                // Deserialize the response and validate the new balance
                var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_response.Content);
                var expectedBalance = _currentBalance - 100;  // Expected new balance after withdrawal

                Assert.Equal(expectedBalance, balanceResponse.Amount);
            }

            // Scenario: Withdrawing same amount as balance
            [When(@"I withdraw same amount as balance")]
            public async Task WhenIWithdrawSameAmountAsBalance()
            {
                // Withdraw the entire balance
                var withdrawalAmount = _currentBalance;

                var withdrawal = new { Amount = withdrawalAmount };
                _request = new RestRequest("/onlinewallet/withdraw", Method.Post);
                _request.AddJsonBody(withdrawal);

                // Execute the request
                _response = await _client.ExecuteAsync(_request);
            }

            [Then(@"I get response of correct new balance")]
            public void ThenIGetResponseOfCorrectNewBalanceAfterFullWithdrawal()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);

                // Deserialize the response and validate the new balance
                var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_response.Content);
                var expectedBalance = 0;  // Balance should be zero after full withdrawal

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
                _request = new RestRequest("/onlinewallet/withdraw", Method.Post);
                _request.AddJsonBody(withdrawal);

                // Execute the request
                _response = await _client.ExecuteAsync(_request);
            }

            [Then(@"I get response of no sufficient funds")]
            public void ThenIGetResponseOfNoSufficientFunds()
            {
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);

                // Check for specific error message
                var errorResponse = _response.Content;
                Assert.True(errorResponse.Contains("Invalid withdrawal amount. There are insufficient funds."), "Expected 'no sufficient funds' message.");
            }

            // Scenario: Attempting to withdraw invalid value
            [When(@"I withdraw invalid value")]
            public async Task WhenIWithdrawInvalidValue()
            {
                // Attempt to withdraw an invalid amount (e.g., negative value)
                var invalidWithdrawalAmount = -50;

                var withdrawal = new { Amount = invalidWithdrawalAmount };
                _request = new RestRequest("/onlinewallet/withdraw", Method.Post);
                _request.AddJsonBody(withdrawal);

                // Execute the request
                _response = await _client.ExecuteAsync(_request);
            }

            [Then(@"I get response of validation/error message")]
            public void ThenIGetResponseOfValidationErrorMessage()
            {
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);

                // Optionally check for the specific error message content
                var errorResponse = _response.Content;
                Assert.True(errorResponse.Contains("'Amount' must be greater than or equal to '0'."), "Error message does not match expected.");
            }
        }
}
