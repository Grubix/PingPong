using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PingPong.Devices {
    ///<summary>
    ///Frame sent to the KUKA robot
    ///</summary>
    public class OutputFrame {

        /// <summary>
        /// <list type="bullet">
        /// <item>
        /// <term>EStr</term>
        /// <description>Notification or error message</description>
        /// </item>
        /// <item>
        /// <term>Rkorr</term>
        /// <description>O tym nie ma ani s?owa w dokumentacji xdddd</description>
        /// </item>
        /// <item>
        /// <term>IPOC</term>
        /// <description>The keyword IPOC sends a time stamp and is generated automatically</description>
        /// </item>
        /// </list>
        /// </summary>
        private static readonly string frameTemplate = @"
            <Sen Type='PingPong'>
                <EStr>{0}</EStr>
                <RKorr X='{1}' Y='{2}' Z='{3}' A='{4}' B='{5}' C='{6}' />
                <IPOC></IPOC>
            </Sen>";

        ///<summary>
        ///Frame template minification (remove new lines, indentation etc.)
        ///</summary>
        static OutputFrame() {
            XDocument document = XDocument.Parse(frameTemplate);
            StringBuilder sBuilder = new StringBuilder();
            XmlWriterSettings xmlSettings = new XmlWriterSettings() {
                OmitXmlDeclaration = true
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(sBuilder, xmlSettings)) {
                document.Root.Save(xmlWriter);
            }

            frameTemplate = sBuilder.ToString();
        }

        public string Message { get; set; } = "PingPong";

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double A { get; set; }

        public double B { get; set; }

        public double C { get; set; }

        public OutputFrame() {
        }

        public OutputFrame(E6POS pos) {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            A = pos.A;
            B = pos.B;
            C = pos.C;
        }

        public override string ToString() {
            return string.Format(frameTemplate, Message, X, Y, Z, A, B, C);
        }

    }
}