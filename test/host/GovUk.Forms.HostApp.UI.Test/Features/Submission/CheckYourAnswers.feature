
@MEDS-1046 @addVideo
Feature: Check Your Answers

              As an Insolvency Practitioner
              I want to see the file that I have uploaded
So that I can confirm it’s the correct file and continue

        Background:
            Given I am on the upload page as a "InssTestTwo" user

        @functional @rp14a @cleanCosmosDb
        Scenario: Return to the upload page from the review page
            Given I am reviewing my uploaded RP14A document
             When I navigate back from the review page
             Then I should be returned to the document upload page


        @smoke @functional @rp14a @cleanCosmosDb
        Scenario: Review and submit an uploaded RP14A form
            Given I am reviewing my uploaded RP14A document
             When I submit the RP14A form
             Then the RP14A form should be successfully submitted
              And I should be able to log out successfully

        @regression @rp14 @cleanCosmosDb
        Scenario: Insolvency Practitioner starts a new RP14 submission after signing in
            Given I have successfully submitted an RP14 form
              And I am logged out
             When I sign in
             Then I should be taken to the declaration page
              And I should be able to upload a new RP14 form

        @regression @rp14 @api-upload @cleanCosmosDb
        Scenario: Insolvency Practitioner can view an uploaded RP14 after signing in
            Given I have uploaded an RP14 document
              And I am logged out
             When I sign in
             Then I should be taken to the declaration page
              And I should be able to view the uploaded RP14 document

        @functional @rp14a @cleanCosmosDb
        Scenario: Change the uploaded RP14A form before submission
            Given I am reviewing my uploaded RP14A document
             When I choose to change the uploaded document
             Then I should be returned to the case reference page
