using System;

namespace M8 {
    public class ConsoleCommandAttribute : Attribute {
        /// <summary>
        /// Set this to allow Console to filter this command (will need to match in Console.tags)
        /// </summary>
        public string tag = "";

        /// <summary>
        /// The command to use in console; if empty, use function name
        /// </summary>
        public string alias = "";

        /// <summary>
        /// Help display when executed with  "/?"
        /// </summary>
        public string hint = "";
    }
}