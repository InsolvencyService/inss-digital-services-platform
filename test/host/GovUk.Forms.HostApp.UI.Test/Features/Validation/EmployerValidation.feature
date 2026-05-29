@MEDS-1067
Feature: Employer Validation

              As an Insolvency Practitioner user
              I want RP14A validation to run before submission to Dynamics
  So that I can fix errors immediately and avoid delayed rejection

        Background:
            Given I am on the upload page as a "Admin" user

        @regression @validation @rp14a
        Scenario Outline: Employer name length boundary validation
            Given I have uploaded an RP14A file with employer name of length <length>
             When I submit the RP14A file
             Then the submission should be "<outcome>"
              And the error summary should "<summaryBehaviour>" with "<detailsBehaviour>"
              And I should be able to view error details

        Examples:
                  | length | outcome  | summaryBehaviour                      | detailsBehaviour                 |
                  | 99     | accepted | none                                  | none                             |
                  | 100    | rejected | 1 invalid length of the employer name | Maximum of 99 characters allowed |

@regression @validation @rp14a @addScreencast
Scenario: RP14A Display multiple validation categories together
    Given the RP14A contains multiple validation issues
    When I attempt to submit the RP14A
    Then I should see the following validation categories
        | Category         |
        | Case             |
        | Employer         |
        | Employee         |
        | Employee pay     |
        | Employee holiday |
    And I should see the following multiple validation errors
        | Category         | Type                        | Message                               | Hint                             |
        | Case             | Case reference              | 1 missing a case reference            |                                  |
        | Case             | Case reference              | 1 invalid case reference format       | Format is CN12345678             |
        | Employer         | Employer name               | 1 invalid length of the employer name | Maximum of 99 characters allowed |
        | Employee         | Employee surname            | 1 missing employee surname            |                                  |
        | Employee pay     | Employee basic pay per week | 1 invalid basic pay per week          | Expected format is 12.34 or 100  |
        | Employee holiday | Holiday owed                | 1 invalid holiday owed                | Expected format is 28.25 or 33   |
        | Employee holiday | Holiday owed                | 1 invalid range of holiday owed       | 0 to 365 days allowed            |
    And I should be able to view error details for all validation categories


  @regression @validation @rp14a
  Scenario: Multiple employer names exceeding allowed length are rejected
         Given I have uploaded an RP14A file with 3 employer names of length 100
         When I submit the RP14A file
         Then the submission should be "rejected"
          And the error summary should "3 invalid length of the employer name" with "Maximum of 99 characters allowed"
          And I should be able to view error details for multiple employees     