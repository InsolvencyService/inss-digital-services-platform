Feature: Start Page Navigation

As an Insolvency Practitioner
I want to access the IPUS start page
So I can begin the RP14/14A upload journey
Background: 
	Given the user is on the IPUS start page
 @functional @MEDS-1061
  Scenario: User accesses customer feedback from the start page
    When the user chooses to provide feedback
    Then a new browser tab should be opened
	And the Director Conduct Reporting Service customer feedback page should be displayed

@functional @MEDS-1061
Scenario: Start the service from the start page
  When the user chooses to start the application
  Then the user is redirected to the sign-in page
        

@visual @smoke
Scenario: Verify Start Page visual snapshot
  Then the start page should match the visual snapshot

@functional @footer
Scenario Outline: Verify footer links navigate correctly
  Given the user is on the IPUS start page
  When the user clicks on the "<linkName>" link in the footer
  Then a new page or tab should open with url containing "<expectedUrl>"

Examples:
  | linkName    | expectedUrl                      |
  | Privacy     | privacy                         |
  | Cookies     | cookies                         |
  | Contact us  | contact                         |
  | Open Government Licence v3.0 | nationalarchives.gov.uk |
  | Crown copyright | nationalarchives.gov.uk |