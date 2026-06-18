@cleanCosmosDb
Feature: Cross Category Validation

As an Insolvency Practitioner
I want to see all validation errors from my RP14 upload
So that I can fix issues across different sections in one go

  Background:
    Given I am on the upload page as a "InssTestNine" user

@regression @validation @rp14 @api-upload @addVideo
Scenario: RP14 Api displays errors from different categories together
    Given the RP14 XML contains the following invalid values
        | caseReference | businessName | directorNationalInsuranceNumber | shareholderPercentage | payRecordsContactName |
        | ABC12345      |              | 00123456C                       | 50.588                |                       |
    When I attempt to submit the RP14
    Then I should see the following validation errors
        | Message                                            | Hint                                                 | Type                               |
        | 1 case reference is in the wrong format            |                                                      | Case reference                     |
        | 1 business name is missing                         |                                                      | Name of business                   |
        | 1 National Insurance number is in the wrong format | Enter a National Insurance number like QQ 12 34 56 C | Director national insurance number |
        | 1 shareholder percentage is incorrect              | Enter a number like 50.50 or 100                     | Shareholder percentage             |
        | 1 pay records contact name is missing              |                                                      | Pay records contact name           |



@regression @validation @rp14 @addVideo
Scenario: RP14 displays correct counts for repeated validation errors
    Given the RP14 XML contains the following invalid values
        | Type                            | Count |
        | DirectorNationalInsuranceNumber | 3     |
        | ShareholderPercentage           | 2     |
        | AddressLine                     | 4     |
        | BusinessName                    | 2     |
    When I submit the RP14 XML
    Then I should see the following validation errors
        | Message                                              | Hint                                                 | Type                               |
        | 1 business name is the wrong length                  | Enter up to 60 characters                           | Name of business                   |
        | 3 address lines are the wrong length                 | Enter up to 35 characters                            | Address lines                      |
        | 3 National Insurance numbers are in the wrong format | Enter a National Insurance number like QQ 12 34 56 C | Director national insurance number |
        | 2 shareholder percentages are incorrect              | Enter a number like 50.50 or 100                     | Shareholder percentage             |
        | 1 associated company name is the wrong length        | Enter up to 60 characters                            | Associated company name            |
