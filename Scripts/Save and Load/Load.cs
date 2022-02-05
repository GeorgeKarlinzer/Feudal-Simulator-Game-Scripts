using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

public class Load
{
    static public T LoadFileJSON<T>(string path)
    {
        if (File.Exists(path))
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
        return default;
    }

    static public T LoadFileBinary<T>(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                return (T)formatter.Deserialize(fs);
            }
        }
        else
            return default;
    }

}
