using System.IO.Compression;
using System.Xml;
using static System.Console;
using static System.IO.Path;
using static System.Environment;

WorkWithCompression();

static void WorkWithCompression(bool useBrotli = true)
{
    var names = new string[] { "X", "Y", "Z" };

    string fileExt = useBrotli ? "brotli" : "gzip";

    //compress the XML output
    string filePath = Combine(GetFolderPath(SpecialFolder.Personal), $"streams.{fileExt}");

    FileStream file = File.Create(filePath);

    Stream compressor;
    if (useBrotli)
    {
        compressor = new BrotliStream(file, CompressionMode.Compress);
    }
    else
    {
        compressor = new GZipStream(file, CompressionMode.Compress);
    }

    using (compressor)
    {
        using (XmlWriter xml = XmlWriter.Create(compressor))
        {
            xml.WriteStartDocument();
            xml.WriteStartElement("Names");
            foreach (var name in names)
            {
                xml.WriteElementString("name", name);
            }
        }
    }

    //output all the contents of the compressed file
    WriteLine($"{filePath} contains {new FileInfo(filePath).Length} bytes.");
    WriteLine("The compressed contents:");
    WriteLine(File.ReadAllText(filePath));

    //read a compressed file
    WriteLine("Reading the compressed XML file:");

    file = File.Open(filePath, FileMode.Open);

    Stream decompress;
    if (useBrotli)
    {
        decompress = new BrotliStream(file, CompressionMode.Decompress);
    }
    else
    {
        decompress = new GZipStream(file, CompressionMode.Decompress);
    }

    using (decompress)
    {
        using (XmlReader reader = XmlReader.Create(decompress))
        {
            while (reader.Read())
            {
                if ((reader.NodeType == XmlNodeType.Element)
                    && (reader.Name == "name"))
                {
                    reader.Read();
                    WriteLine($"{reader.Value}");
                }
            }
        }
    }

}