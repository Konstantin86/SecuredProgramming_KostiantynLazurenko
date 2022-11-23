using System.Text;
using ZXing.Rendering;
using ZXing.Common;
using ZXing;

public class BarcodeHelper
{
    public static T RenderBarcode<T>(IBarcodeRenderer<T> renderer, string content, EncodingOptions options, BarcodeFormat format = BarcodeFormat.QR_CODE) =>
        new BarcodeWriter<T> { Format = format, Options = options, Renderer = renderer }.Write(content);

    public class StringWriter : IBarcodeRenderer<string>
    {

        public string Fill { get; set; }
        public string EmptyBlock { get; set; }
        public string NewLine { get; set; }

        public string Render(BitMatrix matrix, BarcodeFormat format, string content) => Render(matrix, format, content, null);

        public string Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            var strBuilder = new StringBuilder();

            for (int i = 0; i < matrix.Height; i++)
            {
                if (i > 0) 
                    strBuilder.Append(NewLine);
                for (int X = 0; X < matrix.Width; X++) 
                    strBuilder.Append(matrix[X, i] ? Fill : EmptyBlock);
            }

            return strBuilder.ToString();
        }
    }

}