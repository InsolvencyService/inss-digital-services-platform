Feature: Insolvency Practitioner Validation

As an Insolvency Practitioner
I want to see the errors from my RP14 upload
So that I know what to change and re-upload

  Background:
   Given I am on the upload page as a "Admin" user

@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner
 Scenario Outline: RP14 insolvency practitioner name length boundary validation
            Given the RP14 XML contains an insolvency practitioner name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                            | hint                            | type                         |
                  |     60 | none                                    | none                            | Insolvency practitioner name |
                  |     61 | 1 too long insolvency practitioner name | Up to 60 characters are allowed | Insolvency practitioner name |

@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner
Scenario Outline: RP14 insolvency practitioner registration number length boundary validation
            Given the RP14 XML contains an insolvency practitioner registration number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage                   | hint                           | type                |
                  |      9 | none                           | none                           | Registration number |
                  |     10 | 1 too long registration number | Up to 9 characters are allowed | Registration number |


@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner
Scenario Outline: RP14 insolvency practitioner firm name length boundary validation
            Given the RP14 XML contains an insolvency practitioner firm name of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage         | hint                             | type      |
                  |    255 | none                 | none                             | Firm name |
                  |    256 | 1 too long firm name | Up to 255 characters are allowed | Firm name |


@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner
Scenario Outline: RP14 insolvency practitioner email length boundary validation
            Given the RP14 XML contains an insolvency practitioner email of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<errorMessage>" with "<hint>" With "<type>"

        Examples:
                  | length | errorMessage     | hint                             | type  |
                  |    100 | none             | none                             | Email |
                  |    101 | 1 too long email | Up to 100 characters are allowed | Email |



@regression @validation @rp14 @addVideo @allure.story:InsolvencyPractitioner
Scenario Outline: RP14 insolvency practitioner phone length boundary validation
            Given the RP14 XML contains an insolvency practitioner phone number of length <length>
             When I attempt to submit the RP14
             Then the error summary should "<summaryBehaviour>" with "<detailsBehaviour>" With "<type>"

        Examples:
                  | length | summaryBehaviour | detailsBehaviour                | type  |
                  | 40     | none             | none                            | Phone |
                  | 41     | 1 too long phone | Up to 40 characters are allowed | Phone |