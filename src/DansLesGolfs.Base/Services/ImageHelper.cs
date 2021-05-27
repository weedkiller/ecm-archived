using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace DansLesGolfs.Base
{
    public class ImageHelper
    {
        /// <summary>
        /// Get Thumbnail image by maintain original size;
        /// </summary>
        /// <param name="img"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static System.Drawing.Image GetResizedImage(Bitmap img, int width, int height)
        {
            System.Drawing.Image thumbnail = null;
            try
            {
                if(img.Width < width && img.Height < height)
                {
                    thumbnail = GetThumbnailImage(img, img.Width, img.Height);
                }
                else
                {
                    double ratio = 0.0;
                    if (img.Width > img.Height)
                    {
                        ratio = width * 100 / img.Width;
                        height = (int)(img.Height * ratio / 100);
                    }
                    else
                    {
                        ratio = height * 100 / img.Height;
                        width = (int)(img.Width * ratio / 100);
                    }
                    thumbnail = GetThumbnailImage(img, width, height);
                }
            }
            catch (Exception ex)
            {
                thumbnail = img;
            }
            return thumbnail;
        }

        private static Bitmap GetThumbnailImage(Bitmap img, int width, int height)
        {
            Bitmap thumbnail = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(thumbnail);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, new Rectangle(0, 0, width, height));
            return thumbnail;
        }
    }
}
