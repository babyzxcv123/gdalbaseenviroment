using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gdal
{
    /// <summary>
    /// Oracle Spatial 操作函数
    /// </summary>
    class OSOperator
    {
        public void OpenFeatureClass(string layername)
        {
            Feature pFeature;
            Geometry pGeometry = null;
            FeatureDefn pFeatureDefn = null;
            FieldDefn pFieldDefn = null;
            var path = @"d:\temp\gs\result\201812\620102\DLG620102.gdb";
            var driver = Ogr.GetDriverByName("FileGDB");

            if (driver != null)
            {
                var dsz = driver.Open(path, 0);
                Console.WriteLine(Encoding.UTF8.GetString(Encoding.Default.GetBytes(dsz.name)));
            }
            //const char* uri = "/home/hawk/data/gis/beijing/beijing_Pt.shp";
            //const char* uri  = "PG:dbname='GBSDE' host='127.0.0.1' port='5432' user='GBSDE' password='GBSDE'";
            #region oracle
            //string path = "OCI:tlmh/tlmh@10.1.4.92:1521/orcl";            
            //var driver=Ogr.GetDriverByName("OCI");



            //if (pDataSource == null)
            //{
            //    Console.WriteLine("open failed.\n");
            //    Console.ReadKey();
            //    Environment.Exit(-1);
            //}
           
            #endregion
            DataSource pDataSource = driver.Open(path, 0);
            for (int i = 0; i < pDataSource.GetLayerCount(); i++)
            {
                Console.WriteLine(pDataSource.GetLayerByIndex(i).GetName());
            }
            Layer pLayer = pDataSource.GetLayerByName(layername);
            if(pLayer==null)
            {
                pDataSource.Dispose();
                return;
            }
            pLayer.ResetReading();
            var wktgeo = @"POLYGON ((116.330651049 40.084599132, 116.330453588 40.084395933, 116.330477399 40.084273941, 116.330523052 40.084039644, 116.330518847 40.084005658, 116.330499899 40.08385487, 116.330556458 40.083565115, 116.330575875 40.083465657, 116.330741186 40.082618069, 116.330796316 40.082516124, 116.330866728 40.082303332, 116.330920917 40.082032656, 116.33120179 40.081919559, 116.332974342 40.082072609, 116.33297822 40.08204646, 116.333107863 40.082057779, 116.333104057 40.082083812, 116.335961669 40.082330485, 116.336178351 40.082523437, 116.336144053 40.082837756, 116.336154487 40.082938106, 116.336183181 40.083070586, 116.336058538 40.084259822, 116.335981357 40.084435789, 116.335961686 40.084623793, 116.335941836 40.084813309, 116.335688497 40.084985925, 116.335219453 40.084971788, 116.334980226 40.085009628, 116.334236431 40.084987183, 116.334183662 40.084985466, 116.334039009 40.084979817, 116.333918803 40.084974047, 116.333801806 40.084967511, 116.333684946 40.084960058, 116.333568189 40.084951704, 116.333451559 40.084942433, 116.333335054 40.084932253, 116.333218676 40.084921163, 116.333100456 40.084908955, 116.332346651 40.084827614, 116.332232922 40.084815341, 116.331862257 40.084775334, 116.331339038 40.084718866, 116.331115505 40.084649266, 116.330651049 40.084599132))
";
            Geometry pAnaGeom =  Geometry.CreateFromWkt(wktgeo);
            pAnaGeom.AssignSpatialReference(pLayer.GetSpatialRef());
            //CoordinateTransformation c = new CoordinateTransformation(pAnaGeom.GetSpatialReference(), pLayer.GetSpatialRef());
            //c.(pAnaGeom.GetPointCount(),pAnaGeom
            while ((pFeature = pLayer.GetNextFeature()) != null)
            {
                //pFeatureDefn = pLayer.GetLayerDefn();
                //for (int i = 0; i < pFeatureDefn.GetFieldCount(); i++)
                //{
                //    pFieldDefn = pFeatureDefn.GetFieldDefn(i);
                //}
                //pGeometry = pFeature.GetGeometryRef();
                //if (pAnaGeom == null)
                //    pAnaGeom = pGeometry.Clone();
                //var oid=pFeature.GetFieldAsString("bsm");
                //if (oid == "126594")
                //{
                //}
                //if (pGeometry != null)
                //{
                //    if (pGeometry.Touches(pAnaGeom))
                //    {
                //    }

                //    var intersectGeo=pGeometry.Intersection(pAnaGeom);
                   
                //    if (intersectGeo.IsEmpty() || !intersectGeo.IsValid())
                //    {
                //    }
                //    else
                //    {
                //        Console.WriteLine(pFeature.GetFieldAsString("bsm") + "______" + intersectGeo.GetArea());
                //    }

                //    //string argout = string.Empty;
                //    //pGeometry.ExportToWkt(out argout);
                //    //Console.WriteLine(argout);
                //}

                pFeature.Dispose();
            }

            pDataSource.Dispose();
        }
    }
}
