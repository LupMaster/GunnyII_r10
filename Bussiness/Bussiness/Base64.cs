namespace Bussiness
{
    public class Base64
    {
        private static readonly string BASE64_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        static Base64()
        {
        }

        public static string encodeByteArray(byte[] param1)
        {
            string str = "";
            byte[] numArray1 = new byte[4];
            int num = 0;
            while (num < param1.Length)
            {
                byte[] numArray2 = new byte[3];
                for (int index = 0; index < param1.Length; ++index)
                {
                    if (index < 3)
                    {
                        if (index + num <= param1.Length)
                            numArray2[index] = param1[index + num];
                        else
                            break;
                    }
                }
                numArray1[0] = (byte)(((int)numArray2[0] & 252) >> 2);
                numArray1[1] = (byte)(((int)numArray2[0] & 3) << 4 | (int)numArray2[1] >> 4);
                numArray1[2] = (byte)(((int)numArray2[1] & 15) << 2 | (int)numArray2[2] >> 6);
                numArray1[3] = (byte)((uint)numArray2[2] & 63U);
                for (int length = numArray2.Length; length < 3; ++length)
                    numArray1[length + 1] = (byte)64;
                for (int index = 0; index < numArray1.Length; ++index)
                    str = str + Base64.BASE64_CHARS.Substring((int)numArray1[index], 1);
                num += 4;
            }
            return str.Substring(0, param1.Length - 1) + "=";
        }

        public static byte[] decodeToByteArray2(string param1)
        {
            byte[] numArray1 = new byte[param1.Length];
            byte[] numArray2 = new byte[4];
            int num = 0;
            while (num < param1.Length)
            {
                int index1 = 0;
                int startIndex;
                do
                {
                    startIndex = num + index1;
                    if (index1 < 4)
                        numArray2[index1] = (byte)Base64.BASE64_CHARS.IndexOf(param1.Substring(startIndex, 1));
                    ++index1;
                }
                while (startIndex < param1.Length);
                for (int index2 = 0; index2 < numArray2.Length && (int)numArray2[index2] != 64; ++index2)
                    numArray1[num + index2] = numArray2[index2];
                num += 4;
            }
            return numArray1;
        }

        public static byte[] decodeToByteArray(string param1)
        {
            byte[] numArray1 = new byte[param1.Length];
            byte[] numArray2 = new byte[4];
            byte[] numArray3 = new byte[3];
            int num = 0;
            while (num < param1.Length)
            {
                int index1 = 0;
                int startIndex;
                do
                {
                    startIndex = num + index1;
                    if (index1 < 4)
                        numArray2[index1] = (byte)Base64.BASE64_CHARS.IndexOf(param1.Substring(startIndex, 1));
                    ++index1;
                }
                while (startIndex < param1.Length);
                numArray3[0] = (byte)(((int)numArray2[0] << 2) + (((int)numArray2[1] & 48) >> 4));
                numArray3[1] = (byte)((((int)numArray2[1] & 15) << 4) + (((int)numArray2[2] & 60) >> 2));
                numArray3[2] = (byte)((uint)(((int)numArray2[2] & 3) << 6) + (uint)numArray2[3]);
                for (int index2 = 0; index2 < numArray3.Length && (int)numArray2[index2 + 1] != 64; ++index2)
                    numArray1[num + index2] = numArray3[index2];
                num += 4;
            }
            return numArray1;
        }
    }
}
