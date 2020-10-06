namespace Resilio_Project {
    static class ResponseString {

        //TODO: Co to sa przepraszam te AKorr i EKorr ??

        private static string response;
        private static string preamble = "<Sen Type=\"Resilio_Project\"><EStr>Resilio Project</EStr>";
        private static string rkorr = "<RKorr X=\"0.0000\" Y=\"0.0000\" Z=\"0.0000\" A=\"0.0000\" B=\"0.0000\" C=\"0.0000\" />";
        private static string akorr = "<AKorr A1=\"0.0000\" A2=\"0.0000\" A3=\"0.0000\" A4=\"0.0000\" A5=\"0.0000\" A6=\"0.0000\" />";
        private static string ekorr = "<EKorr E1=\"0.0000\" E2=\"0.0000\" E3=\"0.0000\" E4=\"0.0000\" E5=\"0.0000\" E6=\"0.0000\" />";
        private static string tech = "<Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\" />";
        private static string zmienna = "<Zmienna>1</Zmienna>";
        private static string ipoc = "<IPOC></IPOC>";
        private static string postamble = "</Sen>";

        public static void UpdateIPOC(long IPOC) {
            ipoc = "<IPOC>" + IPOC.ToString() + "</IPOC>";
        }

        public static string getString() {
            response = preamble + rkorr + akorr + ekorr + tech + zmienna + ipoc + postamble;
            return response;
        }

    }
}
