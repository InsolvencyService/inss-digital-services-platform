using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Exceptions;
using Xunit;

namespace Inss.Platform.Domain.Tests.Components;

public class QueryParamListTests
{
    [Fact]
    public void ValueWithNoSpace_AddQueryParam_AddsValue()
    {
        QueryParamList queryParams = [];
        
        queryParams.AddQueryParam("name", "Homer");
        
        Assert.True(queryParams.ContainsKey("name"));
        Assert.Equal("Homer", queryParams["name"]);
    }
    
    [Fact]
    public void ValueWithSpace_AddQueryParam_AddsValue()
    {
        QueryParamList queryParams = [];
        
        queryParams.AddQueryParam("name", "Homer Simpson");
        
        Assert.True(queryParams.ContainsKey("name"));
        Assert.Equal("Homer+Simpson", queryParams["name"]);
    }
    
    [Fact]
    public void UnknownKeyForStringValue_FindQueryParam_ReturnsDefault()
    {
        QueryParamList queryParams = [];

        string? value = queryParams.FindQueryParam<string?>("name");
        
        Assert.Null(value);
    }
    
    [Fact]
    public void UnknownKeyForIntValue_FindQueryParam_ReturnsDefault()
    {
        QueryParamList queryParams = [];

        int value = queryParams.FindQueryParam<int>("age");
        
        Assert.Equal(0, value);
    }
    
    [Fact]
    public void KnownKeyForStringValue_FindQueryParam_ReturnsValue()
    {
        QueryParamList queryParams = [];
        queryParams.AddQueryParam("name", "Homer");
        
        string? value = queryParams.FindQueryParam<string?>("name");
        
        Assert.Equal("Homer", value);
    }
    
    [Fact]
    public void KnownKeyForIntValue_FindQueryParam_ReturnsValue()
    {
        QueryParamList queryParams = [];
        queryParams.AddQueryParam("age", 40);
        
        int value = queryParams.FindQueryParam<int>("age");
        
        Assert.Equal(40, value);
    }
    
    [Fact]
    public void UnknownKeyForStringValue_GetQueryParam_ThrowsException()
    {
        QueryParamList queryParams = [];

        ComponentException exception = Assert.Throws<ComponentException>(() => queryParams.GetQueryParam<string?>("name"));
        
        Assert.Equal("No name query param found.", exception.Message);
    }
    
    [Fact]
    public void UnknownKeyForIntValue_GetQueryParam_ThrowsException()
    {
        QueryParamList queryParams = [];

        ComponentException exception = Assert.Throws<ComponentException>(() => queryParams.GetQueryParam<int>("age"));
        
        Assert.Equal("No age query param found.", exception.Message);
    }
    
    [Fact]
    public void KnownKeyForStringValue_GetQueryParam_ReturnsValue()
    {
        QueryParamList queryParams = [];
        queryParams.AddQueryParam("name", "Homer");

        string? value = queryParams.GetQueryParam<string?>("name");
        
        Assert.Equal("Homer", value);
    }
    
    [Fact]
    public void KnownKeyForIntValue_GetQueryParam_ReturnsValue()
    {
        QueryParamList queryParams = [];
        queryParams.AddQueryParam("age", 40);

        int value = queryParams.GetQueryParam<int>("age");
        
        Assert.Equal(40, value);
    }
    
    [Fact]
    public void NoQueryParamsProvided_BuildQueryParams_ReturnsNull()
    {
        QueryParamList queryParams = [];

        string? value = queryParams.BuildQueryParams();
        
        Assert.Null(value);
    }
    
    [Fact]
    public void SingleQueryParamsProvided_BuildQueryParams_ReturnsValue()
    {
        QueryParamList queryParams = [];
        queryParams.AddQueryParam("name", "Homer");
        
        string? value = queryParams.BuildQueryParams();
        
        Assert.Equal("?name=Homer", value);
    }
    
    [Fact]
    public void MultipleQueryParamsProvided_BuildQueryParams_ReturnsValue()
    {
        QueryParamList queryParams = [];
        queryParams.AddQueryParam("name", "Homer Simpson");
        queryParams.AddQueryParam("age", 40);
        
        string? value = queryParams.BuildQueryParams();
        
        Assert.Equal("?name=Homer+Simpson&age=40", value);
    }
}