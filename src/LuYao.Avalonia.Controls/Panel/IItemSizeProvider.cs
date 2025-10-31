using Avalonia;

namespace LuYao.Avalonia.Controls;

/// <summary>
/// Provides item sizes for variable-sized items in a VirtualizingWrapPanel.
/// </summary>
public interface IItemSizeProvider
{
    /// <summary>
    /// Gets the size of the item at the specified index.
    /// </summary>
    /// <param name="itemIndex">The index of the item.</param>
    /// <returns>The size of the item.</returns>
    Size GetSizeForItem(int itemIndex);
}
