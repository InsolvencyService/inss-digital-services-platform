@cleanCosmosDb
Feature: Insolvency Practitioner Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "InssTestFifteen" user

@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner @cleanCosmosDb
 Scenario Outline: RP14 insolvency practitioner name length boundary validation
            Given the RP14 XML contains an insolvency practitioner name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                       | hint                      | type                         |
                  |     60 | none                                               | none                      | Insolvency practitioner name |
                  |     61 | 1 insolvency practitioner name is the wrong length | Enter up to 60 characters | Insolvency practitioner name |

        @api-upload
        Examples:
                  | length | errorMessage                                       | hint                      | type                         |
                  |     60 | none                                               | none                      | Insolvency practitioner name |
                  |     61 | 1 insolvency practitioner name is the wrong length | Enter up to 60 characters | Insolvency practitioner name |

@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner @cleanCosmosDb
Scenario Outline: RP14 insolvency practitioner registration number length boundary validation
            Given the RP14 XML contains an insolvency practitioner registration number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                              | hint                     | type                |
                  |      9 | none                                      | none                     | Registration number |
                  |     10 | 1 registration number is the wrong length | Enter up to 9 characters | Registration number |

        @api-upload
        Examples:
                  | length | errorMessage                              | hint                     | type                |
                  |      9 | none                                      | none                     | Registration number |
                  |     10 | 1 registration number is the wrong length | Enter up to 9 characters | Registration number |


@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner @cleanCosmosDb
Scenario Outline: RP14 insolvency practitioner firm name length boundary validation
            Given the RP14 XML contains an insolvency practitioner firm name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                    | hint                       | type      |
                  |    255 | none                            | none                       | Firm name |
                  |    256 | 1 firm name is the wrong length | Enter up to 255 characters | Firm name |

        @api-upload
        Examples:
                  | length | errorMessage                    | hint                       | type      |
                  |    255 | none                            | none                       | Firm name |
                  |    256 | 1 firm name is the wrong length | Enter up to 255 characters | Firm name |


@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner @cleanCosmosDb
Scenario Outline: RP14 insolvency practitioner email length boundary validation
            Given the RP14 XML contains an insolvency practitioner email of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                                                | hint                       | type                                    |
                  |    100 | none                                                        | none                       | Insolvency practitioner email address   |
                  |    101 | 1 insolvency practitioner email address is the wrong length | Enter up to 100 characters | Insolvency practitioner email address   |

        @api-upload
        Examples:
                  | length | errorMessage                                                | hint                       | type                                    |
                  |    100 | none                                                        | none                       | Insolvency practitioner email address   |
                  |    101 | 1 insolvency practitioner email address is the wrong length | Enter up to 100 characters | Insolvency practitioner email address   |


@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner @cleanCosmosDb
Scenario Outline: RP14 insolvency practitioner phone length boundary validation
            Given the RP14 XML contains an insolvency practitioner phone number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour                                           | detailsBehaviour          | type                                 |
                  |     12 | none                                                       | none                      | Insolvency practitioner phone number |
                  |     13 | 1 insolvency practitioner phone number is the wrong length | Enter up to 12 characters | Insolvency practitioner phone number |

        @api-upload
        Examples:
                  | length | summaryBehaviour                                           | detailsBehaviour          | type                                 |
                  |     12 | none                                                       | none                      | Insolvency practitioner phone number |
                  |     13 | 1 insolvency practitioner phone number is the wrong length | Enter up to 12 characters | Insolvency practitioner phone number |