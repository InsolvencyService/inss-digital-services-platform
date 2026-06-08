Feature: Employee Holiday Validation

A short summary of the feature

              As an Insolvency Practitioner user
              I want RP14A validation to run before submission to Dynamics
   So that I can fix errors immediately and avoid delayed rejection

    Background:
            Given I am on the upload page as a "Admin" user

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug
 Scenario: RP14A Display error for missing contracted holiday entitlement
            Given the RP14A contains no contracted holiday entitlement
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing contracted holiday entitlement"

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
 Scenario Outline: RP14A Display error for invalid contracted holiday entitlement format
            Given the RP14A contains contracted holiday entitlement "<contractedHoliday>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                  | Hint                           | Type                           |
                  | 1 invalid contracted holiday entitlement | Expected format is 28.25 or 33 | Contracted holiday entitlement |
               And I should be able to view contracted holiday entitlement validation error details

        Examples:
                  | contractedHoliday |
                  | 28.2              |
                  | 28.255            |
                  | -1                |

  @regression @validation @rp14a @allure.subSuite:EmployeeHoliday
   Scenario Outline:RP14A Display error for contracted holiday entitlement outside allowed range
            Given the RP14A contains contracted holiday entitlement "<contractedHoliday>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                           | Hint                  | Type                           |
                  | 1 invalid range of contracted holiday entitlement | 0 to 365 days allowed | Contracted holiday entitlement |
               And I should be able to view contracted holiday entitlement validation error details

        Examples:
                  | contractedHoliday |
                  |               366 |
                  |               999 |
                  |                -1 |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug
Scenario: RP14A Display error for missing holiday days carried forward
            Given the RP14A contains no holiday days carried forward
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing holiday days carried forward"

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
 Scenario Outline: RP14A Display error for invalid holiday days carried forward format
            Given the RP14A contains holiday days carried forward "<holidayCarriedForward>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                           | Hint                           | Type                    |
                  | 1 invalid holiday carried forward | Expected format is 28.25 or 33 | Holiday carried forward |
               And I should be able to view holiday days carried forward validation error details

        Examples:
                  | holidayCarriedForward |
                  | 28.3                  |
                  | 28.345                |
                  | -1                    |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
 Scenario Outline:RP14A Display error for holiday days carried forward outside allowed range
            Given the RP14A contains holiday days carried forward "<holidayCarriedForward>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                    | Hint                  | Type                    |
                  | 1 invalid range of holiday carried forward | 0 to 365 days allowed | Holiday carried forward |
               And I should be able to view holiday days carried forward validation error details
        Examples:
                  | holidayCarriedForward |
                  |                   366 |
                  |                   999 |
                  |                    -1 |


@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug
  Scenario:RP14A Display error for missing holiday days taken
            Given the RP14A contains no holiday days taken
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing holiday days taken"

  @regression @validation @rp14a @allure.subSuite:EmployeeHoliday
  Scenario Outline: RP14A Display error for invalid holiday days taken format
            Given the RP14A contains holiday days taken "<holidayTaken>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
              | Message                      | Hint                           | Type               |
              | 1 invalid holiday days taken | Expected format is 28.25 or 33 | Holiday days taken |
	         And I should be able to view holiday days taken validation error details
        Examples:
                  | holidayTaken |
                  | 12.3         |
                  | 12.345       |
                  | -1           |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
   Scenario Outline:RP14A Display error for holiday days taken outside allowed range
            Given the RP14A contains holiday days taken "<holidayTaken>"
             When I attempt to submit the RP14A
              Then I should see the following validation errors
              | Message                               | Hint                  | Type               |
              | 1 invalid range of holiday days taken | 0 to 365 days allowed | Holiday days taken |
	         And I should be able to view holiday days taken validation error details
        Examples:
                  | holidayTaken |
                  |          366 |
                  |          999 |
                  |           -1 |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug
      Scenario: RP14A Display error for missing holiday owed
            Given the RP14A contains no holiday owed
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing holiday owed"


@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
       Scenario Outline:RP14A Display error for invalid holiday owed format
            Given the RP14A contains holiday owed "<holidayOwed>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
              | Message                 | Hint                           | Type         |
              | 1  invalid holiday owed | Expected format is 28.25 or 33 | Holiday owed |
               And I should be able to view Holiday owed validation error details
        Examples:
                  | holidayOwed |
                  | 12.3        |
                  | 12.345      |
                  | -1          |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
Scenario Outline:RP14A Display error for holiday owed outside allowed range
            Given the RP14A contains holiday owed "<holidayOwed>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
              | Message                          | Hint                  | Type         |
              | 1  invalid range of holiday owed | 0 to 365 days allowed | Holiday owed |
               And I should be able to view Holiday owed validation error details

        Examples:
                  | holidayOwed |
                  |         366 |
                  |          -1 |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug @ignore
 Scenario: RP14A Display error when holiday not paid start date is after end date
            Given the RP14A contains holiday not paid start date after end date
             When I attempt to submit the RP14A
              Then I should see the following validation errors
              | Message                             | Hint                                   | Type |
              | 1 invalid holiday not paid of dates | Start date must be before the end date |      |

  @regression @validation @rp14a @allure.subSuite:EmployeeHoliday
  Scenario: RP14A Display aggregated count for repeated invalid holiday owed errors
            Given the RP14A contains 4 employees with invalid holiday owed
             When I attempt to submit the RP14A
             Then I should see the following multiple validation errors
              | Message                          | Hint                           | Type         |
              | 4  invalid holiday owed          | Expected format is 28.25 or 33 | Holiday owed |
              | 1  invalid range of holiday owed | 0 to 365 days allowed          | Holiday owed |
               And I should be able to view the list of Holiday owed validation error details
                And I should be able to view the invalid range of holiday owed validation error details


 @regression @validation @rp14a @allure.story:Holiday
 Scenario Outline: RP14A displays error for invalid holiday days taken formats for multiple employees
            Given the RP14A contains <count> invalid holiday days taken values "<holidayTaken>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                  | Hint                           | Type               |
                  | <count> holiday days taken are incorrect | Enter a number like 28.5 or 33 | Holiday days taken |
              And I should be able to view holiday days taken for multiple employees error details

        Examples:
                  | count | holidayTaken |
                  | 2     | 12.345       |


@regression @validation @rp14a @allure.story:Holiday
Scenario Outline: RP14A displays error for holiday days carried forward outside allowed range for multiple employees
            Given the RP14A contains <count> employees with invalid holiday days carried forward "<holidayCarriedForward>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                          | Hint                  | Type                    |
                  | <count> invalid range of holiday carried forward | 0 to 365 days allowed | Holiday carried forward |
              And I should be able to view holiday days carried forward validation error details for multiple employees

        Examples:
                  | count | holidayCarriedForward |
                  | 2     | 366                   |
                  | 2     | -1                    |
         