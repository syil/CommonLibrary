using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Extensions
{
    public static class DataExtensions
    {
        /// <summary>
        /// DbDataReader nesnesinin o anki pozisyonunda ki belirtilen kolonun değerini döner
        /// </summary>
        /// <typeparam name="T">Değerin tipi. Veritabanından gelen tip ile uyuşmalıdır, aksi halde null döner</typeparam>
        /// <param name="reader">Değerin okunacağı DbDataReader nesnesi</param>
        /// <param name="columnName">DbDataReader nesnesinin o anki pozisyonunda tanımlı kolon adı</param>
        /// <returns>Belirtilen kolonun değeri</returns>
        public static T GetValueOrDefault<T>(this IDataReader reader, string columnName)
        {
            try
            {
                T value = (T)reader[columnName];
                return value;
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// İki DataTable nesnesini birleştirir. 
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public static DataTable UnionWith(this DataTable dt1, DataTable dt2)
        {
            DataTable unionDt = dt1.Copy();

            foreach (DataRow dr in dt2.Rows)
            {
                unionDt.ImportRow(dr);
            }

            return unionDt;
        }

        #region DataTable & DataRow Extensions
        /// <summary>
        /// DataRow nesnesinin içerisindeki değerleri, yeni bir T nesnesine doldurur. T nesnesinin özelliklerini DataRow kolonları ile eşleştirir.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow dr)
            where T : class, new()
        {
            T obj = new T();
            dr.FillEntity<T>(obj);

            return obj;
        }

        /// <summary>
        /// DataRow nesnesinin içerisindeki değerleri parametre olarak gönderilen T nesnesine doldurur. T nesnesinin özelliklerini DataRow kolonları ile eşleştirir.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="obj"></param>
        public static void FillEntity<T>(this DataRow dr, T obj)
            where T : class
        {
            PropertyInfo[] propertyList = typeof(T).GetProperties();
            DataColumnCollection columns = dr.Table.Columns;

            foreach (PropertyInfo prop in propertyList)
            {
                string columnName = prop.GetAssociatedColumnName();
                if (columns[columnName] == null)
                    continue;

                object value = dr[columnName];
                if (!value.Equals(DBNull.Value))
                {
                    prop.SetPropertyValue(obj, value);
                }
            }
        }

        /// <summary>
        /// DataTable içerisindeki tüm satırları yeni bir listeye doldurur
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToIEnumerable<T>(this DataTable dt)
            where T : class, new()
        {
            foreach (DataRow rows in dt.Rows)
            {
                yield return rows.ToEntity<T>();
            }
        }

        /// <summary>
        /// Varolan bir liste içerisindeki T nesnelerini DataTable içerisindeki satırlar ile eşleştirerek günceller. 
        /// Güncelleme için T nesnesinde <c ref="KeyAttribute">KeyAttribute</c> ile belirtilmiş bir özellik bulunmalıdır.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="objList"></param>
        public static void FillList<T>(this DataTable dt, List<T> objList)
            where T : class
        {
            PropertyInfo[] propertyList = typeof(T).GetProperties();
            PropertyInfo keyProperty = null;

            foreach (PropertyInfo prop in propertyList)
            {
                if (prop.HasAttribute<KeyAttribute>())
                {
                    keyProperty = prop;
                    break;
                }
            }

            if ((keyProperty != null))
            {
                foreach (DataRow rows in dt.Rows)
                {
                    object key = Convert.ChangeType(rows[keyProperty.GetAssociatedColumnName()], keyProperty.PropertyType);
                    T listItem = objList.Where(o => keyProperty.GetValue(o, null).Equals(key)).SingleOrDefault();

                    if ((listItem != null))
                    {
                        rows.FillEntity(listItem);
                    }
                }
            }
        }
        #endregion

        #region IDataReader Extensions

        public static T ToEntity<T>(this IDataReader dr, bool read = true)
            where T : class, new()
        {
            if (read)
                if (!dr.Read())
                    return null;

            T obj = new T();
            if (FillEntity<T>(dr, obj))
                return obj;
            else
                return null;
        }

        public static IEnumerable<T> ToIEnumerable<T>(this IDataReader dr)
            where T : class, new()
        {
            while (dr.Read())
            {
                var entity = dr.ToEntity<T>(false);

                if (entity != null)
                    yield return entity;
            }
        }

        /// <summary>
        /// IDataReader ile okunan değerleri parametre olarak gönderilen T nesnesine doldurur. T nesnesinin özelliklerini kolonlar ile eşleştirir.
        /// </summary>
        /// <remarks>Metodu çağırmadan önce IDataReader.Read() çağırılmalıdır</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="obj"></param>
        public static bool FillEntity<T>(this IDataReader dr, T obj)
            where T : class
        {
            PropertyInfo[] propertyList = typeof(T).GetProperties();
            var columns = dr.GetColumnNames();

            bool anyPropertiesSet = false;

            foreach (PropertyInfo prop in propertyList)
            {
                string columnName = prop.GetAssociatedColumnName();
                if (!columns.ContainsValue(columnName))
                    continue;

                object value = dr[columnName];
                try
                {
                    if (!value.Equals(DBNull.Value))
                    {
                        prop.SetPropertyValue(obj, value);
                        anyPropertiesSet = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException(string.Format("Cast failed, [{0}] typed [{1}] column value [{2}] cannot set to [{3}] which is declaring as [{4}]", value.GetType(), columnName, value, prop.Name, prop.PropertyType), ex);
                }
            }

            return anyPropertiesSet;
        }

        public static void FillList<T>(this IDataReader dr, List<T> objList)
            where T : class
        {
            PropertyInfo[] propertyList = typeof(T).GetProperties();
            PropertyInfo keyProperty = null;

            foreach (PropertyInfo prop in propertyList)
            {
                if (prop.HasAttribute<KeyAttribute>())
                {
                    keyProperty = prop;
                    break;
                }
            }

            if ((keyProperty != null))
            {
                while (dr.Read())
                {
                    object key = Convert.ChangeType(dr[keyProperty.Name], keyProperty.PropertyType);
                    T listItem = objList.Where(o => keyProperty.GetValue(o, null).Equals(key)).SingleOrDefault();

                    if ((listItem != null))
                    {
                        dr.FillEntity(listItem);
                    }
                }
            }
        }

        public static Dictionary<int, string> GetColumnNames(this IDataRecord record)
        {
            var result = new Dictionary<int, string>();
            for (int i = 0; i < record.FieldCount; i++)
            {
                result.Add(i, record.GetName(i));
            }

            return result;
        }

        #endregion

        public static DataTable GetDataTable()
        {
            throw new NotImplementedException();

            //SqlConnection cnn = null;
            //System.Data.DataTable dt = null;
            //SqlCommand cmd = null;
            //SqlDataAdapter adp = null;

            //try
            //{
            //    cnn = new SqlConnection(ConnectionString);
            //    cnn.Open();

            //    cmd = new SqlCommand(strSQL, cnn);
            //    cmd.CommandTimeout = CommandTimeout;

            //    adp = new SqlDataAdapter();
            //    adp.SelectCommand = cmd;

            //    dt = new System.Data.DataTable();
            //    adp.Fill(dt);
            //    adp.Dispose();

            //    cnn.Close();
            //    cnn.Dispose();
            //    return dt;
            //}
            //catch
            //{
            //    if ((cnn != null))
            //    {
            //        cnn.Close();
            //    }
            //    throw;
            //}
        }
    } 
}