using System;

namespace Dojo
{
    public class ImageDetails
    {
        public float Height { get; set; }

        public string Name { get; set; }

        public CGSize Size
        {
            get { return new CGSize(Width, Height); }
        }

        public float Width { get; set; }
    }
}
