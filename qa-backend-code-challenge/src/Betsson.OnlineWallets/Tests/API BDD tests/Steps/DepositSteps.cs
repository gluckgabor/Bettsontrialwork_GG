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
using Betsson.OnlineWallets.Models;


namespace Betsson.OnlineWallets.Tests.API_BDD_tests.Steps
{
    [Binding]
    public class DepositSteps
    {
        private RestClient _client;

        private static RestRequest _get_balance_request;
        private static RestResponse _get_balance_response;

        private static RestRequest _post_deposit_request;
        private static RestResponse _post_deposit_response;

        private static RestRequest _post_withdraw_request;
        private static RestResponse _post_withdraw_response;


        private decimal _currentBalance;
        private decimal _depositAmount;

        public DepositSteps()
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

        // Scenario: [Initial balance zero, valid amount deposited]
        [Given(@"I have a wallet with balance zero before deposit")]
        public void GivenIHaveAWalletWithBalanceZero()
        {
            _currentBalance = 0;
        }

        [When(@"I deposit valid value")]
        public async Task WhenIDepositValidValue()
        {
            // Example: Deposit a valid amount of 100
            _depositAmount = 100;

            // Prepare the deposit request
            var deposit = new { Amount = _depositAmount };
            _post_deposit_request = new RestRequest("/onlinewallet/deposit", Method.Post);
            _post_deposit_request.AddJsonBody(deposit);

            // Execute the request
            _post_deposit_response = await _client.ExecuteAsync(_post_deposit_request);
        }

        [Then(@"I get response of correct new balance containing deposited amount")]
        public void ThenIGetResponseOfCorrectNewBalanceScenario3()
        {
            // Ensure the response is OK
            Assert.Equal(HttpStatusCode.OK, _post_deposit_response.StatusCode);

            // Deserialize the response and validate the new balance
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_post_deposit_response.Content);
            var expectedBalance = _currentBalance + _depositAmount;

            Assert.Equal(expectedBalance, balanceResponse.Amount);
        }


        [Then(@"I get response of correct new balance")]
        public void ThenIGetResponseOfCorrectNewBalance()
        {
            // Ensure the response is OK
            Assert.Equal(HttpStatusCode.OK, _post_deposit_response.StatusCode);

            // Deserialize the response and validate the new balance
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_post_deposit_response.Content);
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
            _post_deposit_request = new RestRequest("/onlinewallet/deposit", Method.Post);
            _post_deposit_request.AddJsonBody(deposit);

            // Execute the request
            _post_deposit_response = await _client.ExecuteAsync(_post_deposit_request);
        }

        [Then(@"I get a response of validation/error message")]
        public void ThenIGetResponseOfValidationErrorMessage()
        {
            // Ensure the response is a validation or bad request error
            Assert.Equal(HttpStatusCode.BadRequest, _post_deposit_response.StatusCode);

            // Optionally, check the error message content
            var errorResponse = _post_deposit_response.Content;
            Assert.True(errorResponse.Contains("'Amount' must be greater than or equal to '0'."), "Error message does not match expected.");
        }



        // Scenario: [Initial value bigger than zero, valid amount deposited]
        [Given(@"I query the balance")]
        public async Task WhenIQueryTheBalance()
        {
            _get_balance_request = new RestRequest("/onlinewallet/balance", Method.Get);
            _get_balance_response = await _client.ExecuteAsync(_get_balance_request);
        }

        [Given(@"I deposit valid value to have bigger then zero balance")]
        public async Task WhenIDepositValidValueToHaveBiggerThanZeroBalance()
        {
            // Example: Deposit a valid amount of 100
            _depositAmount = 100;

            // Prepare the deposit request
            var deposit = new { Amount = _depositAmount };
            _post_deposit_request = new RestRequest("/onlinewallet/deposit", Method.Post);
            _post_deposit_request.AddJsonBody(deposit);

            // Execute the request
            _post_deposit_response = await _client.ExecuteAsync(_post_deposit_request);
        }

        [Given(@"I have a wallet with balance bigger then zero before second deposit")]
        public void GivenIHaveAWalletWithBalanceBiggerThanZero()
        {
            // Deserialize the response content into a Balance object
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_post_deposit_response.Content);

            // Setting an initial positive balance
            _currentBalance = balanceResponse.Amount;  // Example: Wallet starts with 200
        }        
    }
}