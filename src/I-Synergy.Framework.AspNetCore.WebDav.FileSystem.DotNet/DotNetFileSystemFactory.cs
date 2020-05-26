﻿using System.IO;
using System.Security.Principal;
using ISynergy.Framework.AspNetCore.WebDav.Server.FileSystem;
using ISynergy.Framework.AspNetCore.WebDav.Server.Locking;
using ISynergy.Framework.AspNetCore.WebDav.Server.Props.Store;
using Microsoft.Extensions.Options;

namespace ISynergy.Framework.AspNetCore.WebDav.FileSystem.DotNet
{
    /// <summary>
    /// The factory creating/getting the file systems that use <see cref="System.IO"/> for its implementation
    /// </summary>
    public class DotNetFileSystemFactory : IFileSystemFactory
    {
        private readonly IPathTraversalEngine _pathTraversalEngine;

        private readonly IPropertyStoreFactory _propertyStoreFactory;

        private readonly ILockManager _lockManager;

        private readonly DotNetFileSystemOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystemFactory"/> class.
        /// </summary>
        /// <param name="options">The options for this file system</param>
        /// <param name="pathTraversalEngine">The engine to traverse paths</param>
        /// <param name="propertyStoreFactory">The store for dead properties</param>
        /// <param name="lockManager">The global lock manager</param>
        public DotNetFileSystemFactory(
            IOptions<DotNetFileSystemOptions> options,
            IPathTraversalEngine pathTraversalEngine,
            IPropertyStoreFactory propertyStoreFactory = null,
            ILockManager lockManager = null)
        {
            _pathTraversalEngine = pathTraversalEngine;
            _propertyStoreFactory = propertyStoreFactory;
            _lockManager = lockManager;
            _options = options.Value;
        }

        /// <inheritdoc />
        public virtual IFileSystem CreateFileSystem(ICollection mountPoint, IPrincipal principal)
        {
            var rootFileSystemPath = Server.Utils.SystemInfo.GetUserHomePath(
                principal,
                homePath: _options.RootPath,
                anonymousUserName: _options.AnonymousUserName);

            Directory.CreateDirectory(rootFileSystemPath);

            return new DotNetFileSystem(_options, mountPoint, rootFileSystemPath, _pathTraversalEngine, _lockManager, _propertyStoreFactory);
        }
    }
}
