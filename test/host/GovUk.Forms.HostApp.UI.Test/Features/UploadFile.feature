@MEDS-1063
Feature: Upload Documents

A short summary of the feature

Background: 
  Given I am on the upload page as a "Admin" user

@functional @upload @Addvideo
Scenario: Upload a valid RP14A file successfully
  When I upload a valid RP14A file
  Then the uploaded file should appear in the file list

 @functional @upload @addVideo
Scenario: Prevent uploading the same file twice
  When I upload a valid RP14A file
  And I upload the same file again
  Then the file list should contain only one instance of that file

 @visual @smoke
Scenario: Verify Upload Document Page visual snapshot
  Then the upload document page should match the visual snapshot


@functional @visual 
  Scenario: Verify Common issues when uploading RP14/A forms Contents
	When I expand the common issues section
	Then the common issues section should display the correct content

@ignore @regression
Scenario: Display error when submitting without uploading a file
  When I click the continue button without selecting a file
  Then I should see the file upload error "The file must end with an XML extension"
