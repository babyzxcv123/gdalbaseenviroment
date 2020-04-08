using OSGeo.GDAL;
using System;
using System.IO;

namespace gdal
{
    class Program
    {
        static void Main(string[] args)
        {//调用
            TilesBounds tilesBounds = new TilesBounds();
            tilesBounds.minCol = 109196;
            tilesBounds.maxCol = 109217;
            tilesBounds.minRow = 53340;
            tilesBounds.maxRow = 53355;
            tilesBounds.zoomLevel = 17;
            GdalConfiguration.ConfigureGdal();
            GdalConfiguration.ConfigureOgr();
            var osOperator = new OSOperator();
            osOperator.OpenFeatureClass("V_BOUA5");
            var clip = new RasterClip();
            clip.Clip();
            Console.ReadKey();
            
        }
    }

    class TilesCombine
    {
        void SaveBitmapBuffered(Dataset src, Dataset dst, int x, int y)
        {
            if (src.RasterCount < 3)
            {
                System.Environment.Exit(-1);
            }

            // Get the GDAL Band objects from the Dataset
            Band redBand = src.GetRasterBand(1);
            Band greenBand = src.GetRasterBand(2);
            Band blueBand = src.GetRasterBand(3);
            
            // Get the width and height of the raster
            int width = redBand.XSize;
            int height = redBand.YSize;

            byte[] r = new byte[width * height];
            byte[] g = new byte[width * height];
            byte[] b = new byte[width * height];

            redBand.ReadRaster(0, 0, width, height, r, width, height, 0, 0);
            greenBand.ReadRaster(0, 0, width, height, g, width, height, 0, 0);
            blueBand.ReadRaster(0, 0, width, height, b, width, height, 0, 0);

            Band wrb = dst.GetRasterBand(1);
            wrb.WriteRaster(x * width, y * height, width, height, r, width, height, 0, 0);
            Band wgb = dst.GetRasterBand(2);
            wgb.WriteRaster(x * width, y * height, width, height, g, width, height, 0, 0);
            Band wbb = dst.GetRasterBand(3);
            wbb.WriteRaster(x * width, y * height, width, height, b, width, height, 0, 0);
        }

        /// <summary>
        /// 拼接瓦片
        /// </summary>
        /// <param name="tilesBounds"></param>
        /// <param name="tilePath"></param>
        /// <param name="outPutFileName"></param>
        public void CombineTiles(TilesBounds tilesBounds, string tilePath, string outPutFileName)
        {
            if (File.Exists(outPutFileName))
            {
                File.Delete(outPutFileName);
            }
            int imageWidth = 256 * (tilesBounds.maxCol - tilesBounds.minCol + 1);
            int imageHeight = 256 * (tilesBounds.maxRow - tilesBounds.minRow + 1);
           
            //Register driver(s). 
            //Gdal.AllRegister();
            Driver driver = Gdal.GetDriverByName("GTiff");
            Dataset destDataset = driver.Create(outPutFileName, imageWidth, imageHeight, 3, DataType.GDT_Byte, null);


            //for (int col = tilesBounds.minCol; col <= tilesBounds.maxCol; col++)
            //{
            //    for (int row = tilesBounds.minRow; row <= tilesBounds.maxRow; row++)
            //    {
                    try
                    {
                        //string sourceFileName = tilePath + tilesBounds.zoomLevel.ToString() + "\\" + col.ToString() + "\\" + row.ToString() + ".png";
                        var sourceFileName = @"D:\temp\test\4\4_2007_887.jpg";
                        if (File.Exists(sourceFileName))
                        {
                            Dataset sourceDataset = Gdal.Open(sourceFileName, Access.GA_ReadOnly);
                            
                            if (sourceDataset != null)
                            {
                                SaveBitmapBuffered(sourceDataset, destDataset, 0, 0);
                            }
                        }
                        sourceFileName = @"D:\temp\test\4\4_2008_887.jpg";
                        var sourceDataset2 = Gdal.Open(sourceFileName, Access.GA_ReadOnly);
                        if (sourceDataset2 != null)
                        {
                            SaveBitmapBuffered(sourceDataset2, destDataset, 1, 0);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
            //    }
            //}
            destDataset.Dispose();

        }
    }

    class TilesBounds
    {
        public int minCol { get; set; }
        public int maxCol { get; set; }
        public int minRow { get; set; }
        public int maxRow { get; set; }
        public int zoomLevel { get; set; }
    }
}
