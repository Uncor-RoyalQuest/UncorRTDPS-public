using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace UncorRTDPS.Services.MobsIcons
{
    public class ImgMobNameDictionaryLoader
    {
        private string keyValueSeparator = "=";

        public Dictionary<string, BitmapImage> LoadDictionary(string linkerFileFullPath, string imgFolderFullPath)
        {
            //format: <"001",Img>
            Dictionary<string, BitmapImage> imgs = new Dictionary<string, BitmapImage>();
            //format: <"001", "Archon">
            List<(string, string)> links = new List<(string, string)>();
            //format: <"Archon", Img>
            Dictionary<string, BitmapImage> res = new Dictionary<string, BitmapImage>();

            if (!Directory.Exists(imgFolderFullPath) || !File.Exists(linkerFileFullPath))
                return res;

            string[] imgFileNames = null;
            try
            {
                imgFileNames = Directory.GetFiles(imgFolderFullPath, "*.png", SearchOption.TopDirectoryOnly);
            }
            catch { imgFileNames = null; }
            if (imgFileNames == null)
                return res;

            foreach (string imgFullName in imgFileNames)
            {
                FileInfo fi = new FileInfo(imgFullName);
                if (fi.Name.Length < 3)
                    continue;
                string imgUniqueNumber = fi.Name.Substring(0, 3);
                imgs[imgUniqueNumber] = new BitmapImage(new Uri(imgFullName));
            }

            try
            {
                using StreamReader file = new StreamReader(linkerFileFullPath);
                string line;
                string[] d;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Trim().Length < 1)
                        continue;
                    d = line.Split(keyValueSeparator);
                    if (d.Length == 2 && d[0] != null && d[1] != null)
                    {
                        string key = d[0].Trim();
                        string val = d[1].Trim();
                        if (key.Length > 0 && val.Length > 0)
                        {
                            links.Add((key, val));
                        }
                    }
                }
            }
            catch { return null; }

            foreach (var kv in links)
            {
                if (imgs.ContainsKey(kv.Item1))
                {
                    res[kv.Item2] = imgs[kv.Item1];
                }
            }

            return res;
        }
    }
}
