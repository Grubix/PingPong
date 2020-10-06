using System.Globalization;

namespace Resilio_Project {
    abstract class StringOperations {

        static public double getDoubleBetween(string strSource, string strStart, string strEnd) {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                strSource = strSource.Substring(Start, End - Start);
                return double.Parse(strSource, CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }

        static public long getLongBetween(string strSource, string strStart, string strEnd) {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                strSource = strSource.Substring(Start, End - Start);
                return long.Parse(strSource, CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }

        static public string getStringBetween(string strSource, string strStart, string strEnd) {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                strSource = strSource.Substring(Start, End - Start);
                return strSource;
            }
            else
            {
                return "";
            }
        }
    }
}
