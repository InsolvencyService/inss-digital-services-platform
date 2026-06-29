using Xunit;

namespace GovUk.Forms.Domain.Test;

public class RemoveModelTests
{
    [Fact]
    public void InitializedRemove_ResetRemove_ClearsValues()
    {
        RemoveModel remove = new() { SetIndex = 1, RemoveConfirmed = true };
        
        remove.ResetRemove();
        
        Assert.Equal(-1, remove.SetIndex);
        Assert.False(remove.RemoveConfirmed);
    }
}