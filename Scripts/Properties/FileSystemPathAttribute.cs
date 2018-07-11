using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Inspector helper for grabbing file path from the OS
    /// </summary>
    public class FileSystemPathAttribute : PropertyAttribute {
        public string fileDialogTitle { get; set; }
        public string fileExtension { get; set; }

        public FileSystemPathAttribute(string fileDialogTitle, string fileExtension) {
            this.fileDialogTitle = fileDialogTitle;
            this.fileExtension = fileExtension;
        }
    }
}