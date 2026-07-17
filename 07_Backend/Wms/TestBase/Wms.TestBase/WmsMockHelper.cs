using Moq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Wms.TestBase;

/// <summary>
/// WmsMockHelper — utility for quickly creating common ABP mock setups.
/// Reduces boilerplate in AppService tests where IRepository mocks need
/// standard ABP behavior (GetById, GetList, Insert, Update, Delete).
/// (Phase 10 test standardization)
/// </summary>
public static class WmsMockHelper
{
    /// <summary>
    /// Setup a standard ABP IRepository mock with default behaviors:
    /// - InsertAsync returns the entity
    /// - UpdateAsync returns the entity
    /// - DeleteAsync completes
    /// </summary>
    public static Mock<IRepository<TEntity, Guid>> SetupStandardRepository<TEntity>()
        where TEntity : class, IEntity<Guid>
    {
        var mock = new Mock<IRepository<TEntity, Guid>>();

        mock.Setup(r => r.InsertAsync(It.IsAny<TEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TEntity entity, bool autoSave, CancellationToken ct) => entity);

        mock.Setup(r => r.UpdateAsync(It.IsAny<TEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TEntity entity, bool autoSave, CancellationToken ct) => entity);

        mock.Setup(r => r.DeleteAsync(It.IsAny<TEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mock.Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }

    /// <summary>
    /// Setup repository mock to return a specific entity when GetByIdAsync is called.
    /// </summary>
    public static Mock<IRepository<TEntity, Guid>> SetupGetById<TEntity>(
        Mock<IRepository<TEntity, Guid>> mock, Guid id, TEntity entity)
        where TEntity : class, IEntity<Guid>
    {
        mock.Setup(r => r.GetAsync(id, It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mock.Setup(r => r.FindAsync(id, It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        return mock;
    }

    /// <summary>
    /// Setup repository mock to return a list of entities when GetListAsync is called.
    /// </summary>
    public static Mock<IRepository<TEntity, Guid>> SetupGetList<TEntity>(
        Mock<IRepository<TEntity, Guid>> mock, List<TEntity> entities)
        where TEntity : class, IEntity<Guid>
    {
        mock.Setup(r => r.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        return mock;
    }

    /// <summary>
    /// Create a mock for a custom repository interface that extends IRepository.
    /// The mock also implements IRepository through As&lt;IRepository&lt;TEntity, Guid&gt;&gt;().
    /// </summary>
    public static Mock<TRepo> SetupCustomRepository<TRepo, TEntity>()
        where TRepo : class, IRepository<TEntity, Guid>
        where TEntity : class, IEntity<Guid>
    {
        var mock = new Mock<TRepo>();

        // Also setup base IRepository behaviors
        mock.As<IRepository<TEntity, Guid>>()
            .Setup(r => r.InsertAsync(It.IsAny<TEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TEntity entity, bool autoSave, CancellationToken ct) => entity);

        mock.As<IRepository<TEntity, Guid>>()
            .Setup(r => r.UpdateAsync(It.IsAny<TEntity>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TEntity entity, bool autoSave, CancellationToken ct) => entity);

        return mock;
    }

    /// <summary>
    /// Create a mock IDistributedEventHandler for verifying event bus integration.
    /// </summary>
    public static Mock<THandler> CreateEventHandlerMock<THandler>()
        where THandler : class
    => new Mock<THandler>();
}
