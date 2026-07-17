using Shouldly;
using Volo.Abp;
using Xunit;

namespace Wms.TestBase;

/// <summary>
/// WmsAssertionHelper — unified assertion extension methods for WMS domain tests.
/// Standardizes the assertion patterns across all modules.
/// Uses Shouldly as the primary assertion library (Phase 8 convention).
/// (Phase 10 test standardization)
/// </summary>
public static class WmsAssertionHelper
{
    /// <summary>
    /// Assert that a BusinessException was thrown with a specific WMS error code.
    /// WMS error codes follow the pattern: WMS:{Module}:{Action}
    /// </summary>
    public static void ShouldThrowWmsError(Action action, string expectedErrorCode)
    {
        var exception = Should.Throw<BusinessException>(action);
        exception.Code.ShouldBe(expectedErrorCode);
    }

    /// <summary>
    /// Assert that a BusinessException was thrown (any WMS error code).
    /// </summary>
    public static void ShouldThrowBusinessException(Action action)
    {
        Should.Throw<BusinessException>(action);
    }

    /// <summary>
    /// Assert that no exception is thrown when executing the action.
    /// </summary>
    public static void ShouldNotThrow(Action action)
    {
        Should.NotThrow(action);
    }

    /// <summary>
    /// Assert that a SmartEnum property has the expected value.
    /// </summary>
    public static void ShouldHaveEnumValue<TEnum>(int actual, TEnum expected)
        where TEnum : Wms.Shared.Domain.Enums.SmartEnum<TEnum, int>
    => actual.ShouldBe(expected.Value);

    /// <summary>
    /// Assert that a collection is not empty and has the expected count.
    /// </summary>
    public static void ShouldHaveCount<T>(IEnumerable<T> collection, int expectedCount)
    => collection.Count().ShouldBe(expectedCount);
}
