﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Build.Engine.Incrementals
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.DocAsCode.Common;
    using Microsoft.DocAsCode.Plugins;

    public static class IncrementalUtility
    {
        private const int MaxRetry = 3;

        public static T LoadIntermediateFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return default(T);
            }
            using (var reader = new StreamReader(fileName))
            {
                return JsonUtility.Deserialize<T>(reader);
            }
        }

        public static DependencyGraph LoadDependency(string dependencyFile)
        {
            if (string.IsNullOrEmpty(dependencyFile))
            {
                return null;
            }
            using (var reader = new StreamReader(dependencyFile))
            {
                return DependencyGraph.Load(reader);
            }
        }

        public static void SaveDependency(string fileName, DependencyGraph dg)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (dg == null)
            {
                return;
            }
            using (var writer = new StreamWriter(fileName))
            {
                dg.Save(writer);
            }
        }

        public static void SaveIntermediateFile<T>(string fileName, T content)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            using (var writer = new StreamWriter(fileName))
            {
                JsonUtility.Serialize(writer, content);
            }
        }

        public static BuildMessage LoadBuildMessage(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }
            using (var reader = new StreamReader(file))
            {
                var bm = new BuildMessage();
                var content = JsonUtility.Deserialize<Dictionary<BuildPhase, string>>(reader);
                foreach (var c in content)
                {
                    using (var sr = new StringReader(c.Value))
                    {
                        bm[c.Key] = BuildMessageInfo.Load(sr);
                    }
                }
                return bm;
            }
        }

        public static void SaveBuildMessage(string fileName, BuildMessage bm)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if (bm == null)
            {
                return;
            }
            using (var writer = new StreamWriter(fileName))
            {
                JsonUtility.Serialize(
                    writer,
                    bm.ToDictionary(
                        p => p.Key,
                        p =>
                        {
                            using (var sw = new StringWriter())
                            {
                                p.Value.Save(sw);
                                return sw.ToString();
                            }
                        }));
            }
        }

        public static string GetDependencyKey(FileAndType file)
        {
            if (file == null)
            {
                return null;
            }
            return ((RelativePath)file.File).GetPathFromWorkingFolder().ToString();
        }

        public static string GetRandomEntry(string baseDir)
        {
            string name;
            do
            {
                name = Path.GetRandomFileName();
            } while (Directory.Exists(Path.Combine(baseDir, name)) || File.Exists(Path.Combine(baseDir, name)));
            return name;
        }

        public static string CreateRandomFileName(string baseDir) =>
            RetryIO(() =>
            {
                string fileName = GetRandomEntry(baseDir);
                using (File.Create(Path.Combine(baseDir, fileName)))
                {
                    // create new zero length file.
                }
                return fileName;
            });

        public static FileStream CreateRandomFileStream(string baseDir) =>
            RetryIO(() => File.Create(Path.Combine(baseDir, GetRandomEntry(baseDir))));

        public static string CreateRandomDirectory(string baseDir) =>
            RetryIO(() =>
            {
                var folderName = GetRandomEntry(baseDir);
                Directory.CreateDirectory(Path.Combine(baseDir, folderName));
                return folderName;
            });

        public static T RetryIO<T>(Func<T> func)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    return func();
                }
                catch (IOException)
                {
                    if (count++ >= MaxRetry)
                    {
                        throw;
                    }
                }
            }
        }

        public static void RetryIO(Action action)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    action();
                    return;
                }
                catch (IOException)
                {
                    if (count++ >= MaxRetry)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
