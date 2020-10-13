using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace PingPong.Devices.KUKA {
    /// <summary>
    /// Frame sent to the KUKA robot.
    /// </summary>
    public class OutputFrame {

        private static readonly string frameTemplate = @"
            <Sen Type='PingPong'>
                <EStr>{0}</EStr>
                <RKorr X='{1}' Y='{2}' Z='{3}' A='{4}' B='{5}' C='{6}' />
                <IPOC>{7}</IPOC>
            </Sen>";

        /// <summary>
        /// Minifies frame template (removes new lines, indentation, redundant white characters etc.)
        /// </summary>
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

        private E6POS correction = new E6POS();

        public E6POS Correction { 
            get {
                return correction;
            }
            set {
                correction = ClampCorrectionValues(value);
            } 
        }

        public override string ToString() {
            return string.Format(frameTemplate, 
                Message, 
                Correction.X,
                Correction.Y,
                Correction.Z,
                Correction.A,
                Correction.B,
                Correction.C,
                IPOC
            );
        }

        private static double ClampValue(double value, double min, double max) {
            //TODO: zarzucanie wyjątkiem ??
            if (value < min) {
                return min;
            } else if (value > max) {
                return max;
            } else {
                return value;
            }
        }

        private static E6POS ClampCorrectionValues(E6POS correction) {
            return new E6POS(
                ClampValue(correction.X, -1, 1),
                ClampValue(correction.Y, -1, 1),
                ClampValue(correction.Z, -1, 1),
                ClampValue(correction.A, -1, 1),
                ClampValue(correction.B, -1, 1),
                ClampValue(correction.C, -1, 1)
            );
        }

    }
}