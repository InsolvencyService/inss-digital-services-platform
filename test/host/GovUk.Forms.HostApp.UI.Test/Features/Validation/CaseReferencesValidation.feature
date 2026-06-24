@MEDS-1067 @cleanCosmosDb
Feature: Case References Validation

 As an Insolvency Practitioner user
 I want RP14A validation to run before submission to Dynamics
  So that I can fix errors immediately and avoid delayed rejection

        Background:
            Given I am on the upload page as a "InssTestEight" user

        @regression @validation @rp14a
        Scenario:RP14A Show validation error when case reference is missing
            Given the RP14A contains an employee row with no case reference
             When I attempt to submit the RP14A
             Then I should see the validation error "1 case reference is missing" with the hint "Enter a reference number like CN12345678"
              And I should be able to view case reference error details on a table

        @regression @validation @rp14a @api-upload
        Scenario: RP14A API show validation error when case reference is missing
            Given the RP14A contains an employee row with no case reference
             When I attempt to submit the RP14A
             Then I should see the validation error "1 case reference is missing" with the hint "Enter a reference number like CN12345678"
              And I should be able to view case reference error details on a table

        @regression @validation @rp14a
        Scenario: RP14A Show validation error when case reference format is invalid
            Given the RP14A contains a case reference "AB12345678"
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                 | Hint | Type           |
                  | 1 case reference is in the wrong format |      | Case reference |
              And I should be able to view case reference error details

        @regression @validation @rp14a @api-upload
        Scenario: RP14A API show validation error when case reference format is invalid
            Given the RP14A contains a case reference "AB12345678"
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                 | Hint | Type           |
                  | 1 case reference is in the wrong format |      | Case reference |
              And I should be able to view case reference error details

        @regression @validation @rp14a
        Scenario:RP14A display error for case reference longer than 10 characters
            Given the RP14A contains a case reference "CN123456789"
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                      | Hint                      | Type           |
                  | 1 case reference is too long | Enter up to 10 characters | Case reference |
              And I should be able to view case reference error details

        @regression @validation @rp14a @api-upload
        Scenario: RP14A API display error for case reference longer than 10 characters
            Given the RP14A contains a case reference "CN123456789"
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                      | Hint                      | Type           |
                  | 1 case reference is too long | Enter up to 10 characters | Case reference |
              And I should be able to view case reference error details

        @regression @validation @rp14a 
        Scenario: Display error when case reference is not found in RPS
            Given the RP14A contains a case reference does not exist in RPS
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                        | Hint | Type           |
                  | 1 case reference was not found |      | Case reference |
             And I should be able to view case reference error details


      @regression @validation @rp14a
      Scenario Outline: RP14A displays multiple errors for missing case reference
            Given the RP14A contains <count> employees with no case reference
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                     | Hint                                     | Type           |
                  | 1 case reference is missing | Enter a reference number like CN12345678 | Case reference |
              And I should be able to view case reference error details for multiple employees

        Examples:
                  | count |
                  | 3     |

      @regression @validation @rp14a
      Scenario Outline: RP14A displays multiple errors for invalid case reference format
            Given the RP14A contains <count> invalid case references
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                 | Hint | Type           |
                  | 1 case reference is in the wrong format |      | Case reference |
              And I should be able to view case reference error details for multiple employees

        Examples:
                  | count |
                  | 3     |

      @regression @validation @rp14a
      Scenario: RP14A displays multiple errors for case reference too long
            Given the RP14A contains 3 employees with a case reference that is too long
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                      | Hint                      | Type           |
                  | 1 case reference is too long | Enter up to 10 characters | Case reference |
              And I should be able to view case reference error details for multiple employees

     
     @regression @validation @rp14 
       Scenario Outline: RP14 Display error for invalid case reference format
            Given the RP14 XML contains case reference "<caseReference>"
             When I attempt to submit the RP14
             Then I should see the following RP14 validation errors
                  | Message                                 |
                  | 1 case reference is in the wrong format |
    
        Examples:
                  | caseReference |
                  | AB12345678    |
                  | CN1234567A    |
                  | 12345678      |
                  | CN12345       |
                  | cnABC45678    |


      @regression @validation @rp14
      Scenario: RP14 display error for case reference longer than 12 characters
            Given the RP14 XML contains case reference "CN12345678901"
             When I attempt to submit the RP14
             Then I should see the following RP14 validation errors
                  | Message                      | Hint                      |
                  | 1 case reference is too long | Enter up to 10 characters |

      @regression @validation @rp14
      Scenario: RP14 display error for case reference are in the wrong format
            Given the RP14 XML contains case reference "0012345678"
             When I attempt to submit the RP14
             Then I should see the following RP14 validation errors
                  | Message                                 | Hint |
                  | 1 case reference is in the wrong format |      |


        @regression @validation @rp14
        Scenario: Display error for unknown case reference
            Given the RP14 XML contains a valid format case reference that does not exist in RPS
             When I attempt to submit the RP14
             Then I should see the following RP14 validation errors
                  | Message                        | Hint |
                  | 1 case reference was not found |      |


    # ── Prototype v5B — Case Reference Number page ─────────────────────────

    @regression @caseReference
    Scenario: IP is taken to the case reference number page after agreeing to the declaration
        Given I am on the declaration page as a "InssTestThree" user
         When I click Agree and continue
         Then I will be taken to the case reference number page

    @regression @caseReference
    Scenario: IP enters a valid case reference and is taken to the Employer Details page
        Given I am on the declaration page as a "InssTestThree" user
          And I am on the case reference number page
         When I enter a valid case reference number
          And I click Continue
         Then I will be taken to the Employer Details page

    @regression @caseReference
    Scenario Outline: Invalid case reference inputs show corresponding validation errors
        Given I am on the declaration page as a "InssTestThree" user
          And I am on the case reference number page
         When I enter "<caseReference>" as the case reference number
          And I click Continue
         Then I should see the case reference error "<errorMessage>"

        Examples:
            | caseReference | errorMessage                                           |
            |               | Enter a case reference number                          |
            | A             | The case reference number is too short                 |
            | CN12345678901 | The case reference number is too long                  |
            | 12345678      | The case reference number is not in the correct format |

    @regression @caseReference
    Scenario: Case reference number not linked to an employer shows an error
        Given I am on the declaration page as a "InssTestThree" user
          And I am on the case reference number page
         When I enter a case reference number that has not been linked to an employer
          And I click Continue
         Then I should see the case reference error "The case reference number you entered has not been linked to a valid employer"


    # ── Prototype v5B — Employer Details page ───────────────────────────────

    @regression @employerDetails
    Scenario: IP sees the matched case reference and employer name on the Employer Details page
        Given I am on the declaration page as a "InssTestThree" user
          And I am on the employer details page
         Then I will see the case reference number I entered
          And I will see the name of the employer it relates to

    @regression @employerDetails
    Scenario: IP confirms the correct employer and is taken to the Upload a file page
        Given I am on the declaration page as a "InssTestThree" user
          And I am on the employer details page
         When I confirm that this is the correct employer name
          And I click Continue
         Then I will be taken to the Upload a file page

    @regression @employerDetails
    Scenario: IP declines the correct employer and is taken back to the case reference number page
        Given I am on the declaration page as a "InssTestThree" user
          And I am on the employer details page
         When I confirm that this is not the correct employer name
          And I click Continue
         Then I will be taken to the case reference number page

        @regression @caseReference
        Scenario: 1-IP is taken to the case reference number page after agreeing to the declaration
             When I click Agree and continue
             Then I will be taken to the case reference number page


        @regression @caseReference
        Scenario: 1-IP enters a valid case reference and is taken to the Employer Details page
            Given I am on the case reference number page
             When I enter a valid case reference number
              And I click Continue
             Then I will be taken to the Employer Details page


        @regression @caseReference
        Scenario Outline: 1-Invalid case reference inputs show corresponding validation errors
            Given I am on the case reference number page
             When I enter "<caseReference>" as the case reference number
              And I click Continue
             Then I should see the case reference error "<errorMessage>"

        Examples:
                  | caseReference | errorMessage                                           |
                  |               | Enter a case reference number                          |
                  |               | Enter a case reference number                          |
                  | A             | The case reference number is too short                 |
                  | CN1234567     | The case reference number is too short                 |
                  | CN123456789   | The case reference number is too long                  |
                  | 12345678      | The case reference number is not in the correct format |
                  | AB12345678    | The case reference number is not in the correct format |
                  | cn12345678    | The case reference number is not in the correct format |
                  | CN1234567A    | The case reference number is not in the correct format |
                  | CN12345@78    | The case reference number is not in the correct format |


        @regression @caseReference
        Scenario: Valid case reference not linked to an employer shows an error
            Given I am on the case reference number page
             When I enter a valid case reference number that is not linked to an employer
              And I click Continue
             Then I should see the case reference error "The case reference number you entered has not been linked to a valid employer"


        @regression @employerDetails
        Scenario: 1-IP sees the matched case reference and employer name on the Employer Details page
            Given I have entered a valid case reference number
             When I navigate to the Employer Details page
             Then I will see the case reference number I entered
              And I will see the name of the employer it relates to


        @regression @employerDetails
        Scenario: 1-IP confirms the correct employer and is taken to the Upload a file page
            Given I am on the Employer Details page
             When I confirm that this is the correct employer name
              And I click Continue
             Then I will be taken to the Upload a file page


        @regression @employerDetails
        Scenario: IP confirms this is not the correct employer and is taken back to the case reference number page
            Given I am on the Employer Details page
             When I confirm that this is not the correct employer name
              And I click Continue
             Then I will be taken to the case reference number page


        @regression @employerDetails
        Scenario: IP does not select whether the employer name is correct
            Given I am on the Employer Details page
             When I click Continue
             Then I should see the employer details error "Select whether this is the correct employer name"


        @regression @navigation
        Scenario: Case reference number is retained when IP navigates back from Employer Details page
            Given I have entered a valid case reference number
              And I am on the Employer Details page
             When I click Back
             Then the case reference number I entered should be displayed on the case reference number page


        @accessibility @caseReference
        Scenario: Case reference number page matches the accessibility snapshot
            Given I am on the case reference number page
             Then the case reference number page should match the accessibility snapshot


        @accessibility @employerDetails
        Scenario: Employer Details page matches the accessibility snapshot
            Given I am on the Employer Details page
             Then the Employer Details page should match the accessibility snapshot
