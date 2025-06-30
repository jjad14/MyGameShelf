using MyGameShelf.Application.Validation.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Tests.Validation.Attributes;
public class BirthdayRangeAttributeTests
{
    private readonly BirthdayRangeAttribute _attribute = new BirthdayRangeAttribute();

    [Fact]
    public void Birthday_Today_ShouldBeValid()
    {
        var result = _attribute.GetValidationResult(DateTime.Today, new ValidationContext(new object()));
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void Birthday_Exactly100YearsAgo_ShouldBeValid()
    {
        var result = _attribute.GetValidationResult(DateTime.Today.AddYears(-100), new ValidationContext(new object()));
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void Birthday_Over100YearsAgo_ShouldBeInvalid()
    {
        var result = _attribute.GetValidationResult(DateTime.Today.AddYears(-101), new ValidationContext(new object()));
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Birthday must be within the last 100 years and not in the future.", result.ErrorMessage);
    }

    [Fact]
    public void Birthday_InTheFuture_ShouldBeInvalid()
    {
        var result = _attribute.GetValidationResult(DateTime.Today.AddDays(1), new ValidationContext(new object()));
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("Birthday must be within the last 100 years and not in the future.", result.ErrorMessage);
    }

    [Fact]
    public void Birthday_Null_ShouldBeValid()
    {
        var result = _attribute.GetValidationResult(null, new ValidationContext(new object()));
        Assert.Equal(ValidationResult.Success, result);
    }
}