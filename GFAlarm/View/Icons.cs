using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GFAlarm.View
{
    public class Icons
    {
        public static PackIconMaterial Pin = new PackIconMaterial()
        {
            Kind = PackIconMaterialKind.Pin,
            Width = 11,
            Height = 11,
            Padding = new Thickness(1, 0, 0, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        public static PackIconMaterial PinOff = new PackIconMaterial()
        {
            Kind = PackIconMaterialKind.PinOff,
            Width = 11,
            Height = 11,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
    }
}
