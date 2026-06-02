Feature: Pay Records Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "Admin" user

@regression @validation @rp14 @addVideo
  Scenario: RP14 displays error for missing pay records contact name
         Given the RP14 XML does not contain Pay records contact name
         When I attempt to submit the RP14
         Then I should see the following pay records contact validation errors
            | Message                              | Hint | Type                     |
            | 1 missing a Pay records contact name |      | Pay records contact name |


@regression @validation @rp14 @addVideo
Scenario Outline: RP14 pay records contact name length boundary validation
            Given the RP14 XML contains a Pay records contact name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                        | hint                            | type                     |
                  |     60 | none                                | none                            | Pay records contact name |
                  |     61 | 1 too long Pay records contact name | Up to 60 characters are allowed | Pay records contact name |


@regression @validation @rp14 @addVideo
Scenario Outline: RP14 pay records contact phone number length boundary validation
            Given the RP14 XML contains a Pay records contact phone number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                | hint                            | type                             |
                  |     12 | none                                        | none                            | Pay records contact phone number |
                  |     13 | 1 too long Pay records contact phone number | Up to 12 characters are allowed | Pay records contact phone number |


@regression @validation @rp14 @addVideo
Scenario Outline: RP14 pay records contact email address length boundary validation
            Given the RP14 XML contains a Pay records contact email address of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                 | hint                             | type                              |
                  |    100 | none                                         | none                             | Pay records contact email address |
                  |    101 | 1 too long Pay records contact email address | Up to 100 characters are allowed | Pay records contact email address |
