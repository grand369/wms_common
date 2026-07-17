using System.Reflection;
using System.Threading;

namespace Wms.Shared.Domain.Enums;

/// <summary>
/// Smart Enum base class — provides enumeration with Description attribute support.
/// All shared enums inherit from this class.
/// </summary>
public abstract class SmartEnum<TEnum, TValue> where TEnum : SmartEnum<TEnum, TValue> where TValue : IEquatable<TValue>
{
    private static readonly Lazy<List<TEnum>> _allValues =
    new Lazy<List<TEnum>>(() =>
        typeof(TEnum)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f.FieldType == typeof(TEnum))
            .Select(f => (TEnum)f.GetValue(null))
            .OrderBy(e => e.Value)
            .ToList(),
        LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly Lazy<Dictionary<string, TEnum>> _byName =
        new Lazy<Dictionary<string, TEnum>>(() =>
            _allValues.Value.ToDictionary(e => e.Name, e => e), LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly Lazy<Dictionary<TValue, TEnum>> _byValue =
        new Lazy<Dictionary<TValue, TEnum>>(() =>
        {
            var dict = new Dictionary<TValue, TEnum>();
            foreach (var entry in _allValues.Value)
            {
                if (!dict.ContainsKey(entry.Value))
                    dict[entry.Value] = entry;
            }
            return dict;
        }, LazyThreadSafetyMode.ExecutionAndPublication);

    public string Name { get; }
    public TValue Value { get; }

    protected SmartEnum(string name, TValue value)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static List<TEnum> List => _allValues.Value;
    public static TEnum FromName(string name) => _byName.Value[name];
    public static TEnum FromValue(TValue value) => _byValue.Value[value];
    public static bool TryFromValue(TValue value, out TEnum result) => _byValue.Value.TryGetValue(value, out result);

    public override string ToString() => Name;
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object obj) => obj is SmartEnum<TEnum, TValue> other && Value.Equals(other.Value);

    public static bool operator ==(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right)
    {
        if (left is null) return right is null;
        if (right is null) return false;
        return left.Value.Equals(right.Value);
    }

    public static bool operator !=(SmartEnum<TEnum, TValue>? left, SmartEnum<TEnum, TValue>? right)
        => !(left == right);
}
