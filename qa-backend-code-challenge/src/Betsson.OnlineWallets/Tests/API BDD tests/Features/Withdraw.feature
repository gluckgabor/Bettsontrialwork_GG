Feature: Withdraw

A short summary of the feature

@tag1
Scenario: [scenario name1]
	Given I have a wallet with balance zero
	When I withdraw valid value
	Then I get response of "no sufficient funds".

Scenario: [scenario name2]
	Given I have a wallet with balance zero
	When I withdraw invalid value
	Then I get response of validation/error message

Scenario: [scenario name3]
	Given I have a wallet with balance bigger then zero
	When I withdraw valid value
	Then I get response of correct new balance

