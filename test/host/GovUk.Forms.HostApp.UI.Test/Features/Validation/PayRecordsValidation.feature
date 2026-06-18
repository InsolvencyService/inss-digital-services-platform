@cleanCosmosDb
Feature: Pay Records Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "InssTestSixteen" user

@regression @validation @rp14 @addVideo
  Scenario: RP14 displays error for missing pay records contact name
         Given the RP14 XML does not contain Pay records contact name
         When I attempt to submit the RP14
         Then I should see the following pay records contact validation errors
            | Message                              | Hint | Type                     |
            |1 pay records contact name is missing |      | Pay records contact name |

@regression @validation @rp14 @addVideo @api-upload
  Scenario: RP14 Api displays error for missing pay records contact name
         Given the RP14 XML does not contain Pay records contact name
         When I attempt to submit the RP14
         Then I should see the following pay records contact validation errors
            | Message                               | Hint | Type                     |
            | 1 pay records contact name is missing |      | Pay records contact name |


@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 pay records contact name length boundary validation
            Given the RP14 XML contains a Pay records contact name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                   | hint                      | type                     |
                  |     60 | none                                           | none                      | Pay records contact name |
                  |     61 | 1 pay records contact name is the wrong length | Enter up to 60 characters | Pay records contact name |

        @api-upload
        Examples:
                  | length | errorMessage                                   | hint                      | type                     |
                  |     60 | none                                           | none                      | Pay records contact name |
                  |     61 | 1 pay records contact name is the wrong length | Enter up to 60 characters | Pay records contact name |


@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 pay records contact phone number length boundary validation
            Given the RP14 XML contains a Pay records contact phone number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                           | hint                      | type                             |
                  |     12 | none                                                   | none                      | Pay records contact phone number |
                  |     13 | 1 pay records contact phone number is the wrong length | Enter up to 12 characters | Pay records contact phone number |

        @api-upload
        Examples:
                  | length | errorMessage                                           | hint                      | type                             |
                  |     12 | none                                                   | none                      | Pay records contact phone number |
                  |     13 | 1 pay records contact phone number is the wrong length | Enter up to 12 characters | Pay records contact phone number |


@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 pay records contact email address length boundary validation
            Given the RP14 XML contains a Pay records contact email address of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                            | hint                       | type                              |
                  |    100 | none                                                    | none                       | Pay records contact email address |
                  |    101 | 1 pay records contact email address is the wrong length | Enter up to 100 characters | Pay records contact email address |

        @api-upload
        Examples:
                  | length | errorMessage                                            | hint                       | type                              |
                  |    100 | none                                                    | none                       | Pay records contact email address |
                  |    101 | 1 pay records contact email address is the wrong length | Enter up to 100 characters | Pay records contact email address |
