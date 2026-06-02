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
        Scenario: RP14A employee surname of exactly 99 characters passes validation
            Given the RP14A XML contains an employee surname of length 99
             When I attempt to submit the RP14A
             Then the submission should succeed

        @regression @validation @rp14a @allure.subSuite:Employee
        Scenario: RP14A Display error when employee surname is 100 characters
            Given the RP14A XML contains an employee surname of length 100
             When I attempt to submit the RP14A
             Then I should see the employee surname error "1 invalid length of the employee surname" with hint "Maximum of 99 characters allowed"


        @regression @validation @rp14a @allure.subSuite:NationalInsurance @bug
        Scenario: RP14A Display error for missing employee national insurance number
            Given the RP14A contains an employee with no national insurance number
             When I attempt to submit the RP14A
             Then I should see the national insurance number validation error "1 missing the employee national insurance number"
              And I should be able to view national insurance number error details

       @regression @validation @rp14a @allure.subSuite:NationalInsurance @addVideo
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

        @regression @validation @rp14a @allure.subSuite:NationalInsurance @addVideo
        Scenario Outline: RP14A Display error for multiple invalid employee national insurance number formats
            Given the RP14A contains <employeeCount> employees with national insurance number "<nationalInsuranceNumber>"
             When I attempt to submit the RP14A
             Then I should see the following national insurance number validation errors
                  | Message                                                           | Hint                | Type                               |
                  | <employeeCount> invalid employee national insurance number format | Format is AB112233C | Employee national insurance number |
              And I should be able to view multiple national insurance numbers error details

        Examples:
                  | employeeCount | nationalInsuranceNumber |
                  | 3             | 123456789               |
   
   
        @regression @validation @rp14a @allure.subSuite:NationalInsurance @addVideo @bug
        Scenario Outline: RP14A Display error for multiple missing employee national insurance numbers
            Given the RP14A contains <employeeCount> employees with no national insurance number
             When I attempt to submit the RP14A
             Then I should see the following national insurance number validation errors
                  | Message                                                        | Hint                | Type                               |
                  | <employeeCount> missing the employee national insurance number | Format is AB112233C | Employee national insurance number |
              And I should be able to view multiple national insurance numbers error details

        Examples:
                  | employeeCount |
                  | 3             |

        @regression @validation @rp14a @allure.story:Employee @bug
        Scenario: RP14A Display error when employment start date is after employment end date
            Given the RP14A contains employment start date "2026-04-30" with end date "2026-04-01"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                     | Hint                                   | Type                      |
                  | 1 invalid employment dates for the employee | Start date must be before the end date | Employee employment dates |
              And I should be able to view the employee employment dates error details
              And I should be able to go to the previous page from the error details page


         @regression @validation @rp14a @allure.subSuite:Employee @addVideo
         Scenario: RP14A Display aggregated count for repeated employee surname errors
            Given the RP14A contains 3 employees with no surname
             When I attempt to submit the RP14A
             Then I should see the validation error "3 missing employee surname"
             And I should be able to view the validation error details for employees where the surname is missing