using RestSharp;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Betsson.OnlineWallets.Tests.API_BDD_tests.Steps
{
    [Binding]
    public class GetBalanceSteps
    {
        private RestClient _client;
        private RestRequest _request;
        private RestResponse _response;
        private decimal _expectedBalance;

        private decimal _depositAmount;

        private static RestRequest _get_balance_request;
        private static RestResponse _get_balance_response;

        private static RestRequest _post_deposit_request;
        private static RestResponse _post_deposit_response;

        private static RestRequest _post_withdraw_request;
        private static RestResponse _post_withdraw_response;

        public GetBalanceSteps()
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

        //Scenario: [Get initial zero balance]
        [Given(@"I have a wallet with balance zero before querying balance")]
        public void GivenIHaveAWalletWithBalanceZero()
        {
            // Set expected balance to zero for this scenario
            _expectedBalance = 0;
        }

        [When(@"I query the balance")]
        public async Task WhenIQueryTheBalance()
        {
            _get_balance_request = new RestRequest("/onlinewallet/balance", Method.Get);
            _get_balance_response = await _client.ExecuteAsync(_get_balance_request);
        }

        [Then(@"I get response of correct balance")]
        public void ThenIGetResponseOfCorrectBalance()
        {
            // Check if the response is OK
            Assert.Equal(HttpStatusCode.OK, _get_balance_response.StatusCode);

            // Deserialize the response content into a Balance object
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_get_balance_response.Content);

            // Assert that the returned balance matches the expected balance
            Assert.Equal(_expectedBalance, balanceResponse.Amount);
        }

        //Scenario: [Get positive balance]	
        [Given(@"I make a deposit of (.*)")]
        public async Task GivenIMakeADeposit(decimal _depositAmount)
        {
            // Example: Deposit a valid amount of 100
            //_depositAmount = 100;
            //decimal Amount;
            // Prepare the deposit request
            var deposit = new { Amount = _depositAmount };
            _post_deposit_request = new RestRequest("/onlinewallet/deposit", Method.Post);
            _post_deposit_request.AddJsonBody(deposit);

            // Execute the request
            _post_deposit_response = await _client.ExecuteAsync(_post_deposit_request);
        }

        [Given(@"as a result I have a wallet with balance bigger than zero before balance query")]
        public void GivenIHaveAWalletWithBalanceBiggerThanZero()
        {
            // Set expected balance to a non-zero value for this scenario
            _expectedBalance = 100;
        }       
    }

    public class Balance
    {
        public decimal Amount { get; set; }
    }
}