using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UOStudio.Web.Helper
{
    public class MultipartResult : Collection<MultipartContent>, IActionResult
    {
        private readonly System.Net.Http.MultipartContent _content;

        public MultipartResult(string subtype = "byteranges", string boundary = null) => _content = boundary == null
            ? new System.Net.Http.MultipartContent(subtype)
            : new System.Net.Http.MultipartContent(subtype, boundary);

        public async Task ExecuteResultAsync(ActionContext context)
        {
            foreach (var item in this)
            {
                if (item.Stream != null)
                {
                    var content = new StreamContent(item.Stream);
                    if (item.ContentType != null)
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue(item.ContentType);
                    }

                    if (item.FileName != null)
                    {
                        var contentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = item.FileName
                        };
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = contentDisposition.FileName,
                            FileNameStar = contentDisposition.FileNameStar
                        };
                    }

                    _content.Add(content);
                }
            }

            context.HttpContext.Response.ContentLength = _content.Headers.ContentLength;
            context.HttpContext.Response.ContentType = _content.Headers.ContentType!.ToString();

            await _content.CopyToAsync(context.HttpContext.Response.Body);
        }
    }
}
