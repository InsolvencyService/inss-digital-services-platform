@MEDS-1062
Feature: SignIn

An an Insolvency Practitioner
I want to sign in to IPUS
So I can upload an RP14/14A

 Background: 
 Given I am on the sign in page

@functional
Scenario: Successful sign in with valid credentials
  When I provide valid sign in details
  Then I should be on to view the declaration page

@functional
Scenario: View password while entering sign in details
  Given I provide valid credentials
  When I choose to view my password
  Then I should be able to see the password I entered

@smoke @addVideo @functional
Scenario: Error when email and password are blank
  When I submit the sign in form
  Then I should see the following error messages:
    | Message                |
    | Enter an email address |
    | Enter a password       |
   And I should be able to return to the start page

@smoke
Scenario Outline: Sign in validation errors
When I enter "<email>" into the email address field
  And I enter "<password>" into the password field
  And I choose to sign in
Then I should see "<errorMessage>" for the "<field>" field

Examples:
  | email            | password         | errorMessage                                                  | field    |
  | <empty>          | ValidPassword123 | Enter an email address                                        | email    |
  | user@example.com | <empty>          | Enter a password                                              | password |
  | invalid@temp.org | ValidPassword123 | Error: The email address or password you entered is incorrect | email    |
  #| user@example.com | WrongPassword    | The email address or password you entered is incorrect        | password |


@functional
Scenario: Display error when a locked account attempts to sign in
  When I enter "locked@temp.org" into the email address field
  And I enter "ValidPassword123" into the password field
  And I choose to sign in
  Then I should see the error message "Your account is locked"


