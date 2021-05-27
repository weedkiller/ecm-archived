using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DansLesGolfs
{
    public class ImageResult : FileStreamResult
    {
        public ImageResult(Image input) : this(input, input.Width, input.Height) { }
        public ImageResult(Image input, int width, int height) :
            base(
              GetMemoryStream(input, width, height, ImageFormat.Jpeg),
              "image/jpeg")
        { }

        static MemoryStream GetMemoryStream(Image input, int width,
                            int height, ImageFormat fmt)
        {
            // maintain aspect ratio 
            if (input.Width > input.Height)
                height = input.Height * width / input.Width;
            else
                width = input.Width * height / input.Height;

            var bmp = new Bitmap(input, width, height);
            var ms = new MemoryStream();

            if(fmt == ImageFormat.Jpeg)
            {
                ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageDecoders().Where(it => it.FormatID == ImageFormat.Jpeg.Guid).SingleOrDefault();
                System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters encoderParameters = new EncoderParameters(1);
                EncoderParameter encoderParameter = new EncoderParameter(encoder, 85L);
                encoderParameters.Param[0] = encoderParameter;
                bmp.Save(ms, jpegEncoder, encoderParameters);
            }
            else
            {
                bmp.Save(ms, fmt);
            }

            ms.Position = 0;
            return ms;
        }
    }
}