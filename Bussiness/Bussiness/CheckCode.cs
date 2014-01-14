using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Bussiness
{
    public class CheckCode
    {
        public static ThreadSafeRandom rand = new ThreadSafeRandom();
        private static Color[] c = new Color[2]
    {
      Color.Gray,
      Color.DimGray
    };
        private static string[] font = new string[5]
    {
      "Verdana",
      "Terminal",
      "Comic Sans MS",
      "Arial",
      "Tekton Pro"
    };
        private static char[] digitals = new char[9]
    {
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9'
    };
        private static char[] lowerLetters = new char[21]
    {
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'h',
      'k',
      'm',
      'n',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z'
    };
        private static char[] upperLetters = new char[22]
    {
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'K',
      'M',
      'N',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z'
    };
        private static char[] letters = new char[50]
    {
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z'
    };
        private static char[] mix = new char[51]
    {
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'h',
      'k',
      'm',
      'n',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'K',
      'M',
      'N',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z'
    };

        static CheckCode()
        {
        }

        public static byte[] CreateImage(string randomcode)
        {
            int maxValue = 30;
            Bitmap bitmap = new Bitmap(randomcode.Length * 30, 32);
            Graphics graphics = Graphics.FromImage((Image)bitmap);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            byte[] numArray;
            try
            {
                graphics.Clear(Color.Transparent);
                int index1 = CheckCode.rand.Next(2);
                Brush brush = (Brush)new SolidBrush(CheckCode.c[index1]);
                for (int index2 = 0; index2 < 1; ++index2)
                {
                    int num1 = CheckCode.rand.Next(bitmap.Width / 2);
                    int num2 = CheckCode.rand.Next(bitmap.Width * 3 / 4, bitmap.Width);
                    int num3 = CheckCode.rand.Next(bitmap.Height);
                    int num4 = CheckCode.rand.Next(bitmap.Height);
                    graphics.DrawBezier(new Pen(CheckCode.c[index1], 2f), (float)num1, (float)num3, (float)((num1 + num2) / 4), 0.0f, (float)((num1 + num2) * 3 / 4), (float)bitmap.Height, (float)num2, (float)num4);
                }
                char[] chArray = randomcode.ToCharArray();
                StringFormat format = new StringFormat(StringFormatFlags.NoClip);
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                for (int index2 = 0; index2 < chArray.Length; ++index2)
                {
                    int index3 = CheckCode.rand.Next(5);
                    Font font = new Font(CheckCode.font[index3], 22f, FontStyle.Bold);
                    Point point = new Point(16, 16);
                    float angle = (float)ThreadSafeRandom.NextStatic(-maxValue, maxValue);
                    graphics.TranslateTransform((float)point.X, (float)point.Y);
                    graphics.RotateTransform(angle);
                    graphics.DrawString(chArray[index2].ToString(), font, brush, 1f, 1f, format);
                    graphics.RotateTransform(-angle);
                    graphics.TranslateTransform(2f, -(float)point.Y);
                }
                MemoryStream memoryStream = new MemoryStream();
                bitmap.Save((Stream)memoryStream, ImageFormat.Png);
                numArray = memoryStream.ToArray();
            }
            finally
            {
                graphics.Dispose();
                bitmap.Dispose();
            }
            return numArray;
        }

        private static string GenerateRandomString(int length, CheckCode.RandomStringMode mode)
        {
            string str = string.Empty;
            if (length == 0)
                return str;
            switch (mode)
            {
                case CheckCode.RandomStringMode.LowerLetter:
                    for (int index = 0; index < length; ++index)
                        str = str + (object)CheckCode.lowerLetters[CheckCode.rand.Next(0, CheckCode.lowerLetters.Length)];
                    break;
                case CheckCode.RandomStringMode.UpperLetter:
                    for (int index = 0; index < length; ++index)
                        str = str + (object)CheckCode.upperLetters[CheckCode.rand.Next(0, CheckCode.upperLetters.Length)];
                    break;
                case CheckCode.RandomStringMode.Letter:
                    for (int index = 0; index < length; ++index)
                        str = str + (object)CheckCode.letters[CheckCode.rand.Next(0, CheckCode.letters.Length)];
                    break;
                case CheckCode.RandomStringMode.Digital:
                    for (int index = 0; index < length; ++index)
                        str = str + (object)CheckCode.digitals[CheckCode.rand.Next(0, CheckCode.digitals.Length)];
                    break;
                default:
                    for (int index = 0; index < length; ++index)
                        str = str + (object)CheckCode.mix[CheckCode.rand.Next(0, CheckCode.mix.Length)];
                    break;
            }
            return str;
        }

        public static string GenerateCheckCode()
        {
            return CheckCode.GenerateRandomString(4, CheckCode.RandomStringMode.Mix);
        }

        private enum RandomStringMode
        {
            LowerLetter,
            UpperLetter,
            Letter,
            Digital,
            Mix,
        }
    }
}
