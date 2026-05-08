@ignore
Feature: Check Your Answers

As an Insolvency Practitioner
I want to see the file that I have uploaded
So that I can confirm it’s the correct file and continue


  @functional @rp14a
  Scenario: Return to the upload page from the review page
    Given I am reviewing my uploaded RP14A document
    When I navigate back from the review page
    Then I should be returned to the document upload page


@smoke @functional @rp14a
Scenario: Review and submit an uploaded RP14A form
    Given I am reviewing my uploaded RP14A document
    When I submit the RP14A form
    Then the RP14A form should be successfully submitted

@functional @rp14a
Scenario: Change the uploaded RP14A form before submitting
    Given I am reviewing my uploaded RP14A document
    When I choose to change the uploaded document
    Then I should be returned to the document upload page
