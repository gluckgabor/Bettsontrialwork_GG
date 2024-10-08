Feature: GetBalance

A short summary of the feature

@tag1
Scenario: [Get initial zero balance]	
Given I have a wallet with balance zero before querying balance
When I query the balance
Then I get response of correct balance

Scenario: [Get positive balance]	
Given I make a deposit of 100.00
And as a result I have a wallet with balance bigger than zero before balance query
When I query the balance
Then I get response of correct balance