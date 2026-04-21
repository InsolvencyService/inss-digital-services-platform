Feature: Start Page Navigation

As an Insolvency Practitioner
I want to access the IPUS start page
So I can begin the RP14/14A upload journey

 @smoke @MEDS-1061
  Scenario: User accesses customer feedback from the start page
    Given the user is on the IPUS start page
    When the user chooses to provide feedback
    Then a new browser tab should be opened
	And the Director Conduct Reporting Service customer feedback page should be displayed

@smoke @MEDS-1061
Scenario: Start the service from the start page
  Given the user is on the IPUS start page
  When the user chooses to start the application
  Then the user is redirected to the sign-in page
        

