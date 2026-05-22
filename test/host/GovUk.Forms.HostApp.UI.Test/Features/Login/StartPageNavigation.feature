@MEDS-1061
Feature: Start Page Navigation

As an Insolvency Practitioner
I want to access the IPUS start page
So I can begin the RP14/14A upload journey

Background: 
	Given the user is on the IPUS start page
       
 @functional
  Scenario: User accesses customer feedback from the start page
    When the user chooses to provide feedback
    Then a new browser tab should be opened
	And the Director Conduct Reporting Service customer feedback page should be displayed

@functional
Scenario: Start the service from the start page
  When the user chooses to start the application
  Then the user is redirected to the sign-in page
        

@visual @smoke
Scenario: Verify Start Page visual snapshot
  Then the start page should match the visual snapshot

@functional @footer
Scenario Outline: Verify footer links navigate correctly
  When the use chooses to open "<linkName>" in the footer
  Then a new page should open with title "<expectedTitle>" with url containing "<expectedUrl>"

Examples:
  | linkName                     | expectedUrl                  | expectedTitle                                                  |
  | Privacy                      | personal-information-charter | Personal information charter - The Insolvency Service - GOV.UK |
  | Cookies                      | cookies                      | Cookies                                                        |
  | Contact us                   | login                        | Login                                                          |
  | Open Government Licence v3.0 | open-government-licence      | Open Government Licence                                        |
  | Crown copyright              | crown-copyright              | Crown copyright - Re-using PSI                                 |