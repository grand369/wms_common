using Moq;
using System.Linq.Expressions;

namespace Wms.TestBase;

/// <summary>
/// WmsAppServiceTestBase — base class for Application Service unit tests.
/// Provides a template for setting up mocked dependencies before each test.
/// Uses Moq for mocking IRepository and cross-module IDomainService interfaces.
/// AppService tests focus on: orchestration logic, DTO mapping, cross-module calls, permission checks.
/// (Phase 8 Coding Conventions Section 6, Phase 10 standardization)
/// </summary>
/// <remarks>
/// Usage pattern:
/// <code>
/// public class MyAppServiceTests : WmsAppServiceTestBase
/// {
///     private readonly Mock&lt;IMyRepository&gt; _mockRepo;
///     private readonly MyAppService _service;
///
///     public MyAppServiceTests()
///     {
///         _mockRepo = CreateMock&lt;IMyRepository&gt;();
///         _service = CreateAppService&lt;MyAppService&gt;(
///             mock => mock.SetupMyRepository(_mockRepo));
///     }
/// }
/// </code>
/// </remarks>
public abstract class WmsAppServiceTestBase
{
    /// <summary>
    /// Create a Moq mock for the given interface/type.
    /// Shorthand for new Mock&lt;T&gt;().
    /// </summary>
    protected static Mock<T> CreateMock<T>() where T : class => new Mock<T>();

    /// <summary>
    /// Verify that a mock was called exactly N times.
    /// Shorthand for mock.Verify(expression, Times.Exactly(n)).
    /// </summary>
    protected static void VerifyCalled<T>(Mock<T> mock, Expression<Action<T>> expression, int times = 1)
        where T : class
    => mock.Verify(expression, Times.Exactly(times));

    /// <summary>
    /// Verify that a mock was never called with the given expression.
    /// </summary>
    protected static void VerifyNotCalled<T>(Mock<T> mock, Expression<Action<T>> expression)
        where T : class
    => mock.Verify(expression, Times.Never());
}
