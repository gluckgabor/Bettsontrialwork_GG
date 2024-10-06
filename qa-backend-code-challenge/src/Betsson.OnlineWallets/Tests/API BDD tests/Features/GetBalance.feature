Feature: GetBalance

A short summary of the feature

@tag1
Scenario: [scenario name1]	
Given I have a wallet with balance zero
When I query the balance
Then I get response of correct balance

Scenario: [scenario name2]	
Given I have a wallet with balance bigger than zero
When I query the balance
Then I get response of correct balance

Scenario: [scenario name3]	
Given I have a wallet with balance bigger than zero
When I query the balance with invalid param
Then I get response of validation/error message
