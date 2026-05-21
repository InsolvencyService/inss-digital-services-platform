Feature: Employees Payment Validation

A short summary of the feature

 @regression @validation @rp14a @allure.subSuite:Payment
 Scenario Outline: RP14A Display error for invalid arrears of pay owed format
            Given the RP14A contains employee arrears of pay owed "<arrearsOfPay>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                       | Hint                            | Type                             |
                  | 1 invalid arrears of pay owed | Expected format is 12.34 or 100 | Employee arrears of payment owed |
              And I should be able to view employee arrears of pay owed error details

        Examples:
                  | arrearsOfPay |
                  | 12.3         |
                  | 12.345       |
                  | -100         |

