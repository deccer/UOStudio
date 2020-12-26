using System.IO;

namespace UOStudio.Web.Helper
{
    public class MultipartContent
    {
        public string ContentType { get; set; }

        public string FileName { get; set; }

        public Stream Stream { get; set; }
    }
}
