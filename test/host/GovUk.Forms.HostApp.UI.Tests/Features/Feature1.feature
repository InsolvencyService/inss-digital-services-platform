Feature: Feature1

A short summary of the feature

@tag1 @smoke @regression
Scenario: Verify that section 187 page is accessible from declaration page
	Given I am on the declaration page
	When I choose to view section 187 
	Then I will be taken to thesection 187 page
