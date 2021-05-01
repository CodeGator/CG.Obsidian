using CG.Validations;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace CG.Obsidian.SqlServer
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="ObsidianDbContext"/>
    /// type.
    /// </summary>
    /// <remarks>
    /// This class contains <see cref="ObsidianDbContext"/> related operations that should
    /// only be called from within the <see cref="CG.Obsidian.SqlServer"/> library.
    /// </remarks>
    internal static partial class DbContextExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method applies seed data to the specified data-context.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        /// <param name="logger">The logger to use for the operation.</param>
        public static void ApplySeedData(
            this ObsidianDbContext context,
            ILogger<ObsidianDbContext> logger
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(context, nameof(context));

            // Tell the world what we are doing.
            logger.LogInformation($"Starting seeding operations on: {context.ContextId}");

            // Add data to the tables.
            context.SeedMimeTypes(logger);
            context.SeedFileExtensions(logger);

            // Tell the world what we are doing.
            logger.LogInformation($"Finished seeding operations on: {context.ContextId}");
        }

        #endregion

        // *******************************************************************
        // Private methods.
        // *******************************************************************

        #region Private methods

        /// <summary>
        /// This method applies seed data to the [Obsidian].[MimeTypes] table.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        /// <param name="logger">The logger to use for the operation.</param>
        private static void SeedMimeTypes(
            this ObsidianDbContext context,
            ILogger<ObsidianDbContext> logger
            )
        {
            // Don't seed an already populated table.
            if (true == context.MimeTypes.Any())
            {
                // Tell the world what we are doing.
                logger.LogInformation($"Skipping seeding operation on: [Obsidian].[MimeTypes].");
                return;
            }

            // Tell the world what we are doing.
            logger.LogInformation($"Starting seeding operation on: [Obsidian].[MimeTypes]. This could take a bit ...");

            int docs = 0;
            long errors = 0;
            long skipped = 0;
            long rows = 0;

            // Create a handler.
            using (var handler = new HttpClientHandler())
            {
                // Use default credentials.
                handler.UseDefaultCredentials = true;

                // Use cookies.
                handler.UseCookies = true;

                // Create an HTTP client.
                using (var client = new HttpClient(handler))
                {
                    // Set the user agent.
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36"
                        );

                    // Set the base address.
                    client.BaseAddress = new Uri("http://www.iana.org/assignments/media-types/");

                    // We'll need several documents ...
                    var endpoints = new string[] 
                    {
                        "application.csv",
                        "audio.csv",
                        "font.csv",
                        "image.csv",
                        "message.csv",
                        "model.csv",
                        "multipart.csv",
                        "text.csv",
                        "video.csv"
                    };

                    // Loop and process documents.
                    foreach (var endpoint in endpoints)
                    {
                        try
                        {
                            // Tell the world what we are doing.
                            logger.LogInformation($"Downloading: {endpoint} ...");

                            // Get the CSV.
                            var csv = client.GetStringAsync(endpoint).Result;

                            // Did we fail?
                            if (string.IsNullOrEmpty(csv))
                            {
                                // Track the work.
                                errors++;

                                continue; // Nothing left to do.
                            }

                            try
                            {
                                // Parse the CSV into lines.
                                var lines = csv.Split(Environment.NewLine);

                                // Loop through the lines (skipping the header).
                                foreach (var line in lines.Skip(1))
                                {
                                    // Parse the line into fields.
                                    var fields = line.Split(',');

                                    // Skip any malformed lines.
                                    if (3 != fields.Length)
                                    {
                                        // Track the work.
                                        skipped++;

                                        continue; // Nothing left to do.
                                    }

                                    var parts = new string[0];

                                    // Is the second field empty?
                                    if (fields[1].Trim() == "")
                                    {
                                        // Oddity in the iana.org docs to deal with.
                                        parts = new string[]
                                        {
                                            Path.GetFileNameWithoutExtension(endpoint),
                                            fields[0]
                                        };
                                    }
                                    else
                                    {
                                        // Split the mimetype into parts.
                                        parts = fields[1].Split('/');
                                    }

                                    // Skip any malformed fields.
                                    if (2 != parts.Length)
                                    {
                                        // Track the work.
                                        skipped++;

                                        continue; // Nothing left to do.
                                    }

                                    // Is the record already in the db?
                                    if (context.MimeTypes.Any(x => 
                                        x.Type == parts[0] && x.SubType == parts[1]))
                                    {
                                        // Track the work.
                                        skipped++;

                                        continue; // Nothing left to do.
                                    }

                                    // Add data to the table.
                                    context.Add(new Models.MimeType
                                    {
                                        CreatedBy = "seed",
                                        CreatedDate = DateTime.Now,
                                        Type = parts[0],
                                        SubType = parts[1],
                                        Description = fields[0]
                                    });

                                    // Save the changes.
                                    context.SaveChanges();

                                    // Track the work.
                                    rows++;
                                }
                            }
                            catch (Exception ex)
                            {
                                // Track the work.
                                errors++;

                                // Tell the world what we happened.
                                logger.LogError($"Failed to process: {endpoint}", ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Track the work.
                            errors++;

                            // Tell the world what we happened.
                            logger.LogError($"Failed to download: {endpoint}", ex);
                        }

                        // Track the work.
                        docs++;
                    }
                }
            }

            // Tell the world what we did.
            logger.LogInformation(
                $"Finished seeding [Obsidian].[MimeTypes]. " +
                $"docs: {docs}, rows: {rows}, skipped: {skipped}, errors: {errors}"
                );
        }

        // *******************************************************************

        /// <summary>
        /// This method applies seed data to the [Obsidian].[FileExtensions] table.
        /// </summary>
        /// <param name="context">The data-context to use for the operation.</param>
        /// <param name="logger">The logger to use for the operation.</param>
        private static void SeedFileExtensions(
            this ObsidianDbContext context,
            ILogger<ObsidianDbContext> logger
            )
        {
            // Don't seed an already populated table.
            if (true == context.FileExtensions.Any())
            {
                return;
            }

            // TODO : try to find this list online ...

            // Data for seeding purposes.
            var cannedData = new (string, string, string)[]
            {
                new (".aac", "audio", "aac"),
                new (".abw", "application", "x-abiword"),
                new (".arc", "application", "x-freearc"),
                new (".avi", "video", "x-msvideo"),
                new (".azw", "application", "vnd.amazon.ebook"),
                new (".bin", "application", "octet-stream"),
                new (".bmp", "image", "bmp"),
                new (".bz", "application", "x-bzip"),
                new (".bz2", "application", "x-bzip2"),
                new (".csh", "application", "x-csh"),
                new (".css", "text", "css"),
                new (".csv", "text", "csv"),
                new (".doc", "application", "vnd.openxmlformats-officedocument.wordprocessingml.document"),
                new (".eot", "application", "vnd.ms-fontobject"),
                new (".epub", "application", "epub+zip"),
                new (".gz", "application", "gzip"),
                new (".gif", "image", "gif"),
                new (".htm", "text", "html"),
                new (".html", "text", "html"),
                new (".ico", "image", "vnd.microsoft.icon"),
                new (".ics", "text", "calendar"),
                new (".jar", "application", "java-archive"),
                new (".jpg", "image", "jpeg"),
                new (".jpeg", "image", "jpeg"),
                new (".js", "text", "javascript"),
                new (".json", "application", "json"),
                new (".jsonld", "application", "ld+json"),
                new (".mid", "audio", "midi"),
                new (".midi", "audio", "x-midi"),
                new (".mjs", "text", "javascript"),
                new (".mp3", "audio", "mpeg"),
                new (".cda", "audio", "x-cdf"),
                new (".mp4", "video", "mp4"),
                new (".mpeg", "video", "mpeg"),
                new (".mpkg", "application", "vnd.apple.installer+xml"),
                new (".odp", "application", "vnd.oasis.opendocument.presentation"),
                new (".ods", "application", "vnd.oasis.opendocument.spreadsheet"),
                new (".odt", "application", "vnd.oasis.opendocument.text"),
                new (".oga", "audio", "ogg"),
                new (".ogv", "video", "ogg"),
                new (".ogx", "application", "ogg"),
                new (".opus", "audio", "opus"),
                new (".otf", "font", "otc"),
                new (".png", "image", "png"),
                new (".pdf", "application", "pdf"),
                new (".php", "application", "x-httpd-php"),
                new (".ppt", "application", "vnd.ms-powerpoint"),
                new (".pptx", "application", "vnd.openxmlformats-officedocument.presentationml.presentation"),
                new (".rar", "application", "vnd.rar"),
                new (".rtf", "application", "rtf"),
                new (".sh", "application", "x-sh"),
                new (".svg", "image", "svg+xml"),
                new (".swf", "application", "x-shockwave-flash"),
                new (".tar", "application", "x-tar"),
                new (".tif", "image", "tiff"),
                new (".tiff", "image", "tiff"),
                new (".ts", "video", "ts"),
                new (".ttf", "font", "ttf"),
                new (".text", "text", "plain"),
                new (".vsd", "application", "vnd.visio"),
                new (".wav", "audio", "wav"),
                new (".weba", "audio", "webm"),
                new (".webm", "audio", "webm"),
                new (".webp", "audio", "webp"),
                new (".woff", "font", "woff"),
                new (".woff2", "font", "woff2"),
                new (".xhtml", "application", "xhtml+xml"),
                new (".xls", "application", "vnd.ms-excel"),
                new (".xlsx", "application", "vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                new (".xml", "text", "xml"),
                new (".xul", "application", "vnd.mozilla.xul+xml"),
                new (".zip", "application", "zip"),
                new (".3gp", "video", "3gpp"),
                new (".3g2", "video", "3gpp2"),
                new (".7z", "application", "x-7z-compressed")
            };

            // Tell the world what we are doing.
            logger.LogInformation($"Starting seeding operation on: [Obsidian].[FileExtensions].");

            long skipped = 0;
            long rows = 0;
            long errors = 0;

            try
            {
                // Loop and seed file associations.
                foreach (var row in cannedData)
                {
                    // Is the record already in the db?
                    if (context.FileExtensions.Any(x =>
                        x.Extension == row.Item1
                        ))
                    {
                        // Track the work.
                        skipped++;

                        continue; // Nothing left to do.
                    }

                    // Look for the matching mime type.
                    var mimeType = context.MimeTypes.FirstOrDefault(x =>
                        x.Type == row.Item2 && x.SubType == row.Item3
                        );

                    // Ignore any we can't find.
                    if (null == mimeType)
                    {
                        // Track the work.
                        skipped++;

                        continue; // Nothing left to do.
                    }

                    // Add the association.
                    context.Add(new Models.FileExtension()
                    {
                        CreatedBy = "seed",
                        CreatedDate = DateTime.Now,
                        Extension = row.Item1,
                        MimeTypeId = mimeType.Id
                    });

                    // Save the changes.
                    rows += context.SaveChanges();
                }                
            }
            catch (Exception ex)
            {
                // Track the work.
                errors++;
            }

            // Tell the world what we did.
            logger.LogInformation(
                $"Finished seeding [Obsidian].[FileExtensions]. " +
                $"rows: {rows}, skipped: {skipped}, errors: {errors}"
                );
        }

        #endregion
    }
}
