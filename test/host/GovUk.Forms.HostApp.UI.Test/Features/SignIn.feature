Feature: SignIn

  As a user
  I want to sign in to the RP14/A service
  So that I can access my account securely

  Background: 
  Given I am on the sign in page

@smoke
Scenario: Successful sign in with valid credentials
  When I sign in with valid credential "user@example.com" "ValidPassword123"
  Then I should be on the declaration page

@smoke
Scenario: View password while entering sign in details
  When I choose to view my password
  Then I should be able to see the password I entered

@smoke
Scenario Outline: Sign in validation errors
  Given I am on the sign in page
  When I enter "<email>" email addesss
  And I enter "<password>" password
  And I click the login button
  Then I should see the error message "<errorMessage>"

Examples:
  | email            | password         | errorMessage                                                        |
  |                  | ValidPassword123 | Enter an email address                                              |
  | user@example.com |                  | Enter a password                                                    |
  | invalid-email    | ValidPassword123 | Enter an email address in the correct format, like name@example.com |
  | user@example.com | WrongPassword123 | The email address or password you entered is incorrect              |


  Scenario: Display error when account is locked
  Given I am on the signin page
  And the account for "user@example.com" is locked
  When I enter "user@example.com" into the email field
  And I enter "ValidPassword123" into the password field
  And I click the signin button
  Then I should see the error message "Your account is locked"


