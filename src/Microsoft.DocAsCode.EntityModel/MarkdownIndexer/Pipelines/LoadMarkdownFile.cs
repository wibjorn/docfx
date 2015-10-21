// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.EntityModel.MarkdownIndexer
{
    using System;
    using System.IO;
    using System.Linq;

    using Microsoft.DocAsCode.Utility;
    using Microsoft.DocAsCode.EntityModel.ViewModels;

    public class LoadMarkdownFile : IIndexerPipeline
    {
        public ParseResult Run(MapFileItemViewModel item, IndexerContext context)
        {
            if (string.IsNullOrEmpty(context.MarkdownFileSourcePath))
            {
                return new ParseResult(ResultLevel.Error, "Markdown file source path should be specified!");
            }

            if (string.IsNullOrEmpty(context.CurrentWorkingDirectory)) context.CurrentWorkingDirectory = Environment.CurrentDirectory;

            var targetFiles = new string[] { context.MarkdownFileSourcePath }.CopyFilesToFolder(context.CurrentWorkingDirectory, context.TargetFolder, true, s => Logger.Log(LogLevel.Info, s), s => { Logger.Log(LogLevel.Warning, $"Unable to copy file: {s}, ignored."); return true; });
            var targetFile = targetFiles?.FirstOrDefault() ?? context.MarkdownFileSourcePath;

            context.MarkdownContent = File.ReadAllText(targetFile);
            context.MarkdownFileTargetPath = targetFile;

            if (!string.IsNullOrEmpty(context.MarkdownFileSourcePath))
            {
                item.Remote = GitUtility.GetGitDetail(context.MarkdownFileSourcePath);
            }

            return new ParseResult(ResultLevel.Success);
        }
    }
}
