using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace PingPong.KUKA {
    /// <summary>
    /// Frame received from the KUKA robot.
    /// </summary>
    public class InputFrame {

        private class Tag {

            public string Value { get; private set; }

            public NameValueCollection Attributes { get; private set; }

            public Tag(string data, string tag) {
                Regex tagRegex = new Regex($"<{tag}([^/>]*)/?>(([^<]*)</{tag}>)?");
                Match match = tagRegex.Match(data);

                if(match.Success) {
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

                foreach(Match match in matches) {
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

            Tag IPOCTag = new Tag(data, "IPOC");
            Tag cartesianPositionTag = new Tag(data, "RIst");

            IPOC = long.Parse(IPOCTag.Value);
            Position = new E6POS(
                double.Parse(cartesianPositionTag.Attributes["X"]),
                double.Parse(cartesianPositionTag.Attributes["Y"]),
                double.Parse(cartesianPositionTag.Attributes["Z"]),
                double.Parse(cartesianPositionTag.Attributes["A"]),
                double.Parse(cartesianPositionTag.Attributes["B"]),
                double.Parse(cartesianPositionTag.Attributes["C"])
            );

            //TODO: Sparsowanie reszty tagow
        }

        public override string ToString() {
            return Data;
        }

    }
}