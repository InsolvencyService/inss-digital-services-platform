Feature: AddressValidation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "Admin" user

@regression @validation @rp14 @addVideo
Scenario Outline: RP14 address line length boundary validation
            Given the RP14 XML contains an address line of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage            | hint                            | type          |
                  |     35 | none                    | none                            | Address lines |
                  |     36 | 1 address line too long | Up to 35 characters are allowed | Address lines |

@regression @validation @rp14 @addVideo
Scenario Outline: RP14 address town length boundary validation
            Given the RP14 XML contains an address town of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage            | hintText                        | type         |
                  |     35 | none                    | none                            | Address town |
                  |     36 | 1 address town too long | Up to 35 characters are allowed | Address town |


@regression @validation @rp14 @addVideo
Scenario Outline: RP14 address county length boundary validation
            Given the RP14 XML contains an address county of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage              | hintText                        | type           |
                  |     35 | none                      | none                            | Address county |
                  |     36 | 1 address county too long | Up to 35 characters are allowed | Address county |


@regression @validation @rp14 @addVideo
Scenario Outline: RP14 address postcode length boundary validation
            Given the RP14 XML contains an address postcode of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                | hintText                        | type             |
                  |     10 | none                        | none                            | Address postcode |
                  |     11 | 1 address postcode too long | Up to 10 characters are allowed | Address postcode |



@regression @validation @rp14 @addVideo
Scenario Outline: RP14 address line count boundary validation
            Given the RP14 XML contains <lineCount> address lines
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | lineCount | errorMessage                      | hintText                  | type          |
                  |         4 | none                              | none                      | Address lines |
                  |         5 | 1 too many address lines provided | Up to 4 lines are allowed | Address lines |
