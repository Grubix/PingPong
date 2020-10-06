namespace Resilio_Project {
    static class RequestString {

        private static string request;

        static public void setRequest(string data) {
            request = data;
        }

        static public long getDelay() {
            return StringOperations.getLongBetween(request, "<Delay D=\"", "\" />");
        }

        static public long getIPOC() {
            return StringOperations.getLongBetween(request, "<IPOC>", "</IPOC>");
        }

    }
}
