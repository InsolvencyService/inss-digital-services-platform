Feature: Case Validation

  As an Insolvency Practitioner user
  I want RP14A validation to run before submission to Dynamics
  So that I can fix errors immediately and avoid delayed rejection

Background: 
  Given I am on the upload page

@regression
Scenario:RP14A Show validation error when case reference is missing
  Given the RP14A contains an employee row with no case reference
  When I attempt to submit the RP14A
  Then I should see the validation error "1 missing a case reference"

  @regression
  Scenario: RP14A Show validation error when case reference format is invalid
  Given the RP14A contains a case reference "AB12345678"
  When I attempt to submit the RP14A
  Then I should see the validation error "1 invalid case reference format" with the hint "Format is CN12345678"

  @regression
  Scenario:RP14A display error for case reference longer than 12 characters
  Given the RP14A contains a case reference "CN12345678901"
  When I attempt to submit the RP14A
  Then I should see the validation error "1 too long case reference" with the hint "Up to 12 characters are allowed"
 
 
@regression @ignore
Scenario: Display error when case reference is not found in RPS
  Given the RP14A contains a valid format case reference
  And the case reference does not exist in RPS
  When I attempt to submit the RP14A
  Then I should see the validation error "[COUNT] case reference have not been matched in our system"

