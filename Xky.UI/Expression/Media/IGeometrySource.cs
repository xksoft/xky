using System.Windows;

namespace Xky.UI.Expression.Media
{
    public interface IGeometrySource
    {
        bool InvalidateGeometry(InvalidateGeometryReasons reasons);

        bool UpdateGeometry(IGeometrySourceParameters parameters, Rect layoutBounds);

        System.Windows.Media.Geometry Geometry { get; }

        Rect LayoutBounds { get; }

        Rect LogicalBounds { get; }
    }
}