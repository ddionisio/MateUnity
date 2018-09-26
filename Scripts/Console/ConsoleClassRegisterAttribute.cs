using System;

namespace M8 {
    /// <summary>
    /// Add this to a class to register all its public static functions to Console
    /// </summary>
    public class ConsoleClassRegisterAttribute : Attribute {
        /// <summary>
        /// Set this to allow Console to filter this class (will need to match in Console.tags)
        /// </summary>
        public string tag = "";
    }
}