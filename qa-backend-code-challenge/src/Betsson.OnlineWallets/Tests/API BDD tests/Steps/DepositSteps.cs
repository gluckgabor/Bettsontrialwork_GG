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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Betsson.OnlineWallets.Tests.API_BDD_tests.Steps
{
    internal class DepositSteps
    {
    }

    [Binding]
    public class DepositFundsAsync
    {
        private RestClient _client;
        private RestRequest _request;
        private RestResponse _response;
        private decimal _currentBalance;
        private decimal _depositAmount;

        public DepositFundsAsync()
        {
            _client = new RestClient("http://localhost:8080");
        }

        // Scenario: [Initial balance zero, valid amount deposited]
        [Given(@"I have a wallet with balance zero before deposit")]
        public void GivenIHaveAWalletWithBalanceZero()
        {
            // Setting initial balance to 0 for this scenario
            _currentBalance = 0;
        }

        [When(@"I deposit valid value")]
        public async Task WhenIDepositValidValue()
        {
            // Example: Deposit a valid amount of 100
            _depositAmount = 100;

            // Prepare the deposit request
            var deposit = new { Amount = _depositAmount };
            _request = new RestRequest("/onlinewallet/deposit", Method.Post);
            _request.AddJsonBody(deposit);

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
            var expectedBalance = _currentBalance + _depositAmount;

            Assert.Equal(expectedBalance, balanceResponse.Amount);
        }

        // Scenario: [Invalid value attempted to be deposited]
        [When(@"I deposit invalid value")]
        public async Task WhenIDepositInvalidValue()
        {
            // Example: Deposit an invalid amount (negative value)
            _depositAmount = -50;  // Invalid amount

            // Prepare the deposit request
            var deposit = new { Amount = _depositAmount };
            _request = new RestRequest("/onlinewallet/deposit", Method.Post);
            _request.AddJsonBody(deposit);

            // Execute the request
            _response = await _client.ExecuteAsync(_request);
        }

        [Then(@"I get response of validation/error message")]
        public void ThenIGetResponseOfValidationErrorMessage()
        {
            // Ensure the response is a validation or bad request error
            Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);

            // Optionally, check the error message content
            var errorResponse = _response.Content;
            Assert.True(errorResponse.Contains("Invalid deposit amount"), "Error message does not match expected.");
        }

        // Scenario: [Initial value bigger than zero, valid amount deposited]
        [Given(@"I have a wallet with balance bigger than zero before deposit")]
        public void GivenIHaveAWalletWithBalanceBiggerThanZero()
        {
            // Setting an initial positive balance
            _currentBalance = 200;  // Example: Wallet starts with 200
        }

        //[When(@"I deposit valid value")]
        //public async Task WhenIDepositValidValueScenario3()
        //{
        //    // Example: Deposit a valid amount of 50
        //    _depositAmount = 50;

        //    // Prepare the deposit request
        //    var deposit = new { Amount = _depositAmount };
        //    _request = new RestRequest("/onlinewallet/deposit", Method.Post);
        //    _request.AddJsonBody(deposit);

        //    // Execute the request
        //    _response = await _client.ExecuteAsync(_request);
        //}

        [Then(@"I get response of correct new balance containing deposited amount")]
        public void ThenIGetResponseOfCorrectNewBalanceScenario3()
        {
            // Ensure the response is OK
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);

            // Deserialize the response and validate the new balance
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_response.Content);
            var expectedBalance = _currentBalance + _depositAmount;

            Assert.Equal(expectedBalance, balanceResponse.Amount);
        }
    }
}