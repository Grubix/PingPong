using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PingPong.RSI {
    ///<summary>Frame sent to the KUKA robot</summary>
    public class OutputFrame {

        private static string _frameTemplate = @"
            <Sen Type='PingPong'>
                <EStr>{0}</EStr>
                <RKorr X='{1}' Y='{2}' Z='{3}' A='{4}' B='{5}' C='{6}' />
                <IPOC></IPOC>
            </Sen>";

        ///<summary>Frame template minification (remove new lines, indent etc.)</summary>
        static OutputFrame() {
            XDocument document = XDocument.Parse(_frameTemplate);
            StringBuilder sBuilder = new StringBuilder();
            XmlWriterSettings xmlSettings = new XmlWriterSettings() {
                OmitXmlDeclaration = true
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(sBuilder, xmlSettings)) {
                document.Root.Save(xmlWriter);
            }

            _frameTemplate = sBuilder.ToString();
        }

        public string Message { get; set; } = "PingPong";

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double A { get; set; }

        public double B { get; set; }

        public double C { get; set; }

        public override string ToString() {
            return string.Format(_frameTemplate, Message, X, Y, Z, A, B, C);
        }

    }
}