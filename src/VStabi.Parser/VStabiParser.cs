namespace VStabiParser
{
    using global::VStabiParser.Interfaces;

    public partial class VStabiParser
    {
        private readonly IVStabiReader vstabiReader;

        public VStabiParser(IVStabiReader vstabiReader)
        {
            this.vstabiReader = vstabiReader;
        }
    }
}