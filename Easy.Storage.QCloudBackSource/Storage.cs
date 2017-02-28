using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easy.Extend;
using Newtonsoft.Json.Linq;

namespace Easy.Storage.QCloudBackSource
{
    public class Storage : IStorageService
    {
        private readonly string _host;
        public Storage()
        {
            _host = System.Configuration.ConfigurationManager.AppSettings["BackSourceUrl"];
        }
        string RelativePath(string path)
        {
            if (path.IsNotNullAndWhiteSpace())
            {
                path = path.Replace(AppDomain.CurrentDomain.BaseDirectory, "").Replace("\\", "/");
            }
            return _host + path;
        }
        public string CreateFolder(string folder)
        {
            return folder;
        }

        public string DeleteFile(string file)
        {
            return file;
        }

        public string DeleteFolder(string folder)
        {
            return folder;
        }

        public string SaveFile(string file)
        {
            return RelativePath(file);
        }
    }
}
