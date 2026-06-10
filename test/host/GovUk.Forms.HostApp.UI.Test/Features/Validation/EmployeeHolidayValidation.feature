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

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug @api-upload
 Scenario: RP14A API display error for missing contracted holiday entitlement
            Given the RP14A contains no contracted holiday entitlement
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing contracted holiday entitlement"

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
 Scenario Outline: RP14A Display error for invalid contracted holiday entitlement format
            Given the RP14A contains contracted holiday entitlement "<contractedHoliday>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                       | Hint                           | Type                           |
                  | 1 contracted holiday entitlement is incorrect | Enter a number like 22.5 or 33 | Contracted holiday entitlement |
               And I should be able to view contracted holiday entitlement validation error details

        Examples:
                  | contractedHoliday |
                  | 28.255            |
                  | -1                |

        @api-upload
        Examples:
                  | contractedHoliday |
                  | 28.255            |
                  | -1                |

  @regression @validation @rp14a @allure.subSuite:EmployeeHoliday
   Scenario Outline:RP14A Display error for contracted holiday entitlement outside allowed range
            Given the RP14A contains contracted holiday entitlement "<contractedHoliday>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                                       | Hint                            | Type                           |
                  | 1 contracted holiday entitlement is outside the allowed range | Enter a value between 0 and 365 | Contracted holiday entitlement |
               And I should be able to view contracted holiday entitlement validation error details

        Examples:
                  | contractedHoliday |
                  |               366 |
                  |               999 |
                  |                -1 |

        @api-upload
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

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug @api-upload
Scenario: RP14A API display error for missing holiday days carried forward
            Given the RP14A contains no holiday days carried forward
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing holiday days carried forward"

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
 Scenario Outline: RP14A Display error for invalid holiday days carried forward format
            Given the RP14A contains holiday days carried forward "<holidayCarriedForward>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                      | Hint                           | Type                    |
                  | 1  carried forward holiday days is incorrect | Enter a number like 22.5 or 33 | Holiday carried forward |
               And I should be able to view holiday days carried forward validation error details

        Examples:
                  | holidayCarriedForward |
                  | 28.345                |
                  | -1                    |

        @api-upload
        Examples:
                  | holidayCarriedForward |
                  | 28.345                |
                  | -1                    |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
 Scenario Outline:RP14A Display error for holiday days carried forward outside allowed range
            Given the RP14A contains holiday days carried forward "<holidayCarriedForward>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                                     | Hint                            | Type                    |
                  | 1 carried forward holiday days is outside the allowed range | Enter a value between 0 and 365 | Holiday carried forward |
               And I should be able to view holiday days carried forward validation error details
        Examples:
                  | holidayCarriedForward |
                  |                   366 |
                  |                   999 |
                  |                    -1 |

        @api-upload
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

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug @api-upload
  Scenario:RP14A API display error for missing holiday days taken
            Given the RP14A contains no holiday days taken
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing holiday days taken"

  @regression @validation @rp14a @allure.subSuite:EmployeeHoliday
  Scenario Outline: RP14A Display error for invalid holiday days taken format
            Given the RP14A contains holiday days taken "<holidayTaken>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
              | Message                           | Hint                           | Type               |
              | 1 holiday days taken is incorrect | Enter a number like 22.5 or 33 | Holiday days taken |
	         And I should be able to view holiday days taken validation error details
        Examples:
                  | holidayTaken |
                  | 12.345       |
                  | -1           |

        @api-upload
        Examples:
                  | holidayTaken |
                  | 12.345       |
                  | -1           |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
   Scenario Outline:RP14A Display error for holiday days taken outside allowed range
            Given the RP14A contains holiday days taken "<holidayTaken>"
             When I attempt to submit the RP14A
              Then I should see the following validation errors
              | Message                                           | Hint                            | Type               |
              | 1 holiday days taken is outside the allowed range | Enter a value between 0 and 365 | Holiday days taken |
	         And I should be able to view holiday days taken validation error details
        Examples:
                  | holidayTaken |
                  |          366 |
                  |          999 |
                  |           -1 |

        @api-upload
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

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @bug @api-upload
      Scenario: RP14A API display error for missing holiday owed
            Given the RP14A contains no holiday owed
             When I attempt to submit the RP14A
             Then I should see the validation error "1 missing holiday owed"


@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
       Scenario Outline:RP14A Display error for invalid holiday owed format
            Given the RP14A contains holiday owed "<holidayOwed>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
              | Message                     | Hint                            | Type         |
              | 1 holiday owed is incorrect | Enter a number like 28.25 or 33 | Holiday owed |
               And I should be able to view Holiday owed validation error details
        Examples:
                  | holidayOwed |
                  | 12.345      |
                  | -1          |

        @api-upload
        Examples:
                  | holidayOwed |
                  | 12.345      |
                  | -1          |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
Scenario Outline:RP14A Display error for holiday owed outside allowed range
            Given the RP14A contains holiday owed "<holidayOwed>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
              | Message                                     | Hint                            | Type         |
              | 1 holiday owed is outside the allowed range | Enter a value between 0 and 365 | Holiday owed |
               And I should be able to view Holiday owed validation error details

        Examples:
                  | holidayOwed |
                  |         366 |
                  |          -1 |

        @api-upload
        Examples:
                  | holidayOwed |
                  |         366 |
                  |          -1 |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
 Scenario: RP14A Display error when holiday not paid start date is after end date
            Given the RP14A contains holiday not paid start date after end date
             When I attempt to submit the RP14A
              Then I should see the following validation errors
              | Message                             | Hint                                   | Type |
              | 1 unpaid holiday dates is incorrect | Start date must be before the end date |      |

  @regression @validation @rp14a @allure.subSuite:EmployeeHoliday
  Scenario: RP14A Display aggregated count for repeated invalid holiday owed errors
            Given the RP14A contains 4 employees with invalid holiday owed
             When I attempt to submit the RP14A
             Then I should see the following multiple validation errors
              | Message                                     | Hint                             | Type         |
              | 4 holiday owed is incorrect                 | Enter a number like 28.25 or 33  | Holiday owed |
              | 1 holiday owed is outside the allowed range | Enter a value between 0 and 365  | Holiday owed |
              And I should be able to view the list of Holiday owed validation error details
              And I should be able to view the invalid range of holiday owed validation error details

  @regression @validation @rp14a @allure.subSuite:EmployeeHoliday @api-upload
  Scenario: RP14A API display aggregated count for repeated invalid holiday owed errors
            Given the RP14A contains 4 employees with invalid holiday owed
             When I attempt to submit the RP14A
             Then I should see the following multiple validation errors
              | Message                                     | Hint                             | Type         |
              | 4 holiday owed is incorrect                 | Enter a number like 28.25 or 33  | Holiday owed |
              | 1 holiday owed is outside the allowed range | Enter a value between 0 and 365  | Holiday owed |
              And I should be able to view the list of Holiday owed validation error details
              And I should be able to view the invalid range of holiday owed validation error details


 @regression @validation @rp14a @allure.story:Holiday
 Scenario Outline: RP14A displays error for invalid holiday days taken formats for multiple employees
            Given the RP14A contains <count> invalid holiday days taken values "<holidayTaken>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                  | Hint                           | Type               |
                  | <count> holiday days taken are incorrect | Enter a number like 22.5 or 33 | Holiday days taken |
              And I should be able to view holiday days taken for multiple employees error details

        Examples:
                  | count | holidayTaken |
                  | 2     | 12.345       |

        @api-upload
        Examples:
                  | count | holidayTaken |
                  | 2     | 12.345       |


@regression @validation @rp14a @allure.story:Holiday
Scenario Outline: RP14A displays error for holiday days carried forward incorrect format for multiple employees
            Given the RP14A contains <count> employees with invalid holiday days carried forward "<holidayCarriedForward>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                            | Hint                           | Type                    |
                  | <count> carried forward holiday days are incorrect | Enter a number like 22.5 or 33 | Holiday carried forward |
              And I should be able to view holiday days carried forward validation error details for multiple employees

        Examples:
                  | count | holidayCarriedForward |
                  | 2     | 28.345                |

        @api-upload
        Examples:
                  | count | holidayCarriedForward |
                  | 2     | 28.345                |

@regression @validation @rp14a @allure.story:Holiday
Scenario Outline: RP14A displays error for holiday days carried forward outside allowed range for multiple employees
            Given the RP14A contains <count> employees with invalid holiday days carried forward "<holidayCarriedForward>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                                          | Hint                            | Type                    |
                  | <count> carried forward holiday days are outside the allowed range | Enter a value between 0 and 365 | Holiday carried forward |
              And I should be able to view holiday days carried forward validation error details for multiple employees

        Examples:
                  | count | holidayCarriedForward |
                  | 2     | 366                   |

        @api-upload
        Examples:
                  | count | holidayCarriedForward |
                  | 2     | 366                   |

@regression @validation @rp14a @allure.story:Holiday
Scenario Outline: RP14A displays error for holiday days taken outside allowed range for multiple employees
            Given the RP14A contains <count> invalid holiday days taken values "<holidayTaken>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                                   | Hint                            | Type               |
                  | <count> holiday days taken are outside the allowed range  | Enter a value between 0 and 365 | Holiday days taken |
              And I should be able to view holiday days taken for multiple employees error details

        Examples:
                  | count | holidayTaken |
                  | 2     | 366          |

        @api-upload
        Examples:
                  | count | holidayTaken |
                  | 2     | 366          |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
Scenario Outline: RP14A displays error for invalid contracted holiday entitlement formats for multiple employees
            Given the RP14A contains <count> employees with contracted holiday entitlement "<contractedHoliday>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                                | Hint                           | Type                           |
                  | <count> contracted holiday entitlements are incorrect  | Enter a number like 22.5 or 33 | Contracted holiday entitlement |
              And I should be able to view contracted holiday entitlement validation error details for multiple employees

        Examples:
                  | count | contractedHoliday |
                  | 2     | 28.255            |

        @api-upload
        Examples:
                  | count | contractedHoliday |
                  | 2     | 28.255            |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
Scenario Outline: RP14A displays error for contracted holiday entitlement outside allowed range for multiple employees
            Given the RP14A contains <count> employees with contracted holiday entitlement "<contractedHoliday>"
             When I attempt to submit the RP14A
             Then I should see the following validation errors
                  | Message                                                                   | Hint                            | Type                           |
                  | <count> contracted holiday entitlements are outside the allowed range      | Enter a value between 0 and 365 | Contracted holiday entitlement |
              And I should be able to view contracted holiday entitlement validation error details for multiple employees

        Examples:
                  | count | contractedHoliday |
                  | 2     | 366               |

        @api-upload
        Examples:
                  | count | contractedHoliday |
                  | 2     | 366               |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday
Scenario: RP14A Display aggregated count for repeated invalid holiday not paid date errors
            Given the RP14A contains 2 employees with holiday not paid start date after end date
             When I attempt to submit the RP14A
              Then I should see the following validation errors
              | Message                              | Hint                                   | Type |
              | 2 unpaid holiday dates are incorrect | Start date must be before the end date |      |

@regression @validation @rp14a @allure.subSuite:EmployeeHoliday @api-upload
Scenario: RP14A API display aggregated count for repeated invalid holiday not paid date errors
            Given the RP14A contains 2 employees with holiday not paid start date after end date
             When I attempt to submit the RP14A
              Then I should see the following validation errors
              | Message                              | Hint                                   | Type |
              | 2 unpaid holiday dates are incorrect | Start date must be before the end date |      |
