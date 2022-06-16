namespace InspectorGadget.WebApp.Infrastructure
{
    public class SpiffeJwtSvid
    {
        public string SpiffeId { get; set; }
        public string Jwt { get; set; }
        public string Hint { get; set; }

        public SpiffeJwtSvid()
        {
        }

        public SpiffeJwtSvid(string spiffeId, string jwt, string hint)
        {
            this.SpiffeId = spiffeId;
            this.Jwt = jwt;
            this.Hint = hint;
        }
    }
}