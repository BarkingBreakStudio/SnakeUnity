using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using UnityEngine;

namespace NorLib
{
    public class SaveSystem
    {

        public static FileType DefaultFileType = new JsonHrFile();

        /// <summary>
        /// Save Object of type T in a file
        /// </summary>
        /// <typeparam name="T">object type to save</typeparam>
        /// <param name="key">key to load object</param>
        /// <param name="data">actual data to store</param>
        /// <param name="saveGame">name of the save game, "" for global settings</param>
        /// <param name="ftype">JsonFile or XMLFile, null for defalt file type</param>
        public static void SaveObject<T>(string key, T data, string saveGame = "", FileType ftype = null) where T : class
        {
            string saveDirectory = GetSaveGameDirectory();
            string saveFolder = GetSaveFolder(saveDirectory, saveGame);

            if (ftype == null)
            {
                ftype = DefaultFileType;
            }

            ftype.SaveObject<T>(key, data, saveFolder);
        }

        /// <summary>
        /// Load Object of type T from a file
        /// </summary>
        /// <typeparam name="T">object type to load</typeparam>
        /// <param name="key">key the object was saved with</param>
        /// <param name="saveGame">name of the save game, "" for global settings</param>
        /// <param name="ftype"></param>
        /// <returns>JsonFile or XMLFile, null for defalt file type</returns>
        public static T LoadObject<T>(string key, string saveGame = "", FileType ftype = null) where T : class
        {
            string saveDirectory = GetSaveGameDirectory();
            string saveFolder = GetSaveFolder(saveDirectory, saveGame);

            if (ftype == null)
            {
                ftype = DefaultFileType;
            }

            return ftype.LoadObject<T>(key, saveFolder);
        }

        /// <summary>
        /// Get all available save games
        /// </summary>
        /// <returns>a list of game saves</returns>
        public static string[] GetAvailableSaveGames()
        {
            string saveDirectory = GetSaveGameDirectory();
            return Directory.GetDirectories(saveDirectory).Select(d => new DirectoryInfo(d).Name).ToArray();
        }

        private static string GetSaveGameDirectory()
        {
            string folder = Application.persistentDataPath + "/saves/";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        private static string GetSaveFolder(string saveDirectory, string savefolder)
        {
            if (savefolder == "")
            {
                return saveDirectory;
            }
            else
            {
                string path = saveDirectory + savefolder + "/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }


        //-----------file types--------------------------------------------------------------------
        public abstract class FileType
        {
            public abstract string GetFileExtension();
            public abstract void SaveObject<T>(string key, T data, string savefolder) where T : class;
            public abstract T LoadObject<T>(string key, string savefolder) where T : class;
        }

        //Json
        public class JsonFile : FileType
        {
            public override string GetFileExtension()
            {
                return ".json";
            }

            public override void SaveObject<T>(string key, T data, string savefolder)
            {
                string filePath = savefolder + key + GetFileExtension();

                var ser = new DataContractJsonSerializer((typeof(T)));

                using (var fs = File.Create(filePath))
                {
                    ser.WriteObject(fs, data);
                }
            }

            public override T LoadObject<T>(string key, string savefolder)
            {
                string filePath = savefolder + key + GetFileExtension();

                if (File.Exists(filePath))
                {
                    var ser = new DataContractJsonSerializer((typeof(T)));
                    using (var fs = File.OpenRead(filePath))
                    {
                        return ser.ReadObject(fs) as T;
                    }
                }
                return null;
            }
        }

        //Xml
        public class XMLFile : FileType
        {
            public override string GetFileExtension()
            {
                return ".xml";
            }

            public override void SaveObject<T>(string key, T data, string savefolder)
            {
                string filePath = savefolder + key + GetFileExtension();

                XDocument xml = new XDocument();
                using (var writer = xml.CreateWriter())
                {
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(writer, data);
                }
                xml.Save(filePath);
            }

            public override T LoadObject<T>(string key, string savefolder)
            {
                string filePath = savefolder + key + GetFileExtension();

                if (File.Exists(filePath))
                {
                    XDocument xml = XDocument.Load(filePath);
                    using (var reader = xml.CreateReader())
                    {
                        var deserializer = new DataContractSerializer(typeof(T));
                        return deserializer.ReadObject(reader, false) as T;
                    }
                }
                return null;

            }
        }


        //Json human readable
        public class JsonHrFile : FileType
        {
            public readonly DataContractJsonSerializerSettings Settings =
               new DataContractJsonSerializerSettings
               { UseSimpleDictionaryFormat = true };

            public override string GetFileExtension()
            {
                return ".json";
            }

            public override void SaveObject<T>(string key, T data, string savefolder)
            {
                string filePath = savefolder + key + GetFileExtension();


                using (var stream = File.Create(filePath))
                {
                    var currentCulture = Thread.CurrentThread.CurrentCulture;
                    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                    try
                    {
                        using (var writer = JsonReaderWriterFactory.CreateJsonWriter(
                            stream, Encoding.UTF8, true, true, "  "))
                        {
                            var serializer = new DataContractJsonSerializer(typeof(T), Settings);
                            serializer.WriteObject(writer, data);
                            writer.Flush();
                        }
                    }
                    catch (System.Exception exception)
                    {
                        Debug.LogError(exception.ToString());
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentCulture = currentCulture;
                    }
                }

            }

            public override T LoadObject<T>(string key, string savefolder)
            {
                string filePath = savefolder + key + GetFileExtension();

                if (File.Exists(filePath))
                {

                    using (var stream = File.OpenRead(filePath))
                    {
                        var currentCulture = Thread.CurrentThread.CurrentCulture;
                        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                        try
                        {
                            var serializer = new DataContractJsonSerializer(typeof(T), Settings);
                            var item = serializer.ReadObject(stream) as T;
                            return item;
                        }
                        catch (System.Exception exception)
                        {
                            Debug.LogError(exception.ToString());
                        }
                        finally
                        {
                            Thread.CurrentThread.CurrentCulture = currentCulture;
                        }
                    }
                }
                return null;

            }
        }
    }
}