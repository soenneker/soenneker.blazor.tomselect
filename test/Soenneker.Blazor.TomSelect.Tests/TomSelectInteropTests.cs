using Soenneker.Blazor.TomSelect.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Blazor.TomSelect.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class TomSelectInteropTests : HostedUnitTest
{
    private readonly ITomSelectInterop _util;

    public TomSelectInteropTests(Host host) : base(host)
    {
        _util = Resolve<ITomSelectInterop>(true);
    }

    [Test]
    public void Default()
    { 
    
    }
}
