Feature: Business Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "Admin" user

   @regression @validation @rp14
    Scenario:RP14 Display error for missing business name
         Given the RP14 XML contains no business name
         When I attempt to submit the RP14
         Then I should see the validation error "1 missing name of business"

   @regression @validation @rp14
  Scenario Outline:RP14 business name length boundary validation
            Given the RP14 XML contains a business name of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>" 

        Examples:
                  | length | summaryBehaviour            | detailsBehaviour                | type             |
                  |     60 | none                        | none                            | Name of business |
                  |     61 | 1 too long name of business | Up to 60 characters are allowed | Name of business |

@regression @validation @rp14
Scenario Outline: RP14 company number length boundary validation
            Given the RP14 XML contains a company number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour          | detailsBehaviour                | type           |
                  |     12 | none                      | none                            | Company number |
                  |     13 | 1 too long company number | Up to 12 characters are allowed | Company number |


@regression @validation @rp14 @ignore
Scenario Outline: RP14 standard industrial classification length boundary validation
            Given the RP14 XML contains a standard industrial classification of length <length>
            When I attempt to submit the RP14
            Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                              | detailsBehaviour                 |
                  | 255    | none                                          | none                             |
                  | 256    | 1 too long standard industrial classification | Up to 255 characters are allowed |

