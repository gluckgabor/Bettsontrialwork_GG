Feature: Deposit

A short summary of the feature

@tag1
Scenario: [Initial balance zero, valid amount deposited]
	Given I have a wallet with balance zero before deposit
	When I deposit valid value
	Then I get response of correct new balance containing deposited amount

Scenario: [Invalid value attempted to be deposited]
	Given I have a wallet with balance zero before deposit
	When I deposit invalid value
	Then I get response of validation/error message

Scenario: [Initial value bigger than zero, valid amount deposited]
	Given I query the balance
	And I have a wallet with balance bigger then zero before deposit
	When I deposit valid value
	Then I get response of correct new balance containing deposited amount
