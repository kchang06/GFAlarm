using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationResources
{
    class Settings
    {
#if DEBUG
        public const string OUTPUT_PATH = "bin/Debug/";
#else
        public const string OUTPUT_PATH = "bin/Release/";
#endif
        public const string RESOURCE_NOT_FOUND_MESSAGE = "{0}";

        public const string LANGUAGE_FILE_PATH = "{0}\\Resource\\Language\\{1}.json";

        public const string TARGET_PROJECT_NAME = "GFAlarm/";
    }
}
