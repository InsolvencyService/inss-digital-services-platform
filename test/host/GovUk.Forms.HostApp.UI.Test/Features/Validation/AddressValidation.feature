@cleanCosmosDb
Feature: AddressValidation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "InssTestFour" user

@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 address line length boundary validation
            Given the RP14 XML contains an address line of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                       | hint                      | type          |
                  |     35 | none                               | none                      | Address lines |
                  |     36 | 1 address line is the wrong length | Enter up to 35 characters | Address lines |

        @api-upload
        Examples:
                  | length | errorMessage                       | hint                      | type          |
                  |     35 | none                               | none                      | Address lines |
                  |     36 | 1 address line is the wrong length | Enter up to 35 characters | Address lines |

@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 address town length boundary validation
            Given the RP14 XML contains an address town of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                       | hintText                  | type         |
                  |     35 | none                               | none                      | Address town |
                  |     36 | 1 address town is the wrong length | Enter up to 35 characters | Address town |

        @api-upload
        Examples:
                  | length | errorMessage                       | hintText                  | type         |
                  |     35 | none                               | none                      | Address town |
                  |     36 | 1 address town is the wrong length | Enter up to 35 characters | Address town |

@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 address county length boundary validation
            Given the RP14 XML contains an address county of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                         | hintText                  | type           |
                  |     35 | none                                 | none                      | Address county |
                  |     36 | 1 address county is the wrong length | Enter up to 35 characters | Address county |

        @api-upload
        Examples:
                  | length | errorMessage                         | hintText                  | type           |
                  |     35 | none                                 | none                      | Address county |
                  |     36 | 1 address county is the wrong length | Enter up to 35 characters | Address county |

@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 address postcode length boundary validation
            Given the RP14 XML contains an address postcode of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                         | hintText                  | type             |
                  |     10 | none                                 | none                      | Address postcode |
                  |     11 | 1 address postcode is the wrong length | Enter up to 10 characters | Address postcode |

        @api-upload
        Examples:
                  | length | errorMessage                             | hintText                  | type             |
                  |     10 | none                                     | none                      | Address postcode |
                  |     11 | 1 address postcode is the wrong length | Enter up to 10 characters | Address postcode |

@regression @validation @rp14 @addVideo @cleanCosmosDb
Scenario Outline: RP14 address line count boundary validation
            Given the RP14 XML contains <lineCount> address lines
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | lineCount | errorMessage | hintText | type          |
                  |         4 | none         | none     | Address lines |

        @api-upload
        Examples:
                  | lineCount | errorMessage                         | hintText                    | type          |
                  |         4 | none                                 | none                        | Address lines |
                  |         5 | 1 address lines are the wrong length | Enter up to 4 address lines | Address lines |
