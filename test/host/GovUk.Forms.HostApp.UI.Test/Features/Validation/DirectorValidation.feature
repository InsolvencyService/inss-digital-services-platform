@cleanCosmosDb
Feature: Director Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "InssTestTen" user

   @regression @validation @rp14
   Scenario Outline: RP14 Display error for invalid director national insurance number
            Given the RP14 XML contains director national insurance number "<niNumber>"
             When I attempt to submit the RP14
             Then I should see the following director validation errors
                  | Message                                            | Hint                                                 | Type                               |
                  | 1 National Insurance number is in the wrong format | Enter a National Insurance number like QQ 12 34 56 C | Director national insurance number |
        Examples:
                  | niNumber  |
                  | 123456789 |
                  | AB11223C  |
                  | QQ112233C |
                  | AB11AA33C |

        @api-upload
        Examples:
                  | niNumber  |
                  | 123456789 |
                  | AB11223C  |
                  | QQ112233C |
                  | AB11AA33C |

@regression @validation @rp14
Scenario: RP14 Spreatsheet Accept valid director national insurance number
          Given the RP14 XML contains director national insurance number "AB112233C"
          When I attempt to submit the RP14
          Then no invalid director national insurance number format error should be displayed

@regression @validation @rp14 @api-upload
Scenario: RP14 Api Accept valid director national insurance number
          Given the RP14 XML contains director national insurance number "AB112233C"
          When I attempt to submit the RP14
          Then no invalid director national insurance number format error should be displayed

@regression @validation @rp14
Scenario Outline: RP14 Display error for multiple invalid director national insurance numbers
            Given the RP14 XML contains <directorCount> directors with national insurance number "<niNumber>"
             When I attempt to submit the RP14
             Then I should see the following director validation errors
                  | Message                                                            | Hint                                                 | Type                               |
                  | <directorCount> National Insurance numbers are in the wrong format | Enter a National Insurance number like QQ 12 34 56 C | Director national insurance number |

        Examples:
                  | directorCount | niNumber  |
                  | 3             | QQ112233C |

        @api-upload
        Examples:
                  | directorCount | niNumber  |
                  | 2             | QQ112233C |



@regression @validation @rp14 @cleanCosmosDb
Scenario Outline: RP14 director initials length boundary validation
            Given the RP14 XML contains director initials of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                             | hintText                   | type              |
                  |    100 | none                                     | none                       | Director initials |
                  |    101 | 1 director initials are the wrong length | Enter up to 100 characters | Director initials |

        @api-upload
        Examples:
                  | length | errorMessage                             | hintText                   | type              |
                  |    100 | none                                     | none                       | Director initials |
                  |    101 | 1 director initials are the wrong length | Enter up to 100 characters | Director initials |


@regression @validation @rp14 @cleanCosmosDb @addVideo
Scenario Outline: RP14 director surname length boundary validation
            Given the RP14 XML contains director surname of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                           | hintText                   | type             |
                  |    100 | none                                   | none                       | Director surname |
                  |    101 | 1 director surname is the wrong length | Enter up to 100 characters | Director surname |

       
@regression @validation @rp14 @cleanCosmosDb @addVideo @api-upload
Scenario Outline: RP14 Api director surname length boundary validation
            Given the RP14 XML contains director surname of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                           | hintText                   | type             |
                  |    100 | none                                   | none                       | Director surname |
                  |    101 | 1 director surname is the wrong length | Enter up to 100 characters | Director surname |

@regression @validation @rp14 @addVideo
Scenario Outline: RP14 multiple director surnames exceeding maximum length
            Given the RP14 XML contains <count> director surnames exceeding the maximum length
             When I attempt to submit the RP14
             Then I should see the following director surnames validation errors
                  | Message                                       | Hint                       | Type             |
                  | <count> director surname are the wrong length | Enter up to 100 characters | Director surname |

        Examples:
                  | count |
                  | 2     |
                  | 3     |

        @api-upload
        Examples:
                  | count |
                  | 2     |
                  | 3     |




