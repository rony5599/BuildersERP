using BuilderERP.Application.DTOs;
using BuilderERP.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace BuilderERP.UnitTests.Validators;

public class CreateRfqDtoValidatorTests
{
    private readonly CreateRfqDtoValidator _validator = new();

    [Fact]
    public void Should_have_error_when_rfq_number_is_empty()
    {
        var model = new CreateRfqDto { RfqNumber = string.Empty, ClosingDate = DateTime.UtcNow.AddDays(7), PurchaseRequisitionId = Guid.NewGuid(), SupplierId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RfqNumber);
    }

    [Fact]
    public void Should_have_error_when_purchase_requisition_id_is_empty()
    {
        var model = new CreateRfqDto { RfqNumber = "RFQ-001", ClosingDate = DateTime.UtcNow.AddDays(7), PurchaseRequisitionId = Guid.Empty, SupplierId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.PurchaseRequisitionId);
    }

    [Fact]
    public void Should_have_error_when_supplier_id_is_empty()
    {
        var model = new CreateRfqDto { RfqNumber = "RFQ-001", ClosingDate = DateTime.UtcNow.AddDays(7), PurchaseRequisitionId = Guid.NewGuid(), SupplierId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.SupplierId);
    }

    [Fact]
    public void Should_not_have_error_for_valid_model()
    {
        var model = new CreateRfqDto { RfqNumber = "RFQ-001", ClosingDate = DateTime.UtcNow.AddDays(7), PurchaseRequisitionId = Guid.NewGuid(), SupplierId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
