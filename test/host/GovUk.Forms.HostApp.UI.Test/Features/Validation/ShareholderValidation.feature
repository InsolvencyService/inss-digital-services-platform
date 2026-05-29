Feature: Shareholder Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "Admin" user

    @regression @validation @rp14
    Scenario Outline: RP14 Display error for invalid shareholder percentage
            Given the RP14 XML contains <shareholderCount> shareholder percentages "<percentage>"
             When I attempt to submit the RP14
             Then I should see the following shareholder validation errors
                  | Message                                     | Hint                            | Type                   |
                  | <errorCount> invalid shareholder percentage | Expected format is 50.50 or 100 | Shareholder percentage |

        Examples:
                  | shareholderCount | percentage | errorCount |
                  | 1                | 50.5       | 1          |
                  | 3                | 50.5       | 3          |
                  | 1                | 50.555     | 1          |
                  | 2                | 50.555     | 2          |
                  | 1                | -10        | 1          |
                  | 2                | -10        | 2          |

@regression @validation @rp14
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


@regression @validation @rp14 @bug
Scenario Outline: RP14 shareholder name length boundary validation
            Given the RP14 XML contains a shareholder name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>" 

        Examples:
                  | length | summaryBehaviour               | detailsBehaviour                 | type             |
                  |    100 | none                           | none                             | Shareholder name |
                  |    101 | 1 too long name of shareholder | Up to 100 characters are allowed | Shareholder name |


@regression @validation @rp14 
 Scenario Outline: RP14 shareholder name length validation for multiple shareholders
            Given the RP14 XML contains <shareholderCount> shareholder names of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<detailsBehaviour>" With "<type>"
        Examples:
                  | shareholderCount | length | errorMessage                   | detailsBehaviour                 | type             |
                  |                3 |    101 | 3 too long name of shareholder | Up to 100 characters are allowed | Shareholder name |
