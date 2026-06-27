@cleanCosmosDb
Feature: Business Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "InssTestSeven" user

   @regression @validation @rp14
    Scenario:RP14 Spreatsheet Display error for missing business name
         Given the RP14 XML contains no business name
         When I attempt to submit the RP14
         Then I should see the validation error "1 business name is missing"

   @regression @validation @rp14 @api-upload
    Scenario:RP14 Api Display error for missing business name
         Given the RP14 XML contains no business name
         When I attempt to submit the RP14
         Then I should see the validation error "1 business name is missing"

   @regression @validation @rp14 @cleanCosmosDb
  Scenario Outline:RP14 business name length boundary validation
            Given the RP14 XML contains a business name of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>" 

        Examples:
                  | length | summaryBehaviour                    | detailsBehaviour          | type             |
                  |     60 | none                                | none                      | Name of business |
                  |     61 | 1 business name is the wrong length | Enter up to 60 characters | Name of business |

        @api-upload
        Examples:
                  | length | summaryBehaviour                    | detailsBehaviour          | type             |
                  |     60 | none                                | none                      | Name of business |
                  |     61 | 1 business name is the wrong length | Enter up to 60 characters | Name of business |

@regression @validation @rp14 @cleanCosmosDb
Scenario Outline: RP14 company number length boundary validation
            Given the RP14 XML contains a company number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                     | detailsBehaviour          | type           |
                  |     12 | none                                 | none                      | Company number |
                  |     13 | 1 company number is the wrong length | Enter up to 12 characters | Company number |

        @api-upload
        Examples:
                  | length | summaryBehaviour                     | detailsBehaviour          | type           |
                  |     12 | none                                 | none                      | Company number |
                  |     13 | 1 company number is the wrong length | Enter up to 12 characters | Company number |


@regression @validation @rp14 @cleanCosmosDb
Scenario Outline: RP14 standard industrial classification length boundary validation
            Given the RP14 XML contains a standard industrial classification of length <length>
            When I attempt to submit the RP14
            Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                                         | detailsBehaviour           | type                               |
                  |    255 | none                                                     | none                       | Standard industrial classification |
                  |    256 | 1 standard industrial classification is the wrong length | Enter up to 255 characters | Standard industrial classification |

        @api-upload
        Examples:
                  | length | summaryBehaviour                                         | detailsBehaviour           | type                               |
                  |    255 | none                                                     | none                       | Standard industrial classification |
                  |    256 | 1 standard industrial classification is the wrong length | Enter up to 255 characters | Standard industrial classification |

