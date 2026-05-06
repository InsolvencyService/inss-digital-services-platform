Feature: Check Your Answers

As an Insolvency Practitioner
I want to see the file that I have uploaded
So that I can confirm it’s the correct file and continue

Background:
    Given I am uploading an RP14A form

  @smoke @functional
  Scenario: Review uploaded document details
    Given I have uploaded a valid RP14A document
    When I continue to the check your answers page
    Then I should be able to review my uploaded document
    And the uploaded document name should be displayed

  @functional
  Scenario: Change the uploaded document
    Given I am reviewing my uploaded RP14A document
    When I choose to change the uploaded document
    Then I should be returned to the document upload page

  @smoke @functional
  Scenario: Submit the RP14A form
    Given I am reviewing my uploaded RP14A document
    When I submit the RP14A form
    Then the RP14A form should be successfully submitted

  @functional
  Scenario: Return to the upload page from the review page
    Given I am reviewing my uploaded RP14A document
    When I navigate back from the review page
    Then I should be returned to the document upload page
