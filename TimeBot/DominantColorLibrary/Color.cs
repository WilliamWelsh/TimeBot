namespace TimeBot.DominantColorLibrary
{
    public struct Color
    {
        /// <summary>
        ///     Get or Set the Alpha component value for sRGB.
        /// </summary>
        public byte A;

        /// <summary>
        ///     Get or Set the Blue component value for sRGB.
        /// </summary>
        public byte B;

        /// <summary>
        ///     Get or Set the Green component value for sRGB.
        /// </summary>
        public byte G;

        /// <summary>
        ///     Get or Set the Red component value for sRGB.
        /// </summary>
        public byte R;

        public string ToHexString()
        {
            return "#" + R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
        }

        public string ToHexAlphaString()
        {
            return "#" + A.ToString("X2") + R.ToString("X2") + G.ToString("X2") + B.ToString("X2");
        }

        public override string ToString()
        {
            if (A == 255)
            {
                return ToHexString();
            }

            return ToHexAlphaString();
        }
    }
}