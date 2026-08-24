using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreatePurchaseReturnDtoValidatorTests
{
    private readonly CreatePurchaseReturnDtoValidator _validator = new();

    private static List<CreatePurchaseReturnDetailDto> ValidDetails() => new()
    {
        new CreatePurchaseReturnDetailDto { GoodsReceiveDetailId = Guid.NewGuid(), MaterialId = Guid.NewGuid(), ReturnQuantity = 5, UnitPrice = 100 }
    };

    [Fact]
    public void Should_have_error_when_reason_is_empty()
    {
        var model = new CreatePurchaseReturnDto { Reason = "", GoodsReceiveId = Guid.NewGuid(), Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }

    [Fact]
    public void Should_have_error_when_goods_receive_id_is_empty()
    {
        var model = new CreatePurchaseReturnDto { Reason = "Damaged", GoodsReceiveId = Guid.Empty, Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.GoodsReceiveId);
    }

    [Fact]
    public void Should_have_error_when_details_is_empty()
    {
        var model = new CreatePurchaseReturnDto { Reason = "Damaged", GoodsReceiveId = Guid.NewGuid(), Details = new() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Details);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreatePurchaseReturnDto { Reason = "Damaged", GoodsReceiveId = Guid.NewGuid(), Details = ValidDetails() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
