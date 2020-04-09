﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using Gdal = OSGeo.GDAL.Gdal;
using Ogr = OSGeo.OGR.Ogr;
using System.IO;
using System.Diagnostics;

namespace gdal
{
    /******************************************************************************
 *
 * Name:     GdalConfiguration.cs.pp
 * Project:  GDAL CSharp Interface
 * Purpose:  A static configuration utility class to enable GDAL/OGR.
 * Author:   Felix Obermaier
 *
 *****************************************************************************/

    public static partial class GdalConfiguration
    {
        private static bool _configuredOgr;
        private static bool _configuredGdal;

        /// <summary>
        /// Construction of Gdal/Ogr
        /// </summary>
        static GdalConfiguration()
        {
            var executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            var executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

            if (string.IsNullOrEmpty(executingDirectory))
                throw new InvalidOperationException("cannot get executing directory");


            var gdalPath = Path.Combine(executingDirectory, "gdal");
            var nativePath = gdalPath;// Path.Combine(gdalPath, GetPlatform());

            // Prepend native path to environment path, to ensure the
            // right libs are being used.
            var path = Environment.GetEnvironmentVariable("PATH");
            path = nativePath + ";" + Path.Combine(nativePath, "plugins") + ";" + path;
            Environment.SetEnvironmentVariable("PATH", path);

            // Set the additional GDAL environment variables.
            var gdalData = Path.Combine(gdalPath, "data");
            Environment.SetEnvironmentVariable("GDAL_DATA", gdalData);
            Gdal.SetConfigOption("GDAL_DATA", gdalData);

            var driverPath = Path.Combine(nativePath, "plugins");
            Environment.SetEnvironmentVariable("GDAL_DRIVER_PATH", driverPath);
            Gdal.SetConfigOption("GDAL_DRIVER_PATH", driverPath);

            Environment.SetEnvironmentVariable("GEOTIFF_CSV", gdalData);
            Gdal.SetConfigOption("GEOTIFF_CSV", gdalData);

            var projSharePath = Path.Combine(gdalPath, "share");
            Environment.SetEnvironmentVariable("PROJ_LIB", projSharePath);
            Gdal.SetConfigOption("PROJ_LIB", projSharePath);
        }

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
        public static void ConfigureOgr()
        {
            if (_configuredOgr) return;

            // Register drivers
            Ogr.RegisterAll();
            // 设置Shp字段、属性等的编码为空
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
#if DEBUG
            for(int i=0;i<Ogr.GetDriverCount();i++)
            {
                Trace.WriteLine(Ogr.GetDriver(i).name);
            }
#endif
            // 解决Shp文件中文路径乱码，无法读取的问题（有时去掉却可以识别中文，待解决）
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            _configuredOgr = true;
        }

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
        public static void ConfigureGdal()
        {
            if (_configuredGdal) return;

            // Register drivers
            Gdal.AllRegister();
            // 设置Shp字段、属性等的编码为空
            Gdal.SetConfigOption("SHAPE_ENCODING", "UTF-8");

            // 解决Shp文件中文路径乱码，无法读取的问题（有时去掉却可以识别中文，待解决）
           /// Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            _configuredGdal = true;
        }
    }
}
