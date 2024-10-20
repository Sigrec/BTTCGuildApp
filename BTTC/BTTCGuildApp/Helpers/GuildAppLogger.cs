using System.IO;
using NLog;
using NLog.Config;

namespace BTTCGuildApp.Helpers
{
    public static class GuildAppLogger
    {
        public static readonly Logger LOGGER;
        static GuildAppLogger()
		{
			var stream = typeof(Program).Assembly.GetManifestResourceStream("BTTCGuildApp.NLog.config");
			string xml;
            using (var reader = new StreamReader(stream))
			{
				xml = reader.ReadToEnd();
			}
			LogManager.Configuration = XmlLoggingConfiguration.CreateFromXmlString(xml);
            LOGGER = LogManager.GetLogger("BTTCGuildAppLogs");
		}
    }
}