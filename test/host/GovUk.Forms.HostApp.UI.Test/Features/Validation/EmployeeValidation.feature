@MEDS-1067
Feature: Employee Validation

              As an Insolvency Practitioner user
              I want RP14A validation to run before submission to Dynamics
  So that I can fix errors immediately and avoid delayed rejection

        Background:
            Given I am on the upload page as a "Admin" user

        @regression @validation @rp14a @allure.subSuite:Employee
        Scenario: RP14A Display error for missing employee surname
            Given the RP14A contains an employee with no surname
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing employee surname"
              And I should be able to view employee error details


        @regression @validation @rp14a @visual @allure.subSuite:Employee
        Scenario: Return to the upload page after attempting to submit an invalid RP14A
             Given the RP14A contains an employee with no surname
             When I attempt to submit the RP14A
              And I proceed to the check answers page
             Then I should be returned to the upload page

                  
        @regression @validation @rp14a @allure.subSuite:Employee
        Scenario: RP14A Display error when employee surname is longer than 99 characters
            Given the RP14A contains an employee surname longer than 99 characters
             When I attempt to submit the RP14A
             Then I should see the validation error "1 invalid length of the employee surname"
              And I should be able to view employee error details


        @regression @validation @rp14a @allure.subSuite:Payment
        Scenario Outline: RP14A Display multiple errors for invalid arrears of pay owed format
            Given the RP14A contains <count> invalid arrears of pay owed
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                             | Hint                            | Type                             |
                  | <count> invalid arrears of pay owed | Expected format is 12.34 or 100 | Employee arrears of payment owed |
              And I should be able to view employee arrears of pay owed error details for multiple employees
        Examples:
                  | count |
                  | 2     |
                  | 3     |

        @regression @validation @rp14a @allure.subSuite:NationalInsurance @bug
        Scenario: RP14A Display error for missing employee national insurance number
            Given the RP14A contains an employee with no national insurance number
             When I attempt to submit the RP14A
             Then I should see the national insurance number validation error "[COUNT] missing the employee national insurance number"
              And I should be able to view national insurance number error details

        @regression @validation @rp14a @allure.subSuite:NationalInsurance
        Scenario Outline: RP14A Display error for invalid employee national insurance number format
            Given the RP14A contains employee national insurance number "<nationalInsuranceNumber>"
             When I attempt to submit the RP14A
             Then I should see the national insurance number validation error "1 invalid employee national insurance number format"
              And I should be able to view national insurance number error details
        Examples:
                  | nationalInsuranceNumber |
                  | 123456789               |
                  | AB12345C                |
                  | QQ123456A               |
                  | AB123456Z               |

        @regression @validation @rp14a @allure.subSuite:Payment
        Scenario Outline: RP14A Display error for invalid money owed to employer format
            Given the RP14A contains money owed to employer "<moneyOwed>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                          | Hint                            | Type                   |
                  | 1 invalid money owed to employer | Expected format is 12.34 or 100 | Money owed to employer |
              And I should be able to view money owed to employer error details

        Examples:
                  | moneyOwed |
                  | 12.3      |
                  | 12.345    |
                  | -50       |


        @regression @validation @rp14a @allure.story:Employee @bug
        Scenario: RP14A Display error when employment start date is after employment end date
            Given the RP14A contains employment start date "2026-04-30" with end date "2026-04-01"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                     | Hint                                   | Type                      |
                  | 1 invalid employment dates for the employee | Start date must be before the end date | Employee employment dates |
              And I should be able to view the employee employment dates error details
              And I should be able to go to the previous page from the error details page


        @regression @validation @rp14a @allure.story:Payment @bug
        Scenario: RP14A Display error when arrears of pay start date is after end date
            Given the RP14A contains arrears of pay start date "2026-04-30" and end date "2026-04-01"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                    | Hint                                   | Type                              |
                  | 1 invalid arrears of dates | Start date must be before the end date | Employee arrears of payment dates |
              And I should be able to view error details


        @regression @validation @rp14a @allure.story:Payment
        Scenario Outline:RP14A Display error for invalid employee basic pay per week format
            Given the RP14A contains employee basic pay per week "<basicPayPerWeek>"
             When I attempt to submit the RP14A
              Then I should see the following basic pay per week validation errors
                  | Message                      | Hint                            | Type                        |
                  | 1 invalid basic pay per week | Expected format is 12.34 or 100 | Employee basic pay per week |
                And I should be able to view basic pay per week validation error details
        Examples:
                  | basicPayPerWeek |
                  | 12.3            |
                  | 15.345          |
                  | -100            |

        @regression @validation @rp14a @allure.subSuite:Employee
         Scenario: RP14A Display aggregated count for repeated employee surname errors
            Given the RP14A contains 3 employees with no surname
             When I attempt to submit the RP14A
             Then I should see the validation error "3 missing employee surname"
             And I should be able to view the validation error details for employees where the surname is missing