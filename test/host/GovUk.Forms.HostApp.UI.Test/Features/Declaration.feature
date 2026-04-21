@MEDS-1061
Feature: Declaration page

As an Insolvency Practitioner
I want to complete the declaration page
So that I can continue the RP14/14A upload journeyA short summary of the feature

Background: 
	Given I am on the declaration page

@smoke @MEDS-1061
Scenario: Verify that section 187 page is accessible from declaration page
	When I choose to view section 187 
	Then I will be taken to the section 187 page

@smoke @addScreencast
 Scenario: Navigate to file upload page after agreeing
    When I choose to Agree and continue
    Then I will be taken to the file upload page
@smoke
  Scenario: Navigate back to start page
	When I choose to return to the start page
	Then the start page should be displayed

@smoke @ignore
 Scenario: View declaration terms
    Given I am completing the IP Upload journey
    When I land on the declaration page
    Then I will see the terms I need to agree to