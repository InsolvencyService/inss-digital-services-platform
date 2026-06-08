Feature: Director Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "Admin" user

   @regression @validation @rp14
   Scenario Outline: RP14 Display error for invalid director national insurance number
            Given the RP14 XML contains director national insurance number "<niNumber>"
             When I attempt to submit the RP14A
             Then I should see the following director validation errors
                  | Message                                             | Hint                | Type     |
                  | [1] invalid director national insurance number format | Format is AB112233C | Director |
        Examples:
                  | niNumber  |
                  | 123456789 |
                  | AB11223C  |
                  | QQ112233C |
                  | AB112233Z |
                  | AB11AA33C |

@regression @validation @rp14
Scenario: RP14 Accept valid director national insurance number
          Given the RP14 XML contains director national insurance number "AB112233C"
          When I attempt to submit the RP14A
          Then no invalid director national insurance number format error should be displayed

@regression @validation @rp14
Scenario Outline: RP14 Display error for multiple invalid director national insurance numbers
            Given the RP14 XML contains <directorCount> directors with national insurance number "<niNumber>"
             When I attempt to submit the RP14
             Then I should see the following director validation errors
                  | Message                                                           | Hint                | Type     |
                  | <directorCount> invalid director national insurance number format | Format is AB112233C | Director |

        Examples:
                  | directorCount | niNumber  |
                  | 2             | QQ112233C |
                  | 3             | QQ112233C |


@regression @validation @rp14 
Scenario Outline: RP14 director initials length boundary validation
            Given the RP14 XML contains director initials of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                    | hintText                         | type              |
                  |    100 | none                            | none                             | Director initials |
                  |    101 | 1 too long initials of director | Up to 100 characters are allowed | Director initials |


@regression @validation @rp14 
Scenario Outline: RP14 director surname length boundary validation
            Given the RP14 XML contains director surname of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hintText>" With "<type>"

        Examples:
                  | length | errorMessage                   | hintText                         | type             |
                  |    100 | none                           | none                             | Director surname |
                  |    101 | 1 too long surname of director | Up to 100 characters are allowed | Director surname |

@regression @validation @rp14 @addVideo
Scenario Outline: RP14 multiple director surnames exceeding maximum length
            Given the RP14 XML contains <count> director surnames exceeding the maximum length
             When I attempt to submit the RP14
             Then I should see the following director surnames validation errors
                  | Message                              | Hint                             | Type             |
                  | <count> too long surname of director | Up to 100 characters are allowed | Director surname |

        Examples:
                  | count |
                  | 2     |
                  | 3     |




