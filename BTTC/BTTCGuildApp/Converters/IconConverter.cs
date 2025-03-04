﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AvaloniaMiaDev.Converters;

public class TypeConverters
{
    private const string StreamGeometryNotFound =
        "M12.45 2.15C14.992 4.05652 17.5866 5 20.25 5C20.6642 5 21 5.33579 21 5.75V11C21 16.0012 18.0424 19.6757 12.2749 21.9478C12.0982 22.0174 11.9018 22.0174 11.7251 21.9478C5.95756 19.6757 3 16.0012 3 11V5.75C3 5.33579 3.33579 5 3.75 5C6.41341 5 9.00797 4.05652 11.55 2.15C11.8167 1.95 12.1833 1.95 12.45 2.15ZM12 3.67782C9.58084 5.38829 7.07735 6.32585 4.5 6.47793V11C4.5 15.2556 6.95337 18.3789 12 20.4419C17.0466 18.3789 19.5 15.2556 19.5 11V6.47793C16.9227 6.32585 14.4192 5.38829 12 3.67782ZM12 16C12.4142 16 12.75 16.3358 12.75 16.75C12.75 17.1642 12.4142 17.5 12 17.5C11.5858 17.5 11.25 17.1642 11.25 16.75C11.25 16.3358 11.5858 16 12 16ZM12 7.00356C12.3797 7.00356 12.6935 7.28572 12.7432 7.65179L12.75 7.75356V14.2523C12.75 14.6665 12.4142 15.0023 12 15.0023C11.6203 15.0023 11.3065 14.7201 11.2568 14.3541L11.25 14.2523V7.75356C11.25 7.33935 11.5858 7.00356 12 7.00356Z";

    public static FuncValueConverter<string, StreamGeometry> IconConverter { get; } =
        new(iconKey =>
        {
            if (iconKey is null) return StreamGeometry.Parse(StreamGeometryNotFound);

            Application.Current!.TryFindResource(iconKey, out var resource);
            return resource as StreamGeometry ?? StreamGeometry.Parse(StreamGeometryNotFound);
        });
}