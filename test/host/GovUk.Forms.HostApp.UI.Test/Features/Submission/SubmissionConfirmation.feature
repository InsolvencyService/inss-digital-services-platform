@MEDS-1064
Feature: SubmissionConfirmation

  As an Insolvency Practitioner
  I want to see that my RP14/14A has been submitted
  So that I know I have reached the end of the process

Background: 
  Given I am on the upload page as a "Admin" user

       @regression @rp14a
        Scenario: Insolvency Practitioner chooses to submit another RP14A form
            Given I am on the submission confirmation page
             When I select to Upload another form
             Then I will be taken to the declaration page
             And I should be able to log out successfully

        @regression @rp14a
        Scenario: Insolvency Practitioner starts a new RP14A submission
            Given I have selected Upload another form
              And I have agreed to the declaration
             When I upload another RP14A form
             Then I will be able to upload a new RP14A form


