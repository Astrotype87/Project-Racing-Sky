using System;
using System.IO;

public class JsonDataHandler : IDataHandler
{
    private string dataDirectoryPath;
    private string dataFileName;
    private bool useEncryption;
    
    private readonly string encryptionKey = "Astrotype";
    
    public JsonDataHandler(string dataDirectoryPath, string dataFileName, bool useEncryption)
    {
        this.dataDirectoryPath = dataDirectoryPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }
    
    public bool LoadData<T>(ref T data)
    {
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
        
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    using (StreamReader reader = new StreamReader(stream))
                        dataToLoad = reader.ReadToEnd();
                
                if (useEncryption)
                    dataToLoad = ApplySymmetricEncryption(dataToLoad);
                
                data = UnityEngine.JsonUtility.FromJson<T>(dataToLoad);
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error occured while loading data from file: " + fullPath + "\n" + e);
                return false;
            }
        }
        
        return false;
    }
    
    public bool SaveData<T>(T data)
    {
        string fullPath = Path.Combine(dataDirectoryPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = UnityEngine.JsonUtility.ToJson(data, !useEncryption);
            
            if (useEncryption)
                dataToStore = ApplySymmetricEncryption(dataToStore);
            
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(stream))
                    writer.Write(dataToStore);
            
            return true;
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error occurred while saving data to file: " + fullPath + "\n" + e);
            return false;
        }
    }
    
    private string ApplySymmetricEncryption(string data)
    {
        // This uses XOR encryption
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char) (data[i] ^ encryptionKey[i % encryptionKey.Length]);
        }
        return modifiedData;
    }
    
}
