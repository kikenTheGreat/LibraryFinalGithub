using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;

//eto yung bago
namespace Library_Final
{
    public class SimpleBitmapRenderer : ZXing.Rendering.IBarcodeRenderer<Bitmap>
    {
        public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
        {
            return Render(matrix, format, content, new EncodingOptions());
        }

        public Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            int width = matrix.Width;
            int height = matrix.Height;
            Bitmap bitmap = new Bitmap(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, matrix[x, y] ? Color.Black : Color.White);
                }
            }
            return bitmap;
        }
    }
}
