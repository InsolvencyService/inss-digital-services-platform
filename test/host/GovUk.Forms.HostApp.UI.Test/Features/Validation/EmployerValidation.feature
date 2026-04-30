Feature: Employe Validation

  As an Insolvency Practitioner user
  I want RP14A validation to run before submission to Dynamics
  So that I can fix errors immediately and avoid delayed rejection

Background: 
  Given I am on the upload page

@regression @validation @rp14a
Scenario Outline: Employer name length boundary validation
  Given I have uploaded an RP14A file with employer name of length <length>
  When I submit the RP14A file
  Then the submission should be "<outcome>"
  And the error summary should "<summaryBehaviour>" with "<detailsBehaviour>"
  And I should be able to view error details

Examples:
  | length | outcome  | summaryBehaviour                                | detailsBehaviour                     |
  |     99 | accepted | not contain invalid length of the employer name | not contain Maximum of 99 characters |
  |    100 | rejected | 1 invalid length of the employer name           | Maximum of 99 characters allowed     |
