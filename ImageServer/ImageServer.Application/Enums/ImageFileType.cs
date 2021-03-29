using System.ComponentModel;

namespace ImageServer.Application.Enums
{
    public enum ImageFileType
    {
        [Description("Default")]
        Default,
        [Description("Jpg")]
        Jpg,
        [Description("Png")]
        Png,
    }
}