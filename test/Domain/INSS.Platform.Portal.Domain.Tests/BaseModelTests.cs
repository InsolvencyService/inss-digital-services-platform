using Xunit;

namespace INSS.Platform.Portal.Domain.Tests;

public class BaseModelTests
{
    [Fact]
    public void SourceFullName_CopyTo_UpdatesTargetFullName()
    {
        FullNameModel source = new() {FullName = "Marge Simpson"};
        FullNameModel target = new() {FullName = "Homer Simpson"};

        source.CopyTo(target);

        Assert.Equal("Marge Simpson", target.FullName);
    }

    [Fact]
    public void SourceBankAccount_CopyTo_UpdatesTargetBankAccount()
    {
        BankAccountModel source = new() {AccountNumber = "12345678", SortCode = "12-34-56"};
        BankAccountModel target = new() {AccountNumber = "87654321", SortCode = "65-43-21"};

        source.CopyTo(target);

        Assert.Equal("12345678", target.AccountNumber);
        Assert.Equal("12-34-56", target.SortCode);
    }

    [Fact]
    public void FromSource_DeepCopyTo_CopiesAllSourceProperties()
    {
        FullNameModel source = new() {FullName = "Marge Simpson", PathName = "test"};
        FullNameModel target = new() {FullName = "Homer Simpson", PathName = "old"};

        source.DeepCopyTo(target);

        Assert.Equal(source.FullName, target.FullName);
        Assert.Equal(source.Id, target.Id);
        Assert.Equal(source.PathName, target.PathName);
        Assert.Equal(source.PageUrl, target.PageUrl);
        Assert.Equal(source.Kind, target.Kind);
        Assert.Equal(source.ViewName, target.ViewName);
    }
    
    [Fact]
    public void SourceFullName_Clone_ReturnsNewFullNameInstance()
    {
        FullNameModel fullName = new() {FullName = "Marge Simpson"};

        BaseModel copy = fullName.Clone();

        Assert.IsType<FullNameModel>(copy);
        Assert.NotSame(fullName, copy);
        FullNameModel fullNameCopy = (FullNameModel)copy;
        Assert.Equal(fullName.FullName, fullNameCopy.FullName);
    }  

    [Fact]
    public void SourceBankAccount_Clone_ReturnsNewBankAccountInstance()
    {
        BankAccountModel bankAccount = new() {AccountNumber = "12345678", SortCode = "12-34-56"};

        BaseModel copy = bankAccount.Clone();

        Assert.IsType<BankAccountModel>(copy);
        Assert.NotSame(bankAccount, copy);
        BankAccountModel bankAccountCopy = (BankAccountModel)copy;
        Assert.Equal(bankAccount.SortCode, bankAccountCopy.SortCode);
    }

    [Fact]
    public void BankAccountWithValues_Reset_ClearsModelProperties()
    {
        BankAccountModel bankAccount = new() 
        {
            Id = Guid.NewGuid().ToString("D"),
            PageUrl = "/test/assets/bank-account",
            AccountNumber = "12345678", 
            SortCode = "12-34-56"
        };

        bankAccount.Reset();

        Assert.NotEmpty(bankAccount.Id);
        Assert.NotEmpty(bankAccount.PageUrl);
        Assert.Null(bankAccount.AccountNumber);
        Assert.Null(bankAccount.SortCode);
    } 

    [Fact]
    public void BankAccountWithValues_GetValues_ReturnsModelValues()
    {
        BankAccountModel bankAccount = new() 
        {
            Id = Guid.NewGuid().ToString("D"),
            PageUrl = "/test/assets/bank-account",
            AccountNumber = "12345678", 
            SortCode = "12-34-56"
        };

        string[] values = bankAccount.GetValues();

        Assert.Contains(bankAccount.AccountNumber, values);
        Assert.Contains(bankAccount.SortCode, values);
    }
}
