@MEDS-1064
Feature: SubmissionConfirmation

  As an Insolvency Practitioner
  I want to see that my RP14/14A has been submitted
  So that I know I have reached the end of the process

Background: 
  Given I am on the upload page as a "InssTestThree" user

@regression @rp14a @cleanCosmosDb @addVideo @api-upload
Scenario: Insolvency Practitioner chooses to submit another RP14A API form 
            Given I am on the submission confirmation page
             And I retrieve the first submission confirmation email
             And the first submission confirmation email contains the submitted details
            When I select to Upload another form
            Then I will be taken to the declaration page
              And I should be able to log out successfully

@regression @rp14a @cleanCosmosDb @addVideo
Scenario: Insolvency Practitioner chooses to submit another RP14A form
            Given I am on the submission confirmation page
             When I select to Upload another form
             Then I will be taken to the declaration page

@regression @rp14a @cleanCosmosDb
Scenario: Insolvency Practitioner starts a new RP14A submission
    Given I am on the submission confirmation page
     And I retrieve the first submission confirmation email
     And the first submission confirmation email contains the submitted details
    When I select Upload another form
     And I agree to the declaration
     And I upload another RP14A form
    Then I should be able to submit the new RP14A form successfully
     And I retrieve the second submission confirmation email
     And the second submission confirmation email contains the submitted details

@regression @rp14a @cleanCosmosDb @api-upload
Scenario: Insolvency Practitioner starts a new RP14A API submission 
            Given I have selected Upload another form
              And I have agreed to the declaration
             When I upload another RP14A form
             Then I will be able to upload a new RP14A form

@regression @rp14 @cleanCosmosDb @addVideo
Scenario: Insolvency Practitioner chooses to submit another RP14 form
            Given I am on the RP14 submission confirmation page
             And I retrieve the first submission confirmation email
             And the first submission confirmation email contains the submitted RP14 details
            When I select to Upload another form
            Then I will be taken to the declaration page
             And I should be able to log out successfully

@regression @rp14 @cleanCosmosDb @addVideo @api-upload
Scenario: Insolvency Practitioner chooses to submit another RP14 API form
            Given I am on the RP14 submission confirmation page
             When I select to Upload another form
             Then I will be taken to the declaration page
              And I should be able to log out successfully

@regression @rp14 @cleanCosmosDb
Scenario: Insolvency Practitioner starts a new RP14 submission
            Given I have selected Upload another RP14 form
              And I have agreed to the declaration
             When I upload another RP14 form
             Then I will be able to upload a new RP14 form

@regression @rp14 @cleanCosmosDb @api-upload
Scenario: Insolvency Practitioner starts a new RP14 API submission
           Given I am on the RP14 submission confirmation page
             And I retrieve the first submission confirmation email
             And the first submission confirmation email contains the submitted RP14 details
           When I select Upload another form
            And I agree to the declaration
            And I upload another RP14 form
          Then I will be able to upload a new RP14 form
            And I retrieve the second submission confirmation email
            And the second submission confirmation email contains the submitted RP14 details


