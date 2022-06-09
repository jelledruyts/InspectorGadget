namespace InspectorGadget.WebApp.Infrastructure
{
    public class SpiffeJwtSvid
    {
        public string SpiffeId { get; set; }
        public string Svid { get; set; }
        public string Hint { get; set; }

        public SpiffeJwtSvid()
        {
        }

        public SpiffeJwtSvid(string spiffeId, string svid, string hint)
        {
            this.SpiffeId = spiffeId;
            this.Svid = svid;
            this.Hint = hint;
        }
    }
}