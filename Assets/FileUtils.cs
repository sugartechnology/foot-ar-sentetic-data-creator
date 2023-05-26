using UnityEngine;
using UnityEngine.Networking;
using System.IO.Compression;
using System.IO;
class FileUtils
{
    public static string SaveFileAsText(string folderPath, string fileName, string content, string ext = "json")
    {
        /*string actualFolderPath = folderPath;
        if (folderPath.StartsWith("/"))
            actualFolderPath = System.IO.Path.Combine(Application.persistentDataPath, folderPath);

        if (!System.IO.Directory.Exists(actualFolderPath))
            System.IO.Directory.CreateDirectory(actualFolderPath);*/

        string actualFilePath = System.IO.Path.Combine(Application.persistentDataPath, fileName + "." + ext);
        System.IO.File.WriteAllText(actualFilePath, content);
        Debug.Log("actual file path is," + actualFilePath);
        return actualFilePath;
    }


    public static string SaveFileAsBinary(string folderPath, string fileName, byte[] readArray, string ext = "jpg")
    {
        //Fix Folder Path
        /*string actualFolderPath = folderPath;
        if (folderPath.StartsWith("/"))
            actualFolderPath = System.IO.Path.Combine(Application.persistentDataPath, folderPath);

        if (!System.IO.Directory.Exists(actualFolderPath))
            System.IO.Directory.CreateDirectory(actualFolderPath);*/

        string actualFilePath = System.IO.Path.Combine(Application.persistentDataPath, fileName + "." + ext);
        System.IO.File.WriteAllBytes(actualFilePath, readArray);
        return actualFilePath;
    }


    public static Texture2D LoadFileAsBinary(string folderPath, string fileName)
    {
        string path = System.IO.Path.Combine(folderPath, fileName);
        byte[] readedData = System.IO.File.ReadAllBytes(path);
        Texture2D texture2D = new Texture2D(1, 1);
        texture2D.LoadImage(readedData);
        return texture2D;
    }

    public static ZipArchive LoadZipArchive(string folderPath, string fileName)
    {
        string path = System.IO.Path.Combine(folderPath, fileName);
        FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read);
        ZipArchive zipStream = new ZipArchive(fileStream);
        return zipStream;
    }


    public static Texture2D LoadStremAsTexture2d(Stream mStream)
    {

        MemoryStream ms = new MemoryStream();
        mStream.CopyTo(ms);
        byte[] data = ms.ToArray();
        Texture2D texture2D = new Texture2D(1, 1);
        texture2D.LoadImage(data);
        return texture2D;

    }
}