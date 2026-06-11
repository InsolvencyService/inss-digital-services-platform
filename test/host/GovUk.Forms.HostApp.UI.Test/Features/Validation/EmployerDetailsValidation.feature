@MEDS-1067
Feature: Employer Details Validation

As an Insolvency Practitioner user
I want RP14A validation to run before submission to Dynamics
So that I can fix errors immediately and avoid delayed rejection

 Background:
      Given I am on the upload page as a "Admin" user

 @regression @validation @rp14a @addVideo
 Scenario Outline: Employer name length boundary validation
            Given I have uploaded an RP14A file with employer name of length <length>
             When I submit the RP14A file
             Then the submission should be "<outcome>"
              And the error summary should "<summaryBehaviour>" with "<detailsBehaviour>"
              And I should be able to view error details

        Examples:
                  | length | outcome  | summaryBehaviour                    | detailsBehaviour          |
                  |     99 | accepted | none                                | none                      |
                  |    100 | rejected | 1 employer name is the wrong length | Enter up to 99 characters |

@regression @validation @rp14a @addVideo
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
        | Category         | Type                        | Message                                     | Hint                                     |
        | Case             | Case reference              | 1 case reference is missing                 | Enter a reference number like CN12345678 |
        | Employer         | Employer name               | 1 employer name is the wrong length         | Enter up to 99 characters                |
        | Employee         | Employee surname            | 1 employee surname is missing               |                                          |
        | Employee pay     | Employee basic pay per week | 1 weekly pay amount is incorrect            | Enter a number like 12.34 or 100         |
        | Employee holiday | Holiday owed                | 1 holiday owed is incorrect                 | Enter a number like 28.25 or 33          |
        | Employee holiday | Holiday owed                | 1 holiday owed is outside the allowed range | Enter a value between 0 and 365          |
    And I should be able to view error details for all validation categories

@regression @validation @rp14a @addVideo @api-upload
Scenario: RP14A API display multiple validation categories together
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
        | Category         | Type                        | Message                                     | Hint                                     |
        | Case             | Case reference              | 1 case reference is missing                 | Enter a reference number like CN12345678 |
        | Employee         | Employee surname            | 1 employee surname is missing               |                                          |
        | Employee pay     | Employee basic pay per week | 1 weekly pay amount is incorrect            | Enter a number like 12.34 or 100         |
        | Employee holiday | Holiday owed                | 1 holiday owed is incorrect                 | Enter a number like 28.25 or 33          |
        | Employee holiday | Holiday owed                | 1 holiday owed is outside the allowed range | Enter a value between 0 and 365          |
    And I should be able to view error details for all validation categories


@regression @validation @rp14a @addVideo
  Scenario: Multiple employer names exceeding allowed length are rejected
         Given I have uploaded an RP14A file with 3 employer names of length 100
         When I submit the RP14A file
         Then the submission should be "rejected"
          And the error summary should "3 employer names are the wrong length" with "Enter up to 99 characters"
          And I should be able to view error details for multiple employees

@regression @validation @rp14a @addVideo
Scenario: RP14A Display multiple errors for the same employee
            Given the RP14A contains an employee with:
                 | Surname | NationalInsuranceNumber | MoneyOwedToEmployer | BasicPayPerWeek |
                 |         | QQ123456A               |              12.345 |            12.3 |
             When I attempt to submit the RP14A
             Then I should see the following multiple validation errors
                  | Category     | Type                               | Message                                            | Hint                                                 |
                  | Employee     | Employee surname                   | 1 employee surname is missing                      |                                                      |
                  | Employee     | Employee national insurance number | 1 National Insurance number is in the wrong format | Enter a National Insurance number like QQ 12 34 56 C |
                  | Employee     | Money owed to employer             | 1 amount owed to the employer is incorrect         | Enter a number like 12.34 or 100                     |
                  | Employee pay | Employee basic pay per week        | 1 weekly pay amount is incorrect                   | Enter a number like 12.34 or 100                     |
             And I should be able to view error details for all validation categories

@regression @validation @rp14a @addVideo @api-upload
Scenario: RP14A API display multiple errors for the same employee
            Given the RP14A contains an employee with:
                 | Surname | NationalInsuranceNumber | MoneyOwedToEmployer | BasicPayPerWeek |
                 |         | QQ123456A               |              12.345 |            12.3 |
             When I attempt to submit the RP14A
             Then I should see the following multiple validation errors
                  | Category     | Type                               | Message                                            | Hint                                                 |
                  | Employee     | Employee surname                   | 1 employee surname is missing                      |                                                      |
                  | Employee     | Employee national insurance number | 1 National Insurance number is in the wrong format | Enter a National Insurance number like QQ 12 34 56 C |
                  | Employee     | Money owed to employer             | 1 amount owed to the employer is incorrect         | Enter a number like 12.34 or 100                     |
                  | Employee pay | Employee basic pay per week        | 1 weekly pay amount is incorrect                   | Enter a number like 12.34 or 100                     |
             And I should be able to view error details for all validation categories

