using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public class Save 
{
    static public void SaveFileJSON(object obj, string path)
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(obj));
    }

    static public void SaveFileBinary(object obj, string path)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            formatter.Serialize(fs, obj);
        }
    }
}
