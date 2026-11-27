using System.Threading;
using System.Threading.Tasks;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Infrastructure.Context;
using MediatR;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace Contacts.Tests;

public class CosmosContainerContextTest
{
    [Fact]
    public async Task SaveChangesAsync_WaitsForAsyncEventHandlers_BeforeSaving()
    {
        var batch = new Mock<TransactionalBatch>();
        var batchResponse = new Mock<TransactionalBatchResponse>();
        batchResponse.Setup(r => r.IsSuccessStatusCode).Returns(true);
        batchResponse.Setup(r => r[It.IsAny<int>()]).Returns(new Mock<TransactionalBatchOperationResult>().Object);
        batch.Setup(b => b.ExecuteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(batchResponse.Object);
        var container = new Mock<Container>();
        container.Setup(c => c.CreateTransactionalBatch(It.IsAny<PartitionKey>())).Returns(batch.Object);

        CosmosContainerContext context = null;
        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Publish(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
            .Returns(async (IEvent e, CancellationToken _) =>
            {
                // Simulates a handler that does async work before adding its event to the outbox.
                await Task.Delay(50);
                var evt = (ContactDomainEvent)e;
                context.Add(new DataObject<ContactDomainEvent>(evt.Id.ToString(), evt.ContactId.ToString(),
                    "domainEvent", evt, null, -1, EntityState.Created));
            });
        context = new CosmosContainerContext(container.Object, mediator.Object);

        var contact = Contact.CreateNew();
        context.Add(new DataObject<Contact>(contact.Id.ToString(), contact.Id.ToString(), "contact", contact,
            null, -1, EntityState.Created));

        var result = await context.SaveChangesAsync();

        Assert.Equal(2, result.Count);
        batch.Verify(b => b.CreateItem(It.IsAny<IDataObject<Entity>>(),
            It.IsAny<TransactionalBatchItemRequestOptions>()), Times.Exactly(2));
    }
}
