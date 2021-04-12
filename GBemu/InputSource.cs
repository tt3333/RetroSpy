using System;

namespace GBPemu
{
    public class InputSource
    {
         public static readonly InputSource PRINTER = new InputSource("printer", "Nintendo GameBoy Printer", true, false, false, false, port => new GameBoyPrinterReader(port, GameBoyPrinter.ReadFromPacket));

        public static readonly InputSource DEFAULT = PRINTER;

        public string TypeTag { get; private set; }
        public string Name { get; private set; }
        public bool RequiresComPort { get; private set; }
        public bool RequiresComPort2 { get; private set; }
        public bool RequiresId { get; private set; }
        public bool RequiresHostname { get; private set; }

        public Func<string, IControllerReader> BuildReader { get; private set; }

        public Func<string, string, IControllerReader> BuildReader2 { get; private set; }

        public IControllerReader BuildReader3 { get; private set; }

        private InputSource(string typeTag, string name, bool requiresComPort, bool requiresId, bool requiresHostname, bool requiresComPort2, Func<string, IControllerReader> buildReader)
        {
            TypeTag = typeTag;
            Name = name;
            RequiresComPort = requiresComPort;
            RequiresComPort2 = requiresComPort2;
            RequiresId = requiresId;
            RequiresHostname = requiresHostname;
            BuildReader = buildReader;
        }
    }
}