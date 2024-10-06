Feature: GetBalance

A short summary of the feature

@tag1
Scenario: [Get initial balance]	
Given I have a wallet with balance zero
When I query the balance
Then I get response of correct balance

Scenario: [Get positive balance]	
Given I have a wallet with balance bigger than zero
When I query the balance
Then I get response of correct balance