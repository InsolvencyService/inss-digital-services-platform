@MEDS-1063
Feature: Upload Documents

A short summary of the feature

@functional @upload
Scenario: Upload a valid file successfully
  Given I am on the upload page
  When I upload a valid file
  Then the uploaded file should appear in the file list

 @functional @upload
Scenario: Prevent uploading the same file twice
  Given I am on the upload page
  When I upload a file
  And I upload the same file again
  Then the file list should contain only one instance of that file
