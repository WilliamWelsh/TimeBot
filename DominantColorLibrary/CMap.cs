using System.Linq;
using System.Collections.Generic;
using TimeBot.DominantColorLibrary;

namespace TimeBot
{
    internal class CMap
    {
        private readonly List<VBox> vboxes = new List<VBox>();
        private List<QuantizedColor> palette;

        public void Push(VBox box)
        {
            palette = null;
            vboxes.Add(box);
        }

        public List<QuantizedColor> GeneratePalette()
        {
            if (palette == null)
            {
                palette = (from vBox in vboxes
                           let rgb = vBox.Avg(false)
                           let color = FromRgb(rgb[0], rgb[1], rgb[2])
                           select new QuantizedColor(color, vBox.Count(false))).ToList();
            }

            return palette;
        }

        public Color FromRgb(int red, int green, int blue)
        {
            var color = new Color
            {
                R = (byte)red,
                G = (byte)green,
                B = (byte)blue
            };

            return color;
        }
    }
}