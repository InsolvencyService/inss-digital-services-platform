@cleanCosmosDb
Feature: Shareholder Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "InssTestSeventeen" user

    @regression @validation @rp14
    Scenario Outline: RP14 Display error for invalid shareholder percentage
            Given the RP14 XML contains <shareholderCount> shareholder percentages "<percentage>"
             When I attempt to submit the RP14
             Then I should see the following shareholder validation errors
                  | Message        | Hint                             | Type                   |
                  | <errorMessage> | Enter a number like 50.50 or 100 | Shareholder percentage |

        Examples:
                  | shareholderCount | percentage | errorCount | errorMessage                            |
                  |                1 |     50.555 |          1 | 1 shareholder percentage is incorrect   |
                  |                2 |     50.555 |          2 | 2 shareholder percentages are incorrect |
                  |                1 |        -10 |          1 | 1 shareholder percentage is incorrect   |
                  |                2 |        -10 |          2 | 2 shareholder percentages are incorrect |
        @api-upload
        Examples:
                   | shareholderCount | percentage | errorCount | errorMessage                            |
                   |                1 |     50.555 |          1 | 1 shareholder percentage is incorrect   |
                   |                2 |     50.555 |          2 | 2 shareholder percentages are incorrect |
                   |                1 |        -10 |          1 | 1 shareholder percentage is incorrect   |
                   |                2 |        -10 |          2 | 2 shareholder percentages are incorrect |

@regression @validation @rp14 @cleanCosmosDb
Scenario Outline: Accept valid shareholder percentage format
            Given the RP14 XML contains shareholder percentage "<percentage>"
             When I attempt to submit the RP14
             Then no invalid shareholder percentage error should be displayed

        Examples:
                  | percentage |
                  | 50.50      |
                  | 100        |
                  | 0          |
                  | 0.00       |

        @api-upload
        Examples:
                  | percentage |
                  | 50.50      |
                  | 100        |
                  | 0          |
                  | 0.00       |


@regression @validation @rp14 @cleanCosmosDb
Scenario Outline: RP14 shareholder name length boundary validation
            Given the RP14 XML contains a shareholder name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>" 

        Examples:
                  | length | summaryBehaviour                       | detailsBehaviour           | type             |
                  |    100 | none                                   | none                       | Shareholder name |
                  |    101 | 1 shareholder name is the wrong length | Enter up to 100 characters | Shareholder name |

        @api-upload
        Examples:
                  | length | summaryBehaviour                       | detailsBehaviour           | type             |
                  |    100 | none                                   | none                       | Shareholder name |
                  |    101 | 1 shareholder name is the wrong length | Enter up to 100 characters | Shareholder name |


@regression @validation @rp14
 Scenario Outline: RP14 shareholder name length validation for multiple shareholders
            Given the RP14 XML contains <shareholderCount> shareholder names of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<detailsBehaviour>" With "<type>"
        Examples:
                  | shareholderCount | length | errorMessage                             | detailsBehaviour           | type             |
                  |                3 |    101 | 3 shareholder names are the wrong length | Enter up to 100 characters | Shareholder name |

        @api-upload
        Examples:
                  | shareholderCount | length | errorMessage                             | detailsBehaviour           | type             |
                  |                3 |    101 | 3 shareholder names are the wrong length | Enter up to 100 characters | Shareholder name |
