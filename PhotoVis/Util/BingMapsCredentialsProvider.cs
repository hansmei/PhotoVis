using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BingMapsCredentials;

namespace PhotoVis.Util
{
    public class BingMapsCredentialsProvider
    {
        private string _key;
        public string Key
        {
            get
            {
                return _key;
            }
        }

        public BingMapsCredentialsProvider()
        {
            string fullPath = GetBingMapsCredentialsFullPath();
            if (!File.Exists(fullPath))
            {
                CreateKeyWindow bingMapsKey = new CreateKeyWindow(fullPath);
                bool? result = bingMapsKey.ShowDialog();

                if(bingMapsKey.Key != "")
                {
                    this._key = bingMapsKey.Key;
                }
            }
            else
            {
                this._key = File.ReadAllText(fullPath);
            }
        }

        public static string GetBingMapsCredentialsFileName()
        {
            return "BingMapsKey.txt";
        }

        public static string GetBingMapsCredentialsFullPath()
        {
            string fullPath = Path.Combine(App.PhotoVisDataRoot, GetBingMapsCredentialsFileName());
            return fullPath;
        }
    }
}
