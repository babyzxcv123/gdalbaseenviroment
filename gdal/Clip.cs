using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace gdal
{
    class RasterClip
    {
        public void Clip()
        {
            //规则裁剪  
            string srcFileName = @"D:\temp\1.tif";
            string dstFileName = @"D:\temp\测试17-1.tif";

            if (File.Exists(dstFileName))
                File.Delete(dstFileName);
            if (!File.Exists(srcFileName))
            {
                Console.WriteLine("File " + srcFileName + " not found");
                Console.ReadKey();

            }
            if (!File.Exists(srcFileName))
                return;

            Dataset srcDs = Gdal.Open(srcFileName, Access.GA_ReadOnly);
            DataType srcType = srcDs.GetRasterBand(1).DataType;

            int bandCount = srcDs.RasterCount;

            Console.WriteLine("原始影像数据类型是：{0}", srcType);
            Console.WriteLine("原始影像的列数：{0}", srcDs.RasterXSize);
            Console.WriteLine("原始影像的行数：{0}", srcDs.RasterYSize);


            Console.WriteLine("请输入裁剪从第几列开始：");
            int startXSize = Convert.ToInt32(0);
            Console.WriteLine("请输入裁剪从第几行开始：");
            int startYSize = Convert.ToInt32(0);

            Console.WriteLine("请输入裁剪图像的宽度：");
            int dstXSize = Convert.ToInt32(256);
            Console.WriteLine("请输入裁剪图像的高度：");
            int dstYSize = Convert.ToInt32(256);


            double[] adfGeoTransform = new double[6];
            srcDs.GetGeoTransform(adfGeoTransform);

            //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)  
            //左上角点坐标(padfGeoTransform[0],padfGeoTransform[3])；  
            //padfGeoTransform[1]是像元宽度(影像在宽度上的分辨率)；  
            //p/adfGeoTransform[5]是像元高度(影像在高度上的分辨率)；  
            //如果影像是指北的,padfGeoTransform[2]和padfGeoTransform[4]这两个参数的值为0。  

            Driver drv = Gdal.GetDriverByName("GTIFF");
            Dataset dstDs = drv.Create(dstFileName, dstXSize, dstYSize, bandCount, srcType, null);

            dstDs.SetGeoTransform(adfGeoTransform);
            dstDs.SetProjection(srcDs.GetProjectionRef());

            int[] bandArray = new int[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                bandArray[i] = i + 1;
            }

            //if (srcType == DataType.GDT_UInt16)
            {
                int[] dataArray = new int[dstXSize * dstYSize * bandCount];

                srcDs.ReadRaster(startXSize, startYSize, dstXSize, dstYSize, dataArray, dstXSize, dstYSize, bandCount, bandArray, 0, 0, 0);
                dstDs.WriteRaster(0, 0, dstXSize, dstYSize, dataArray, dstXSize, dstYSize, bandCount, bandArray, 0, 0, 0);

                dstDs.FlushCache();
            }

            Console.WriteLine("success");
            Console.WriteLine();
            Console.WriteLine("裁剪影像的宽为：{0}，高为:{1}。", dstDs.RasterXSize, dstDs.RasterYSize);

            //最后释放资源  
            srcDs.Dispose();
            dstDs.Dispose();
            Console.ReadKey();
        }
    }
}
