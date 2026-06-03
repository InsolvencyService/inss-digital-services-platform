Feature: Associated Company Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "Admin" user


      @regression @validation @rp14 @allure.story:AssociatedCompany 
      Scenario: RP14 displays error for multiple associated company names exceeding maximum length
            Given the RP14 XML contains <associatedCompanyCount> associated companies with name longer than 60 characters
             When I attempt to submit the RP14
             Then I should see the following associated company validation errors
                  | Message                                                      | Hint                            | Type                    |
                  | <associatedCompanyCount> too long name of associated company | Up to 60 characters are allowed | Associated company name |

        Examples:
                  | associatedCompanyCount |
                  | 2                      |

@regression @validation @rp14 @allure.story:AssociatedCompany
Scenario Outline: RP14 associated company name length boundary validation
            Given the RP14 XML contains an associated company name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                      | detailsBehaviour                | type                    |
                  | 60     | none                                  | none                            | Associated company name |
                  | 61     | 1 too long name of associated company | Up to 60 characters are allowed | Associated company name |

    @regression @validation @rp14 @allure.story:AssociatedCompany @ignore
      Scenario: RP14 displays error for multiple reasons for association exceeding maximum length
            Given the RP14 XML contains <associatedCompanyCount> associated companies with 255 characters
             When I attempt to submit the RP14
             Then I should see the following associated company validation errors
                  | Message                                                  | Hint                             | Type                   |
                  | <associatedCompanyCount> too long reason for association | Up to 255 characters are allowed | Reason for association |

        Examples:
                  | associatedCompanyCount |
                  | 2                      |
                  | 3                      |

   
   @regression @validation @rp14  @allure.story:AssociatedCompany
   Scenario Outline: RP14 associated company number length boundary validation
            Given the RP14 XML contains an associated company number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                            | hint                           | type                      |
                  |      9 | none                                    | none                           | Associated company number |
                  |     10 | 1 too long number of associated company | Up to 9 characters are allowed | Associated company number |

   @regression @validation @rp14 @allure.story:AssociatedCompany
   Scenario Outline: RP14 multiple associated company numbers exceeding maximum length
            Given the RP14 XML contains <count> associated company numbers exceeding the maximum length
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
	| count | errorMessage                            | hint                           | type                      |
	|     2 | 2 too long number of associated company | Up to 9 characters are allowed | Associated company number |


  @regression @validation @rp14 @allure.story:AssociatedCompany @ignore
   Scenario: RP14 displays multiple associated company validation errors
            Given the RP14 XML contains 2 associated companies with name longer than 60 characters
              And the RP14 XML contains 2 associated companies with reason for association longer than 255 characters
              And the RP14 XML contains 2 associated companies with company number longer than 9 characters
             When I attempt to submit the RP14
             Then I should see the following associated company validation errors
                  | errorMessage                          | hint                             | type                      |
                  | 2 too long name of associated company | Up to 60 characters are allowed  | Associated company name   |
                  | 2 too long reason for association     | Up to 255 characters are allowed | Reason for association    |
                  | 2 too long associated company number  | Up to 9 characters are allowed   | Associated company number |


   @regression @validation @rp14
   Scenario Outline: RP14 employment continuity employer name length boundary validation
            Given the RP14 XML contains an employment continuity employer name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage             | hint                            | type          |
                  |     60 | none                     | none                            | Employer name |
                  |     61 | 1 too long employer name | Up to 60 characters are allowed | Employer name |


@regression @validation @rp14
Scenario Outline: RP14 transfer to name length boundary validation
            Given the RP14 XML contains a transfer to name of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour            | detailsBehaviour                | type             |
                  | 60     | none                        | none                            | Transfer to name |
                  | 61     | 1 too long transfer to name | Up to 60 characters are allowed | Transfer to name |
      
