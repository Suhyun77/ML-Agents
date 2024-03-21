namespace Anipen.Utils
{
    using System;
    using System.IO;
    using UnityEngine;

    #region Json parser
    public class JsonFileStreamer<T>
    {
        public static T ReadFile(string path)
        {
            if (File.Exists(path))
                return JsonUtility.FromJson<T>(File.ReadAllText(path));

            return default(T);
        }

        public static void WriteFile(string path, T data)
        {
            try
            {
                File.WriteAllText(path, JsonUtility.ToJson(data, true));
            }
            catch (DirectoryNotFoundException ex)
            {
                Debug.LogError("DIrectory not found : " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.LogError("Write file error with exception: " + ex.Message);
            }
        }

        public void CopyWrite(string originPath, string copyPath)
        {

            if (File.Exists(copyPath))
                File.Delete(copyPath);

            File.Copy(originPath, copyPath);
        }

        public static void ClearFile(string path)
        {
            try
            {
                File.WriteAllText(path, string.Empty);
            }
            catch (DirectoryNotFoundException ex)
            {
                Debug.LogError("DIrectory not found : " + ex.StackTrace);
            }
            catch (Exception ex)
            {
                Debug.LogError("Write file error with exception: " + ex.Message);
            }
        }
    }

    public class JsonFileManagingHelper<T> where T : new()
    {
        #region Methods
        protected string savedFilePath;
        protected T data;
        #endregion

        #region Constructor
        public JsonFileManagingHelper(string path)
        {
            savedFilePath = path;
        }

        ~JsonFileManagingHelper() { Reset(); }
        #endregion

        public virtual void WriteFileData(T data)
        {
            try
            {
                this.data = data;
                JsonFileStreamer<T>.WriteFile(savedFilePath, data);
            }
            catch (Exception ex)
            {
                Debug.LogError("Write file error with exception: " + ex.Message);
            }
        }

        public virtual void ClearFileData()
        {
            try
            {
                JsonFileStreamer<T>.ClearFile(savedFilePath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Clear file error with exception: " + ex.Message);
            }
        }

        public virtual T ReadFileData()
        {
            if (data == null)
                data = JsonFileStreamer<T>.ReadFile(savedFilePath);

            return data;
        }

        public virtual void DeleteFile()
        {
            File.Delete(savedFilePath);
        }

        public virtual void Reset()
        {
            data = default;
            savedFilePath = null;
        }
    }

    // Parsing root json
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string wrapperJson = string.Format("{\"Items\":{0}\"}", json);
            Debug.Log("Wrapper json : " + wrapperJson);
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapperJson);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
    #endregion

    #region String Utils
    public static class StringUtil
    {
        public static string GetVectorString(Vector3 target)
        {
            return string.Format("{0},{1},{2}", target.x, target.y, target.z);
        }

        public static string GetVectorString(Vector2 target)
        {
            return string.Format("{0},{1}", target.x, target.y);
        }

        public static Vector3 GetVector3(string target)
        {
            string[] elements = target.Split(',');

            if (elements == null || elements.Length < 3)
                return default;

            float x, y, z;
            float.TryParse(elements[0], out x);
            float.TryParse(elements[1], out y);
            float.TryParse(elements[2], out z);

            return new Vector3(x, y, z);
        }

        public static Vector2 GetVector2(string target)
        {
            string[] elements = target.Split(',');

            if (elements == null || elements.Length < 2)
                return default;

            float x, y;
            float.TryParse(elements[0], out x);
            float.TryParse(elements[1], out y);

            return new Vector2(x, y);
        }

        public static string GetQuaternionString(Quaternion target)
        {
            return string.Format("{0},{1},{2},{3}", target.x, target.y, target.z, target.w);
        }

        public static Quaternion GetQuaternion(string target)
        {
            string[] elements = target.Split(',');

            if (elements == null || elements.Length < 4)
                return default;

            float x, y, z, w;
            float.TryParse(elements[0], out x);
            float.TryParse(elements[1], out y);
            float.TryParse(elements[2], out z);
            float.TryParse(elements[3], out w);

            return new Quaternion(x, y, z, w);
        }
    }

    public static class TextureUtil
    {
        public static Texture2D Rotate90(Texture2D orig)
        {
            Debug.Log("doing rotate90");
            Color32[] origpix = orig.GetPixels32(0);
            Color32[] newpix = new Color32[orig.width * orig.height];
            for (int c = 0; c < orig.height; c++)
            {
                for (int r = 0; r < orig.width; r++)
                {
                    newpix[orig.width * orig.height - (orig.height * r + orig.height) + c] =
                      origpix[orig.width * orig.height - (orig.width * c + orig.width) + r];
                }
            }
            Texture2D newtex = new Texture2D(orig.height, orig.width, orig.format, false);
            newtex.SetPixels32(newpix, 0);
            newtex.Apply();
            return newtex;
        }

        public static Texture2D Rotate180(Texture2D orig)
        {
            Debug.Log("doing rotate180");
            Color32[] origpix = orig.GetPixels32(0);
            Color32[] newpix = new Color32[orig.width * orig.height];
            for (int i = 0; i < origpix.Length; i++)
            {
                newpix[origpix.Length - i - 1] = origpix[i];
            }
            Texture2D newtex = new Texture2D(orig.width, orig.height, orig.format, false);
            newtex.SetPixels32(newpix, 0);
            newtex.Apply();
            return newtex;
        }

        public static Texture2D Rotate270(Texture2D orig)
        {
            Debug.Log("doing rotate270");
            Color32[] origpix = orig.GetPixels32(0);
            Color32[] newpix = new Color32[orig.width * orig.height];
            int i = 0;
            for (int c = 0; c < orig.height; c++)
            {
                for (int r = 0; r < orig.width; r++)
                {
                    newpix[orig.width * orig.height - (orig.height * r + orig.height) + c] = origpix[i];
                    i++;
                }
            }
            Texture2D newtex = new Texture2D(orig.height, orig.width, orig.format, false);

            newtex.SetPixels32(newpix, 0);
            newtex.Apply();
            return newtex;
        }
    }
    #endregion
}
