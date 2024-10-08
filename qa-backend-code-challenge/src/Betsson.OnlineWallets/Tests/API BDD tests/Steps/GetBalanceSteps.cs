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

        private RestRequest _get_balance_request;
        private RestResponse _get_balance_response;

        private RestRequest _post_deposit_request;
        private RestResponse _post_deposit_response;

        public GetBalanceSteps()
        {
            _client = new RestClient("http://localhost:8080");
        }

        [Given(@"I have a wallet with balance zero before querying balance")]
        public void GivenIHaveAWalletWithBalanceZero()
        {
            // Set expected balance to zero for this scenario
            _expectedBalance = 0;
        }

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

        [When(@"I query the balance")]
        public async Task WhenIQueryTheBalance()
        {
            _request = new RestRequest("/onlinewallet/balance", Method.Get);
            _response = await _client.ExecuteAsync(_request);
        }

        [Then(@"I get response of correct balance")]
        public void ThenIGetResponseOfCorrectBalance()
        {
            // Check if the response is OK
            Assert.Equal(HttpStatusCode.OK, _response.StatusCode);

            // Deserialize the response content into a Balance object
            var balanceResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Balance>(_response.Content);

            // Assert that the returned balance matches the expected balance
            Assert.Equal(_expectedBalance, balanceResponse.Amount);
        }
    }

    public class Balance
    {
        public decimal Amount { get; set; }
    }
}