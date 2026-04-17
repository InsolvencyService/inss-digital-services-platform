Feature: SignIn

  As a user
  I want to sign in to the RP14/A service
  So that I can access my account securely

  Background: 
  Given I am on the sign in page

@smoke
Scenario: Successful sign in with valid credentials
  When I provide valid sign in details
  Then I should be on to view the declaration page

@smoke
Scenario: View password while entering sign in details
  Given I provide valid credentials
  When I choose to view my password
  Then I should be able to see the password I entered

@smoke @addVideo
Scenario: Error when email and password are blank
  When I submit the sign in form
  Then I should see the following error messages:
    | Message                |
    | Enter an email address |
    | Enter a password       |

@smoke
Scenario Outline: Sign in validation errors
When I enter "<email>" into the email address field
  And I enter "<password>" into the password field
  And I choose to sign in
Then I should see "<errorMessage>" for the "<field>" field

Examples:
  | email            | password         | errorMessage                                                        | field    |
  | <empty>          | ValidPassword123 | Enter an email address                                              | email    |
  | user@example.com | <empty>          | Enter a password                                                    | password |
  | invalid-email    | ValidPassword123 | Enter an email address in the correct format, like name@example.com | email    |
  | user@example.com | WrongPassword123 | The email address or password you entered is incorrect              | summary  |



@ignore
Scenario: Display error when a locked account attempts to sign in
  Given a locked account exists for "user@example.com"
  When I enter "user@example.com" into the email address field
  And I enter "ValidPassword123" into the password field
  And I choose to sign in
  Then I should see the error message "Your account has been locked"


