Feature: Withdraw

A short summary of the feature

@tag1

Scenario: [Withdrawing valid amount being smaller than balance]
	Given I have a wallet with balance bigger then zero
	When I withdraw valid value
	Then I get response of correct new balance

Scenario: [Withdrawing same amount as balance]
	Given I have a wallet with balance bigger then zero
	When I withdraw valid value
	Then I get response of correct new balance

	Scenario: [Attempting to withdraw more than the balance]
	Given I have a wallet with balance zero
	When I withdraw valid value
	Then I get response of "no sufficient funds".

Scenario: [Attempting to withdraw invalid value]
	Given I have a wallet with balance zero
	When I withdraw invalid value
	Then I get response of validation/error message
