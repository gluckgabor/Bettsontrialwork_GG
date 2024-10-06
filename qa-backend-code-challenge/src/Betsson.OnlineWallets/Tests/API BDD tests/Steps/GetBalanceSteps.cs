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

        public GetBalanceSteps()
        {
            _client = new RestClient("http://localhost:8080");
        }

        [Given(@"I have a wallet with balance zero")]
        public void GivenIHaveAWalletWithBalanceZero()
        {
            // Set expected balance to zero for this scenario
            _expectedBalance = 0;

            // In a real system, this might involve setting up a mock repository or 
            // clearing wallet transactions in a test environment.
        }

        [Given(@"I have a wallet with balance bigger than zero")]
        public void GivenIHaveAWalletWithBalanceBiggerThanZero()
        {
            // Set expected balance to a non-zero value for this scenario
            _expectedBalance = 100;  // Example: set expected balance to 100

            // In a real system, you would probably set up some transactions to reach this balance.
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