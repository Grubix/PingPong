using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace PingPong.KUKA {
    /// <summary>
    /// Represents frame (data) received from the KUKA robot.
    /// </summary>
    public class InputFrame {

        private class Tag {

            public string Value { get; private set; }

            public NameValueCollection Attributes { get; private set; }

            public Tag(string data, string tag) {
                Regex tagRegex = new Regex($"<{tag}([^/>]*)/?>(([^<]*)</{tag}>)?");
                Match match = tagRegex.Match(data);

                if (match.Success) {
                    Value = match.Groups[3].Value.Trim();
                    Attributes = ExtractAttributes(match.Groups[1].Value.Trim());
                } else {
                    throw new Exception($"Tag <{tag}> not found in data");
                }
            }

            private NameValueCollection ExtractAttributes(string attributesString) {
                NameValueCollection attributes = new NameValueCollection();

                if (string.IsNullOrEmpty(attributesString)) {
                    return attributes;
                }

                Regex attributeRegex = new Regex("([a-zA-Z0-9_]+)[ ]*=[ ]*\"([^\"]*)\"");
                MatchCollection matches = attributeRegex.Matches(attributesString);

                foreach (Match match in matches) {
                    attributes[match.Groups[1].Value.Trim()] = match.Groups[2].Value;
                }

                return attributes;
            }

        }

        public string Data { get; }

        public E6POS Position { get; }

        public long IPOC { get; }

        public InputFrame(string data) {
            Data = data;
            IPOC = long.Parse(new Tag(data, "IPOC").Value);

            Tag positionTag = new Tag(data, "RIst");
            double X = double.Parse(positionTag.Attributes["X"]);
            double Y = double.Parse(positionTag.Attributes["Y"]);
            double Z = double.Parse(positionTag.Attributes["Z"]);
            double A = double.Parse(positionTag.Attributes["A"]);
            double B = double.Parse(positionTag.Attributes["B"]);
            double C = double.Parse(positionTag.Attributes["C"]);

            A = A < 0 ? 360.0 + A : A;
            B = B < 0 ? 360.0 + B : B;
            C = C < 0 ? 360.0 + C : C;

            Position = new E6POS(X, Y, Z, A, B, C);
        }

        public override string ToString() {
            return Data;
        }

    }
}