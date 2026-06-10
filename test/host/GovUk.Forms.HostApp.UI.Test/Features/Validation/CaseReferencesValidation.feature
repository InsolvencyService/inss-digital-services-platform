@MEDS-1067
Feature: Case References Validation

 As an Insolvency Practitioner user
 I want RP14A validation to run before submission to Dynamics
  So that I can fix errors immediately and avoid delayed rejection

        Background:
            Given I am on the upload page as a "Admin" user

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

        @regression @validation @rp14a  @ignore @NotImplemented
        Scenario: Display error when case reference is not found in RPS
            Given the RP14A contains a valid format case reference
              And the case reference does not exist in RPS
             When I attempt to submit the RP14A
             Then I should see the validation error "1 case reference was not found"


      @regression @validation @rp14a
      Scenario Outline: RP14A displays multiple errors for missing case reference
            Given the RP14A contains <count> employees with no case reference
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                             | Hint                                     | Type           |
                  | <count> case references are missing | Enter a reference number like CN12345678 | Case reference |
              And I should be able to view case reference error details for multiple employees

        Examples:
                  | count |
                  | 3     |

      @regression @validation @rp14a
      Scenario Outline: RP14A displays multiple errors for invalid case reference format
            Given the RP14A contains <count> invalid case references
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                                         | Hint | Type           |
                  | <count> case references are in the wrong format |      | Case reference |
              And I should be able to view case reference error details for multiple employees

        Examples:
                  | count |
                  | 3     |

      @regression @validation @rp14a
      Scenario: RP14A displays multiple errors for case reference too long
            Given the RP14A contains 3 employees with a case reference that is too long
             When I attempt to submit the RP14A
             Then I should see the following case reference validation errors
                  | Message                        | Hint                      | Type           |
                  | 3 case references are too long | Enter up to 10 characters | Case reference |
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


       ### the following scenario is currently ignored as we are not yet calling the RPS API to validate case references.
       ### Once we implement the RPS API call, we can enable this test to ensure we are properly handling cases where the reference is not found in RPS. 
       @ignore @regression @validation @rp14 @NotImplemented
        Scenario: Display error for unknown case reference
            Given the RP14 XML contains a valid format case reference that does not exist in RPS
             When I upload and submit the RP14 XML file
             Then I should see the validation error "1 case reference have not been matched in our system"
