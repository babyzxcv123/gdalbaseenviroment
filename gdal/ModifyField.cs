using OSGeo.GDAL;
using System.Collections.Generic;
using System.IO;

namespace gdal
{
    class ModifyField
    {
        private List<string> trdPaths = new List<string>();
        private List<string> sixPaths = new List<string>();
        private List<string> otherPaths = new List<string>();
        public void Read(string srcPath, string trdresultpath, string sixresultpath)
        {
            trdPaths = new List<string>();
            sixPaths = new List<string>();
            otherPaths = new List<string>();
            var dirs = Directory.GetDirectories(srcPath);
            if (File.Exists(trdresultpath))
                File.Delete(trdresultpath);
            if (File.Exists(sixresultpath))
            {
                File.Delete(sixresultpath);
            }

            foreach (var item in dirs)
            {
                ReadDir(item);
                ReadFiles(item);
            }
            ReadFiles(srcPath);
            using (StreamWriter sw = new StreamWriter(trdresultpath))
            {
                trdPaths.ForEach(temp => sw.WriteLine(temp));
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            using (StreamWriter sw = new StreamWriter(sixresultpath))
            {
                sixPaths.ForEach(temp => sw.WriteLine(temp));
                sw.Flush();
                sw.Close();
                sw.Dispose();
            }
            if (otherPaths.Count > 0)
            {
                var otherpath = Path.Combine(Path.GetDirectoryName(sixresultpath), "其他投影信息影像列表.txt");
                if (File.Exists(otherpath))
                    File.Delete(otherpath);

                using (StreamWriter sw = new StreamWriter(otherpath))
                {
                    otherPaths.ForEach(temp => sw.WriteLine(temp));
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
        }

        private void ReadDir(string item)
        {
            var dirs = Directory.GetDirectories(item);

            foreach (var subdir in dirs)
            {
                ReadDir(subdir);
                ReadFiles(subdir);
            }
        }

        private void ReadFiles(string item)
        {
            var files = Directory.GetFiles(item, "*.tif");
            Dataset srcDs = null;
            foreach (string fi in files)
            {
                item = item.Replace("\\", "/");
                srcDs = Gdal.Open(fi, Access.GA_ReadOnly);
                if (srcDs != null)
                {
                    var proj = srcDs.GetProjectionRef();
                    srcDs.Dispose();
                    if (proj.ToUpper().Contains("CGCS2000_3") ||
                        proj.ToUpper().Contains("CGCS_2000_3"))
                    {
                        trdPaths.Add(fi);
                    }
                    else
                    {
                        sixPaths.Add(fi);
                    }
                }
                else
                {
                    otherPaths.Add("文件【" + fi + "】读取投影错误");
                }
            }
        }
    }
}
