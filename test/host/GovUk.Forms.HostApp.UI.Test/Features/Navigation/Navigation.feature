Feature: Navigation

  As an Insolvency Practitioner
  I want to be shown a helpful error page
  So that I know when I have navigated to a page that does not exist

@regression @navigation
Scenario: User navigates to an invalid IP Upload URL
    Given I am on the IP Upload application
    When I navigate to an invalid IP Upload URL
    Then the Page not found page should be displayed
