﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;

namespace ISynergy.Framework.AspNetCore.WebDav.Server.Engines.Remote
{
    /// <summary>
    /// The <see cref="ITargetActions{TCollection,TDocument,TMissing}"/> implementation that moves entries between servers
    /// </summary>
    public class MoveRemoteHttpClientTargetActions : RemoteHttpClientTargetActions, IRemoteMoveTargetActions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoveRemoteHttpClientTargetActions"/> class.
        /// </summary>
        /// <param name="dispatcher">The WebDAV dispatcher</param>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use</param>
        public MoveRemoteHttpClientTargetActions(IWebDavDispatcher dispatcher, HttpClient httpClient)
            : base(dispatcher, httpClient)
        {
        }

        /// <inheritdoc />
        public override async Task<RemoteDocumentTarget> ExecuteAsync(IDocument source, RemoteMissingTarget destination, CancellationToken cancellationToken)
        {
            using (var stream = await source.OpenReadAsync(cancellationToken))
            {
                var content = new StreamContent(stream);
                using (var response = await Client
                    .PutAsync(destination.DestinationUrl, content, cancellationToken)
                    )
                {
                    response.EnsureSuccessStatusCode();
                }
            }

            await source.DeleteAsync(cancellationToken);

            return new RemoteDocumentTarget(destination.Parent, destination.Name, destination.DestinationUrl, this);
        }

        /// <inheritdoc />
        public override async Task<ActionResult> ExecuteAsync(IDocument source, RemoteDocumentTarget destination, CancellationToken cancellationToken)
        {
            try
            {
                using (var stream = await source.OpenReadAsync(cancellationToken))
                {
                    var content = new StreamContent(stream);
                    var request = new HttpRequestMessage(HttpMethod.Put, destination.DestinationUrl)
                    {
                        Content = content,
                        Headers =
                        {
                            { "Overwrite", "T" },
                        },
                    };

                    using (var response = await Client
                        .SendAsync(request, cancellationToken)
                        )
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
            catch (Exception ex)
            {
                return new ActionResult(ActionStatus.OverwriteFailed, destination)
                {
                    Exception = ex,
                };
            }

            try
            {
                await source.DeleteAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                return new ActionResult(ActionStatus.CleanupFailed, destination)
                {
                    Exception = ex,
                };
            }

            return new ActionResult(ActionStatus.Overwritten, destination);
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(ICollection source, RemoteCollectionTarget destination, CancellationToken cancellationToken)
        {
            return source.DeleteAsync(cancellationToken);
        }
    }
}
