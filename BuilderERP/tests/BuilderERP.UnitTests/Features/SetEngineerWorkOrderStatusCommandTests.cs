using BuilderERP.Application.Features.EngineerWorkOrders;
using BuilderERP.Domain.Entities;
using BuilderERP.Domain.Enums;
using BuilderERP.Domain.Interfaces;
using Moq;

namespace BuilderERP.UnitTests.Features;

public class SetEngineerWorkOrderStatusCommandTests
{
    [Fact]
    public async Task Handle_UpdatesLatestRevision_WhenExpectedStatusMatches()
    {
        var workOrder = new EngineerWorkOrder
        {
            Id = 42,
            IsLatestRevision = true,
            Status = EngineerWorkOrderStatus.Submitted
        };
        var repository = new Mock<IRepository<EngineerWorkOrder>>();
        repository.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(workOrder);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Repository<EngineerWorkOrder>()).Returns(repository.Object);

        var handler = new SetEngineerWorkOrderStatusCommandHandler(unitOfWork.Object);
        var result = await handler.Handle(new SetEngineerWorkOrderStatusCommand(
            42, EngineerWorkOrderStatus.Submitted, EngineerWorkOrderStatus.UnderApproval, "approver"),
            CancellationToken.None);

        Assert.True(result);
        Assert.Equal(EngineerWorkOrderStatus.UnderApproval, workOrder.Status);
        Assert.Equal("approver", workOrder.ModifiedBy);
        repository.Verify(r => r.Update(workOrder), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(false, EngineerWorkOrderStatus.Submitted)]
    [InlineData(true, EngineerWorkOrderStatus.Draft)]
    public async Task Handle_RejectsStaleOrNonLatestTransition(bool isLatestRevision, EngineerWorkOrderStatus currentStatus)
    {
        var workOrder = new EngineerWorkOrder
        {
            Id = 42,
            IsLatestRevision = isLatestRevision,
            Status = currentStatus
        };
        var repository = new Mock<IRepository<EngineerWorkOrder>>();
        repository.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(workOrder);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(u => u.Repository<EngineerWorkOrder>()).Returns(repository.Object);

        var handler = new SetEngineerWorkOrderStatusCommandHandler(unitOfWork.Object);
        var result = await handler.Handle(new SetEngineerWorkOrderStatusCommand(
            42, EngineerWorkOrderStatus.Submitted, EngineerWorkOrderStatus.UnderApproval, "approver"),
            CancellationToken.None);

        Assert.False(result);
        repository.Verify(r => r.Update(It.IsAny<EngineerWorkOrder>()), Times.Never);
        unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}
