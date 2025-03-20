using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

namespace Burger
{
    public class SaveSystem
    {
        private const string FILE_EXTENSION = ".data";

        /// <summary>
        /// Save object with a string identifier
        /// </summary>
        /// <typeparam name="T">Type of object to save</typeparam>
        /// <param name="objectToSave">Object to save</param>
        /// <param name="key">String identifier for the data to load</param>
        public static void Save<T>(string key, T objectToSave, bool useUnityVersion = true, string path = "")
        {
            SaveToFile(key, objectToSave, useUnityVersion, path);
        }

        /// <summary>
        /// Save object with a string identifier
        /// </summary>
        /// <param name="objectToSave">Object to save</param>
        /// <param name="key">String identifier for the data to load</param>
        public static void Save(string key, Object objectToSave, bool useUnityVersion = true, string path = "")
        {
            SaveToFile(key, objectToSave, useUnityVersion, path);
        }

        /// <summary>
        /// Handle saving data to File
        /// </summary>
        /// <typeparam name="T">Type of object to save</typeparam>
        /// <param name="objectToSave">Object to serialize</param>
        /// <param name="fileName">Name of file to save to</param>
        static void SaveToFile<T>(string fileName, T objectToSave, bool useUnityVersion = true, string path = "")
        {
            // Set the path to the persistent data path (works on most devices by default)
            if (string.IsNullOrWhiteSpace(path))
            {
                path = $"{Application.persistentDataPath}/data/{Application.version}/";
                if (!useUnityVersion) path = path.Replace($"{Application.version}/", "");
            }

            SaveData(path, fileName, objectToSave);
        }

        public static void SaveData<T>(string path, string fileName, T objectToSave)
        {
            // Create the directory IF it doesn't already exist
            Directory.CreateDirectory(path);
            // Grab an instance of the BinaryFormatter that will handle serializing our data
            BinaryFormatter formatter = new BinaryFormatter();
            // Open up a filestream, combining the path and object key
            FileStream fileStream = new FileStream($"{path}{EncryptionHelper.CreateMD5(fileName)}{FILE_EXTENSION}", FileMode.Create, FileAccess.Write);

            // Try/Catch/Finally block that will attempt to serialize/write-to-stream, closing stream when complete
            try
            {
                byte[] key = System.Convert.FromBase64String(cryptoKey);
                CryptoStream cryptoStream = CreateEncryptionStream(key, fileStream);
                formatter.Serialize(cryptoStream, objectToSave);
                cryptoStream.Dispose();
            }
            catch (SerializationException exception)
            {
                Debug.LogError("Save failed. Error: " + exception.Message);
            }
            finally
            {
                fileStream.Close();
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Load data using a string identifier
        /// </summary>
        /// <typeparam name="T">Type of object to load</typeparam>
        /// <param name="key">String identifier for the data to load</param>
        /// <param name="onComplete">Callback when data loaded with the Loaded data</param>
        /// <param name="onFailed">Callback when failed to load data with the string message</param>
        /// <returns></returns>
        public static void Load<T>(string key, System.Action<T> onComplete, System.Action<string> onFailed, bool useUnityVersion = true)
        {
            // Set the path to the persistent data path (works on most devices by default)
            string path = $"{Application.persistentDataPath}/data/{Application.version}/";
            if (!useUnityVersion) path = path.Replace($"{Application.version}/", "");
            LoadData(path, key, onComplete, onFailed);
        }

        public static void LoadData<T>(string path, string key, System.Action<T> onComplete, System.Action<string> onFailed)
        {
            if (IsExists(path, key))
            {
                bool isValid = true;
                string error = "";
                // Initialize a variable with the default value of whatever type we're using
                T returnValue = default(T);

                // Grab an instance of the BinaryFormatter that will handle serializing our data
                BinaryFormatter formatter = new BinaryFormatter();

                // Open up a filestream, combining the path and object key
                FileStream fileStream = new FileStream($"{path}{EncryptionHelper.CreateMD5(key)}{FILE_EXTENSION}", FileMode.Open, FileAccess.Read);

                /* 
                * Try/Catch/Finally block that will attempt to deserialize the data
                * If we fail to successfully deserialize the data, we'll just return the default value for Type
                */
                try
                {
                    byte[] cKey = System.Convert.FromBase64String(cryptoKey);
                    CryptoStream cryptoStream = CreateDecryptionStream(cKey, fileStream);
                    returnValue = (T)formatter.Deserialize(cryptoStream);
                    cryptoStream.Dispose();
                }
                catch (SerializationException exception)
                {
                    error = exception.Message;
                    isValid = false;
                }
                finally
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }

                if (isValid)
                {
                    onComplete?.Invoke(returnValue);
                }
                else
                {
                    onFailed?.Invoke(error);
                }
            }
            else
            {
                onFailed?.Invoke("Can't read data or file not exists");
            }
        }

        public static T Load<T>(string key, bool useUnityVersion = true)
        {
            // Set the path to the persistent data path (works on most devices by default)
            string path = $"{Application.persistentDataPath}/data/{Application.version}/";
            if (!useUnityVersion) path = path.Replace($"{Application.version}/", "");
            return LoadData<T>(path, key);
        }

        public static T LoadData<T>(string path, string key)
        {
            if (IsExists(path, key))
            {
                bool isValid = true;
                string error = "";
                // Initialize a variable with the default value of whatever type we're using
                T returnValue = default(T);

                // Grab an instance of the BinaryFormatter that will handle serializing our data
                BinaryFormatter formatter = new BinaryFormatter();
                
                // Open up a filestream, combining the path and object key
                FileStream fileStream = new FileStream($"{path}{EncryptionHelper.CreateMD5(key)}{FILE_EXTENSION}", FileMode.Open, FileAccess.Read);

                /* 
                * Try/Catch/Finally block that will attempt to deserialize the data
                * If we fail to successfully deserialize the data, we'll just return the default value for Type
                */
                try
                {
                    byte[] cKey = System.Convert.FromBase64String(cryptoKey);
                    CryptoStream cryptoStream = CreateDecryptionStream(cKey, fileStream);
                    returnValue = (T)formatter.Deserialize(cryptoStream);
                    cryptoStream.Dispose();
                }
                catch (SerializationException exception)
                {
                    error = exception.Message;
                    isValid = false;
                }
                finally
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }

                if (isValid)
                {
                    //onComplete?.Invoke(returnValue);
                    return returnValue;
                }
                else
                {
                    //onFailed?.Invoke(error);
                    return default;
                }
            }
            else
            {
                //onFailed?.Invoke("Can't read data or file not exists");
                return default;
            }
        }

        /// <summary>
        /// Check if the data exists
        /// </summary>
        /// <param name="key">String identifier for the data to check</param>
        /// <returns></returns>
        public static bool IsExists(string key)
        {
            string path = $"{Application.persistentDataPath}/data/{Application.version}/";

            if (IsExists(path, key))
            {
                return true;
            }
            else
            {
                path = path.Replace($"{Application.version}/", "");
                return IsExists(path, key);
            }
        }

        /// <summary>
        /// Check if the data exists
        /// </summary>
        /// <param name="filename">String identifier for the data to check</param>
        /// <returns></returns>
        public static bool IsExists(string path, string filename)
        {
            return File.Exists($"{path}{EncryptionHelper.CreateMD5(filename)}{FILE_EXTENSION}");
        }

        /// <summary>
        /// Delete data using a string identifier
        /// </summary>
        /// <param name="filename">String identifier for the data to delete</param>
        /// <returns></returns>
        public static void Delete(string filename, bool useUnityVersion = true)
        {
            string path = $"{Application.persistentDataPath}/data/{Application.version}/";
            if (!useUnityVersion) path = path.Replace($"{Application.version}/", "");

            if (IsExists(path,filename))
            {
                Debug.Log($"Deleting----- {filename} : {path}{EncryptionHelper.CreateMD5(filename)}{FILE_EXTENSION}");
                File.Delete($"{path}{EncryptionHelper.CreateMD5(filename)}{FILE_EXTENSION}");
            }
            else
            {
                path = path.Replace($"{Application.version}/", "");
                if (IsExists(path, filename))
                {
                    Debug.Log($"Deleting----- {filename} : {path}{EncryptionHelper.CreateMD5(filename)}{FILE_EXTENSION}");
                    File.Delete($"{path}{EncryptionHelper.CreateMD5(filename)}{FILE_EXTENSION}");
                }
            }
        }

        /// <summary>
        /// Delete data using a string identifier
        /// </summary>
        /// <param name="filename">String identifier for the data to delete</param>
        /// <returns></returns>
        public static void DeleteAll()
        {
            string dataPath = $"{Application.persistentDataPath}/data/";
            if (Directory.Exists(dataPath)) { Directory.Delete(dataPath, true); }

            string avatarPath = $"{Application.persistentDataPath}/Resource/";
            if (Directory.Exists(avatarPath)) { Directory.Delete(avatarPath, true); }
        }

        private const string cryptoKey = "Q3JpcHRvZ3JhZmlhcyBjb20gUmluamRhZWwgLyBBRVM=";
        private const int keySize = 256;
        private const int ivSize = 16; // block size is 128-bit

        public static CryptoStream CreateEncryptionStream(byte[] key, Stream outputStream)
        {
            byte[] iv = new byte[ivSize];

            using (var rng = new RNGCryptoServiceProvider())
            {
                // Using a cryptographic random number generator
                rng.GetNonZeroBytes(iv);
            }

            // Write IV to the start of the stream
            outputStream.Write(iv, 0, iv.Length);

            Rijndael rijndael = new RijndaelManaged();
            rijndael.KeySize = keySize;

            CryptoStream encryptor = new CryptoStream(
                outputStream,
                rijndael.CreateEncryptor(key, iv),
                CryptoStreamMode.Write);
            return encryptor;
        }

        public static CryptoStream CreateDecryptionStream(byte[] key, Stream inputStream)
        {
            byte[] iv = new byte[ivSize];

            if (inputStream.Read(iv, 0, iv.Length) != iv.Length)
            {
                throw new System.Exception("Failed to read IV from stream.");
            }

            Rijndael rijndael = new RijndaelManaged();
            rijndael.KeySize = keySize;

            CryptoStream decryptor = new CryptoStream(
                inputStream,
                rijndael.CreateDecryptor(key, iv),
                CryptoStreamMode.Read);
            return decryptor;
        }
    }
}