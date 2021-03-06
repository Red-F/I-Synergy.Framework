﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Model;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Dead;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Live;
using ISynergy.Framework.AspNetCore.WebDav.Server.Utils;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Handlers.Impl.GetResults
{
    internal class WebDavFullDocumentResult : WebDavResult
    {
        private readonly IDocument _document;

        private readonly bool _returnFile;

        public WebDavFullDocumentResult(IDocument document, bool returnFile)
            : base(WebDavStatusCode.OK)
        {
            _document = document;
            _returnFile = returnFile;
        }

        public override async Task ExecuteResultAsync(IWebDavResponse response, CancellationToken ct)
        {
            await base.ExecuteResultAsync(response, ct);

            if (_document.FileSystem.SupportsRangedRead)
                response.Headers["Accept-Ranges"] = new[] { "bytes" };

            var properties = await _document.GetProperties(response.Dispatcher).ToList(ct);
            var etagProperty = properties.OfType<GetETagProperty>().FirstOrDefault();
            if (etagProperty != null)
            {
                var propValue = await etagProperty.GetValueAsync(ct);
                response.Headers["ETag"] = new[] { propValue.ToString() };
            }

            if (!_returnFile)
            {
                var lastModifiedProp = properties.OfType<LastModifiedProperty>().FirstOrDefault();
                if (lastModifiedProp != null)
                {
                    var propValue = await lastModifiedProp.GetValueAsync(ct);
                    response.Headers["Last-Modified"] = new[] { propValue.ToString("R") };
                }

                return;
            }

            using (var stream = await _document.OpenReadAsync(ct))
            {
                using (var content = new StreamContent(stream))
                {
                    // I'm storing the headers in the content, because I'm too lazy to
                    // look up the header names and the formatting of its values.
                    await SetPropertiesToContentHeaderAsync(content, properties, ct);

                    foreach (var header in content.Headers)
                    {
                        response.Headers.Add(header.Key, header.Value.ToArray());
                    }

                    // Use the CopyToAsync function of the stream itself, because
                    // we're able to pass the cancellation token. This is a workaround
                    // for issue dotnet/corefx#9071 and fixes FubarDevelopment/WebDavServer#47.
                    await stream.CopyToAsync(response.Body, 81920, ct)
                        ;
                }
            }
        }

        private async Task SetPropertiesToContentHeaderAsync(HttpContent content, IReadOnlyCollection<IUntypedReadableProperty> properties, CancellationToken ct)
        {
            var lastModifiedProp = properties.OfType<LastModifiedProperty>().FirstOrDefault();
            if (lastModifiedProp != null)
            {
                var propValue = await lastModifiedProp.GetValueAsync(ct);
                content.Headers.LastModified = new DateTimeOffset(propValue);
            }

            var contentLanguageProp = properties.OfType<GetContentLanguageProperty>().FirstOrDefault();
            if (contentLanguageProp != null)
            {
                var propValue = await contentLanguageProp.TryGetValueAsync(ct);
                if (propValue.Item1)
                    content.Headers.ContentLanguage.Add(propValue.Item2);
            }

            string contentType;
            var contentTypeProp = properties.OfType<GetContentTypeProperty>().FirstOrDefault();
            if (contentTypeProp != null)
            {
                contentType = await contentTypeProp.GetValueAsync(ct);
            }
            else
            {
                contentType = MimeTypesMap.DefaultMimeType;
            }

            content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = _document.Name,
                FileNameStar = _document.Name,
            };

            content.Headers.ContentDisposition = contentDisposition;
        }
    }
}
