using Xunit;

namespace GovUk.Forms.Domain.Test;

public class PageMetaDataTests
{
    [Fact]
    public void PopulatedMetaData_CopyTo_UpdatesTargetMetaData()
    {
        PageMetaData sourceMetaData = new()
        {
            Question = "What is your name?", 
            Description = "How are you known", 
            Hint = "Your first and last name", 
            Group = "TestGroup", 
            SubmitButtonText = "Submit"
        };
        PageMetaData targetMetaData = new();
        
        sourceMetaData.CopyTo(targetMetaData);
        
        Assert.Equal(sourceMetaData.Question, targetMetaData.Question);
        Assert.Equal(sourceMetaData.Description, targetMetaData.Description);
        Assert.Equal(sourceMetaData.Hint, targetMetaData.Hint);
        Assert.Equal(sourceMetaData.Group, targetMetaData.Group);
        Assert.Equal(sourceMetaData.SubmitButtonText, targetMetaData.SubmitButtonText);
    }
}