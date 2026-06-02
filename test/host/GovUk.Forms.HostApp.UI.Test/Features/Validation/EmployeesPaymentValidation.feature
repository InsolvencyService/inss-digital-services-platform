Feature: Employees Payment Validation

A short summary of the feature
  Background:
  Given I am on the upload page as a "Admin" user

@regression @validation @rp14a @allure.subSuite:Payment @addVideo
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

@regression @validation @rp14a @allure.subSuite:Payment @addVideo
   Scenario Outline: RP14A Display multiple errors for invalid arrears of pay owed format
            Given the RP14A contains <count> invalid arrears of pay owed
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                       | Hint                            | Type                             |
                  | <count> invalid arrears of pay owed | Expected format is 12.34 or 100 | Employee arrears of payment owed |
              And I should be able to view employee arrears of pay owed error details for multiple employees
        Examples:
                  | count |
                  | 2     |
                  | 3     |

@regression @validation @rp14a @allure.subSuite:Payment @addVideo
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

@regression @validation @rp14a @allure.subSuite:Payment @addVideo
     Scenario Outline: RP14A Display error for multiple invalid money owed to employer formats
            Given the RP14A contains <employeeCount> employees with money owed to employer "<moneyOwed>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                        | Hint                            | Type                   |
                  | <employeeCount> invalid money owed to employer | Expected format is 12.34 or 100 | Money owed to employer |
              And I should be able to view money owed to multiple employers error details

        Examples:
                  | employeeCount | moneyOwed |
                  | 3             | 12.3      |


  @regression @validation @rp14a @allure.story:Payment @bug
  Scenario: RP14A Display error when arrears of pay start date is after end date
            Given the RP14A contains arrears of pay start date "2026-04-30" and end date "2026-04-01"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                    | Hint                                   | Type                              |
                  | 1 invalid arrears of dates | Start date must be before the end date | Employee arrears of payment dates |
              And I should be able to view error details

@regression @validation @rp14a @allure.story:Payment @addVideo
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


@regression @validation @rp14a @allure.story:Payment @addVideo
 Scenario Outline: RP14A Display error for multiple invalid employee basic pay per week formats
            Given the RP14A contains <employeeCount> employees with employee basic pay per week "<basicPayPerWeek>"
             When I attempt to submit the RP14A
             Then I should see the following basic pay per week validation errors
                  | Message                                    | Hint                            | Type                        |
                  | <employeeCount> invalid basic pay per week | Expected format is 12.34 or 100 | Employee basic pay per week |
              And I should be able to view basic pay per week error details for multiple employees

        Examples:
                  | employeeCount | basicPayPerWeek |
                  | 3             | -100            |

