using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace UOStudio.Client.Network
{
    public sealed class HttpClientProgressWrapper
    {
        private readonly string _downloadUrl;
        private readonly string _destinationFilePath;
        private readonly Guid _projectId;

        private readonly HttpClient _httpClient;

        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage, Guid projectId, string destinationFilePath);

        public event ProgressChangedHandler ProgressChanged;

        public event Action<Guid, string> ProgressCompleted;

        public HttpClientProgressWrapper(HttpClient httpClient, string downloadUrl, string destinationFilePath, Guid projectId)
        {
            _httpClient = httpClient;
            _downloadUrl = downloadUrl;
            _destinationFilePath = destinationFilePath;
            _projectId = projectId;
        }

        public async Task StartDownload()
        {
            using var response = await _httpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            await DownloadFileFromHttpResponseMessage(response);
        }

        private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await ProcessContentStream(totalBytes, contentStream);
        }

        private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
        {
            const int BufferSize = 8192;
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[BufferSize];
            var isMoreToRead = true;

            await using (var fileStream = new FileStream(
                _destinationFilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                true
            ))
            {
                do
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    readCount += 1;

                    //if (readCount % 100 == 0)
                    {
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                    }
                } while (isMoreToRead);
            }

            TriggerProgressCompleted();
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (ProgressChanged == null)
            {
                return;
            }

            double? progressPercentage = null;
            if (totalDownloadSize.HasValue)
            {
                progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);
            }

            ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage, _projectId, _destinationFilePath);
        }

        private void TriggerProgressCompleted()
        {
            var progressCompleted = ProgressCompleted;
            progressCompleted?.Invoke(_projectId, _destinationFilePath);
        }
    }
}
