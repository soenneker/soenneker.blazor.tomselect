using Soenneker.Blazor.TomSelect.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Blazor.TomSelect.Tests;

[Collection("Collection")]
public class TomSelectInteropTests : FixturedUnitTest
{
    private readonly ITomSelectInterop _util;

    public TomSelectInteropTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ITomSelectInterop>(true);
    }

    [Fact]
    public void Default()
    { 
    
    }
}
