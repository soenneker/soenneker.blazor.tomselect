using Soenneker.Blazor.TomSelect.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Blazor.TomSelect.Tests;

[Collection("Collection")]
public class TomSelectTests : FixturedUnitTest
{
    private readonly ITomSelectInterop _interop;

    public TomSelectTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _interop = Resolve<ITomSelectInterop>(true);
    }
}
