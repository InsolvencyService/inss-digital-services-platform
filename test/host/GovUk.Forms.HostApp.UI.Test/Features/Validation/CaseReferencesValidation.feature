@MEDS-1067 @cleanCosmosDb @@addVideo
Feature: Case References Validation

              As an Insolvency Practitioner user
              I want RP14A validation to run before submission to Dynamics
  So that I can fix errors immediately and avoid delayed rejection


        @regression @caseReference @smoke
        Scenario: IP enters a valid case reference and is taken to the Employer Details page
            Given I am on the declaration page as a "InssTestEight" user
              And I am on the case reference number page
             When I enter a valid case reference number
              And I proceed to the next page
             Then I will be taken to the Employer Details page

        @regression @employerDetails
        Scenario: IP declines the correct employer and is taken back to the case reference number page
            Given I am on the declaration page as a "InssTestEight" user
              And I am on the employer details page
             When I confirm that this is not the correct employer name
              And I proceed to the next page
             Then I will be taken to the case reference number page

        @regression @caseReference
        Scenario: IP clicks Back on the case reference page and is taken to the declaration page
            Given I am on the declaration page as a "InssTestEight" user
              And I am on the case reference number page
             When I go to the previous page
             Then I will be taken to the declaration page

        @regression @employerDetails
        Scenario: IP clicks Back on the employer details page and is taken to the case reference page
            Given I am on the declaration page as a "InssTestEight" user
              And I am on the employer details page
             When I go to the previous page
             Then I will be taken to the case reference number page

        @regression @validation @rp14a
        Scenario:RP14A Show validation error when case reference is missing
            Given I am on the upload page as a "InssTestEight" user
              And the RP14A contains an employee row with no case reference
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
              And I should be able to view case reference error details on a table

        @regression @validation @rp14a @api-upload
        Scenario: RP14A API show validation error when case reference is missing
            Given I am on the upload page as a "InssTestEight" user
              And the RP14A contains an employee row with no case reference
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
              And I should be able to view case reference error details on a table

        @regression @smoke
        Scenario: Employer details are displayed for a valid case reference
            Given I am on the declaration page as a "InssTestEight" user
              And I have entered a valid case reference
             When I navigate to the employer details page
             Then I should see the case reference number
              And I should see the employer name

        @regression @caseReference
        Scenario Outline: Invalid case reference inputs show corresponding validation errors
            Given I am on the declaration page as a "InssTestEight" user
              And I am on the case reference number page
             When I enter "<caseReference>" as the case reference number
              And I proceed to the next page
             Then I should see the case reference error "<errorMessage>"

        Examples:
                  | caseReference | errorMessage                                           |
                  |               | The case reference number is not in the correct format |
                  | A             | The case reference number is not in the correct format |
                  | CN700005371   | The case reference number is not in the correct format |
                  | CN7000053     | The case reference number is not in the correct format |
                  | AB70000537    | The case reference number is not in the correct format |
                  | 12345678      | The case reference number is not in the correct format |
                  | CN7000@537    | The case reference number is not in the correct format |

        @regression @caseReference
        Scenario: IP enters a correctly formatted case reference not linked to a valid employer and sees an error
            Given I am on the declaration page as a "InssTestEight" user
              And I am on the case reference number page
             When I enter a case reference number that has not been linked to an employer
              And I proceed to the next page
             Then I should see the case reference error "The case reference number you entered has not been linked to a valid employer"

        @regression @employerDetails
        Scenario: IP sees the matched case reference and employer name on the Employer Details page
            Given I am on the declaration page as a "InssTestEight" user
              And I am on the employer details page
             Then I will see the case reference number I entered
              And I will see the name of the employer it relates to

        @regression @validation @rp14a
        Scenario: RP14A Show validation error when case reference format is invalid
            Given I am on the upload page as a "InssTestEight" user
              And the RP14A contains a case reference "AB12345678"
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
              And I should be able to view case reference error details

        @regression @validation @rp14a @api-upload
        Scenario: RP14A API show validation error when case reference format is invalid
            Given I am on the upload page as a "InssTestEight" user
              And the RP14A contains a case reference "AB12345678"
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
              And I should be able to view case reference error details


        @regression @validation @rp14a
        Scenario: Display error when case reference is not found in RPS
            Given I am on the upload page as a "InssTestEight" user
              And the RP14A contains a case reference does not exist in RPS
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
              And I should be able to view case reference error details


        @regression @validation @rp14a
        Scenario: RP14A displays multiple errors for case reference too long
            Given I am on the upload page as a "InssTestEight" user
              And the RP14A contains 3 employees with a case reference that is too long
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
              And I should be able to view case reference error details for multiple employees

        @regression @validation @rp14
        Scenario: RP14 display error for case reference longer than 10 characters
            Given I am on the upload page as a "InssTestEight" user
              And the RP14 XML contains case reference "CN123456781"
             When I attempt to submit the RP14
             Then I should see the following RP14 validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
     

        @regression @validation @rp14
        Scenario: RP14 display error for case reference are in the wrong format
            Given I am on the upload page as a "InssTestEight" user
              And the RP14 XML contains case reference "0012345678"
             When I attempt to submit the RP14
             Then I should see the following RP14 validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
     

        @regression @validation @rp14
        Scenario: Display error for unknown case reference
            Given I am on the upload page as a "InssTestEight" user
              And the RP14 XML contains a valid format case reference that does not exist in RPS
             When I attempt to submit the RP14
             Then I should see the following RP14 validation errors
                  | Message                                                                           | Hint | Type           |
                  | 1 case reference does not match the validated case reference {validCaseReference} |      | Case reference |
    