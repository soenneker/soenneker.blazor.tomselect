using System;

namespace Soenneker.Blazor.TomSelect.Base.Abstract;

/// <summary>
/// Represents the base object for TomSelect.
/// </summary>
/// <remarks>We need a base object because we have generic type parameters on TomSelect.</remarks>
public interface IBaseTomSelect : IDisposable, IAsyncDisposable
{
}