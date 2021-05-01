using CG.Diagnostics;
using CG.Obsidian.Models;
using CG.Obsidian.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CG.Obsidian.Managers
{
    /// <summary>
    /// This class is a test fixture for the <see cref="MimeTypeManager"/>
    /// class.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
    public class MimeTypeManagerFixture
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method ensures the <see cref="MimeTypeManager.MimeTypeManager(Stores.IMimeTypeStore, Stores.IFileExtensionStore, Microsoft.Extensions.Logging.ILogger{MimeTypeManager})"/>
        /// constructor properly initializes object instances.
        /// </summary>
        [TestMethod]
        public void MimeTypeManager_ctor()
        {
            // Arrange ...
            var mimeStore = new Mock<IMimeTypeStore>();
            var fileExtensionStore = new Mock<IFileExtensionStore>();
            var logger = new Mock<ILogger<MimeTypeManager>>();

            // Act ...
            var result = new MimeTypeManager(
                mimeStore.Object,
                fileExtensionStore.Object,
                logger.Object
                );

            // Assert ...
            Assert.IsTrue(
                result.MimeTypes == mimeStore.Object,
                "The mime types property wasn't properly initialized."
                );
            Assert.IsTrue(
                result.FileExtensions == fileExtensionStore.Object,
                "The file extension property wasn't properly initialized."
                );
            Assert.IsTrue(
                result.GetPropertyValue("Logger", true) == logger.Object,
                "The file extension property wasn't properly initialized."
                );
        }

        // *******************************************************************

        /// <summary>
        /// This method ensures the <see cref="MimeTypeManager.FindByExtensionAsync(string, System.Threading.CancellationToken)"/>
        /// requests the queryable object from the mime type store and uses that
        /// object to query for a mime type with a matching file extension.
        /// </summary>
        [TestMethod]
        public async Task MimeTypeManager_FindByExtensionAsync()
        {
            // Arrange ...
            var mimeStore = new Mock<IMimeTypeStore>();
            var fileExtensionStore = new Mock<IFileExtensionStore>();
            var logger = new Mock<ILogger<MimeTypeManager>>();

            var mimeTypes = new MimeType[]
            {
                new MimeType() 
                { 
                    Id = 1, 
                    Type = "image", 
                    SubType = "jpg",
                    Extensions = new List<FileExtension>()
                    {
                        new FileExtension()
                        {
                            Id = 1,
                            MimeTypeId = 1,
                            Extension = ".jpg"
                        }
                    }
                }
            };

            mimeStore.Setup(x => x.AsQueryable())
                .Returns(mimeTypes.AsQueryable())
                .Verifiable();

            var manager = new MimeTypeManager(
                mimeStore.Object,
                fileExtensionStore.Object,
                logger.Object
                );

            // Act ...
            var result = await manager.FindByExtensionAsync(
                ".jpg"
                ).ConfigureAwait(false);

            // Assert ...
            Assert.IsTrue(
                result.Count() == 1,
                "The return count was invalid."
                );
            Assert.IsTrue(
                result.ElementAt(0) == mimeTypes[0],
                "The return value was invalid."
                );

            Mock.Verify(mimeStore);
        }

        #endregion
    }
}
