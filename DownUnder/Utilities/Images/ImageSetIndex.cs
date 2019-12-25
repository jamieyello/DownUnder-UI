using System;

namespace DownUnder.Utility.Images
{
    public class ImageSetIndex
    {
        public int index = -1;
        public String name = "";

        public ImageSetIndex()
        {
        }

        public ImageSetIndex(int index_, String name_)
        {
            index = index_;
            name = name_;
        }

        public object Clone()
        {
            ImageSetIndex c = new ImageSetIndex();
            c.index = index;
            c.name = name;
            return c;
        }
    }
}