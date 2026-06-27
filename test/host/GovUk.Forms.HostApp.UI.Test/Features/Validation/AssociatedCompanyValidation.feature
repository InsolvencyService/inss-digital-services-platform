Feature: Associated Company Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "InssTestSix" user


      @regression @validation @rp14 @allure.story:AssociatedCompany @cleanCosmosDb
      Scenario: RP14 displays error for multiple associated company names exceeding maximum length
            Given the RP14 XML contains <associatedCompanyCount> associated companies with name longer than 60 characters
             When I attempt to submit the RP14
             Then I should see the following associated company validation errors
                  | Message                                                                | Hint                      | Type                    |
                  | <associatedCompanyCount> associated company names are the wrong length | Enter up to 60 characters | Associated company name |

        Examples:
                  | associatedCompanyCount |
                  | 2                      |

        @api-upload
        Examples:
                  | associatedCompanyCount |
                  | 2                      |

@regression @validation @rp14 @allure.story:AssociatedCompany @cleanCosmosDb
Scenario Outline: RP14 associated company name length boundary validation
            Given the RP14 XML contains an associated company name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                              | detailsBehaviour          | type                    |
                  |     60 | none                                          | none                      | Associated company name |
                  |     61 | 1 associated company name is the wrong length | Enter up to 60 characters | Associated company name |

        @api-upload
        Examples:
                  | length | summaryBehaviour                              | detailsBehaviour          | type                    |
                  |     60 | none                                          | none                      | Associated company name |
                  |     61 | 1 associated company name is the wrong length | Enter up to 60 characters | Associated company name |

    @regression @validation @rp14 @allure.story:AssociatedCompany @api-upload @cleanCosmosDb
      Scenario: RP14 displays error for multiple reasons for association exceeding maximum length
            Given the RP14 XML contains <associatedCompanyCount> associated companies with 256 characters
             When I attempt to submit the RP14
             Then I should see the following associated company validation errors
                  | Message        | Hint                       | Type                   |
                  | <errorMessage> | Enter up to 255 characters | Reason for association |

        Examples:
                  | associatedCompanyCount | errorMessage                                   |
                  |                      1 | 1 reason for association is the wrong length   |
                  |                      2 | 2 reason for associations are the wrong length |


   @regression @validation @rp14  @allure.story:AssociatedCompany @cleanCosmosDb
   Scenario Outline: RP14 associated company number length boundary validation
            Given the RP14 XML contains an associated company number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                    | hint                     | type                      |
                  |      9 | none                                            | none                     | Associated company number |
                  |     10 | 1 associated company number is the wrong length | Enter up to 9 characters | Associated company number |

        @api-upload
        Examples:
                  | length | errorMessage                                  | hint                       | type                      |
                  |      9 | none                                          | none                       | Associated company number |
                  |     10 | 1 associated company number is the wrong length | Enter up to 9 characters | Associated company number |

@regression @validation @rp14 @allure.story:AssociatedCompany @cleanCosmosDb
   Scenario Outline: RP14 multiple associated company numbers exceeding maximum length
            Given the RP14 XML contains <count> associated company numbers exceeding the maximum length
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | count | errorMessage                                    | hint                       | type                      |
                  |     2 | 2 associated company numbers are the wrong length | Enter up to 9 characters | Associated company number |

        @api-upload
        Examples:
                  | count | errorMessage                                    | hint                       | type                      |
                  |     2 | 2 associated company numbers are the wrong length | Enter up to 9 characters | Associated company number |


  @regression @validation @rp14 @allure.story:AssociatedCompany @api-upload @cleanCosmosDb
   Scenario: RP14 displays multiple associated company validation errors
            Given the RP14 XML contains 2 associated companies with all fields exceeding maximum length
             When I attempt to submit the RP14
             Then I should see the following associated company validation errors
                  | Message                                           | Hint                       | Type                      |
                  | 2 associated company names are the wrong length   | Enter up to 60 characters  | Associated company name   |
                  | 2 associated company numbers are the wrong length | Enter up to 9 characters   | Associated company number |
                  | 2 reason for associations are the wrong length    | Enter up to 255 characters | Reason for association    |


   @regression @validation @rp14 @cleanCosmosDb
   Scenario Outline: RP14 employment continuity employer name length boundary validation
            Given the RP14 XML contains an employment continuity employer name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                        | hint                      | type          |
                  |     60 | none                                | none                      | Employer name |
                  |     61 | 1 employer name is the wrong length | Enter up to 60 characters | Employer name |

        @api-upload
        Examples:
                  | length | errorMessage                        | hint                      | type          |
                  |     60 | none                                | none                      | Employer name |
                  |     61 | 1 employer name is the wrong length | Enter up to 60 characters | Employer name |


@regression @validation @rp14 @cleanCosmosDb
Scenario Outline: RP14 transfer to name length boundary validation
            Given the RP14 XML contains a transfer to name of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                        | detailsBehaviour          | type             |
                  |     61 | 1 transfer to name is the wrong length  | Enter up to 60 characters | Transfer to name |

        @api-upload
        Examples:
                  | length | summaryBehaviour                        | detailsBehaviour          | type             |
                  |     61 | 1 transfer to name is the wrong length  | Enter up to 60 characters | Transfer to name |



@regression @validation @rp14 @cleanCosmosDb
Scenario Outline:Happy Path RP14 transfer to name length boundary validation
            Given the RP14 XML contains a transfer to name of length <length>
             When  I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                        | detailsBehaviour          | type             |
                  |     60 | none                                    | none                      | Transfer to name |

        @api-upload
        Examples:
                  | length | summaryBehaviour                        | detailsBehaviour          | type             |
                  |     60 | none                                    | none                      | Transfer to name |
