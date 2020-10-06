using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace PingPong.Devices {
    ///<summary>
    ///Frame received from the KUKA robot
    ///</summary>
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
                    throw new Exception($"Tag <{tag}> not found");
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

        public E6POS Position { get; private set; }

        public long IPOC { get; private set; }

        public int Delay { get; private set; }

        public InputFrame(string data) {
            IPOC = long.Parse(new Tag(data, "IPOC").Value);

            //TODO: obrobienie reszty tagow

            Console.WriteLine($"Received: {data}");
        }

    }
}