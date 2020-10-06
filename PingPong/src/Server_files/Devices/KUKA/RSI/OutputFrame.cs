using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PingPong.Devices {
    ///<summary>
    ///Frame sent to the KUKA robot
    ///</summary>
    public class OutputFrame {

        private static readonly string frameTemplate = @"
            <Sen Type='PingPong'>
                <EStr>{0}</EStr>
                <RKorr X='{1}' Y='{2}' Z='{3}' A='{4}' B='{5}' C='{6}' />
                <Tech T21='1.09' T22='2.08' T23='3.07' T24='4.06' T25='5.05' T26='6.04' T27='7.03' T28='8.02' T29='9.01' T210='10.00' />
                <IPOC>{7}</IPOC>
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

        public long IPOC { get; set; }

        public string Message { get; set; } = "PingPong";

        public E6POS Position { get; set; } = new E6POS();

        public override string ToString() {
            return string.Format(frameTemplate, 
                Message, 
                Position.X,
                Position.Y,
                Position.Z,
                Position.A,
                Position.B,
                Position.C,
                IPOC
            );
        }

    }
}