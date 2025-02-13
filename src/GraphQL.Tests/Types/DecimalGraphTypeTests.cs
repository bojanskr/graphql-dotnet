using System.Collections;
using GraphQL.Types;
using GraphQLParser.AST;

namespace GraphQL.Tests.Types;

[Collection("StaticTests")]
public class DecimalGraphTypeTests
{
    private readonly DecimalGraphType _type = new();

    [Fact]
    public void coerces_null_to_null()
    {
        _type.ParseValue(null).ShouldBe(null);
    }

    [Fact]
    public void coerces_integer_to_decimal()
    {
        _type.ParseValue(0).ShouldBe((decimal)0);
    }

    [Fact]
    public void coerces_invalid_string_to_exception()
    {
        Should.Throw<InvalidOperationException>(() => _type.ParseValue("abcd"));
    }

    [Fact]
    public void coerces_numeric_string_to_decimal_throws()
    {
        Should.Throw<InvalidOperationException>(() => _type.ParseValue("12345.6579"));
    }

    [Fact]
    public void converts_DecimalValue_to_decimal_using_cultures()
    {
        CultureTestHelper.UseCultures(converts_DecimalValue_to_decimal);
    }

    [Fact]
    public void converts_DecimalValue_to_decimal()
    {
        _type.ParseLiteral(new GraphQLFloatValue(12345.6579m)).ShouldBe(12345.6579m);
        _type.ParseLiteral(new GraphQLFloatValue(39614081257132168796771975168m)).ShouldBe(39614081257132168796771975168m);
    }

    [Fact]
    public void Unsafe_As_Does_Not_Allocate_Memory()
    {
        long allocated = GC.GetAllocatedBytesForCurrentThread();

        decimal number = 12.10m;
        for (int i = 0; i < 1000; i++)
            _ = System.Runtime.CompilerServices.Unsafe.As<decimal, DecimalData>(ref number);

        GC.GetAllocatedBytesForCurrentThread().ShouldBe(allocated);
    }

    [Theory]
    [MemberData(nameof(SerializeListData))]
    public void serialize_list(IEnumerable list, bool canSerializeNullableList, bool canSerializeNonNullList)
    {
        _type.CanSerializeList(list, true).ShouldBe(canSerializeNullableList);
        _type.CanSerializeList(list, false).ShouldBe(canSerializeNonNullList);
        _type.SerializeList(list).ShouldBe(list);
    }

    public static object?[][] SerializeListData = [
        [new List<decimal> { 1, 2 }, true, true],
        [new List<decimal?> { 1, 2 }, true, true],
        [new List<decimal?> { 1, 2, null }, true, false],
    ];
}
