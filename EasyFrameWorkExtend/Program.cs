using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy.Storage.QCloud;
using Easy.Storage.QCloud.Api;
using Easy.Extend;
using Newtonsoft.Json.Linq;

namespace EasyFrameWorkExtend
{
    class Program
    {
        private static CosCloud _client;
        private static string bucketName;
        private static DirectoryInfo baseDirectoryInfo;
        static void Main(string[] args)
        {
            var config = Configuration.GetConfiguration();
            bucketName = config.BucketName;
            _client = new CosCloud(config.AppID, config.SecretId, config.SecretKey);
            baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "Upload\\");
            UploadFiles(baseDirectoryInfo);
            Console.ReadKey();
        }
        static string RelativePath(string path)
        {
            if (path.IsNotNullAndWhiteSpace())
            {
                path = path.Replace(baseDirectoryInfo.FullName, "/").Replace("\\", "/");
            }
            return path;
        }
        static void UploadFiles(DirectoryInfo dir)
        {
            foreach (var childDir in dir.GetDirectories())
            {
                UploadFiles(childDir);
            }
            foreach (var file in dir.GetFiles())
            {
                Console.WriteLine(file.FullName);
                string result = _client.UploadFile(bucketName, RelativePath(file.FullName), file.FullName);
                JToken token = JObject.Parse(result);
                var data = token.SelectToken("data");
                Console.WriteLine(data.SelectToken("access_url"));
                Console.WriteLine(result);
                Console.ReadKey();
            }
        }
    }
}
