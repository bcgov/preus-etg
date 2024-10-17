using CJG.Core.Entities;
using CJG.Core.Interfaces.Service;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CJG.Web.External.Helpers
{
    /// <summary>
    /// <typeparamref name="TempDataExtensions"/> static class, provides extension methods related to <typeparamref name="TempData"/> objects.
    /// </summary>
    public static class TempDataExtensions
    {
        /// <summary>
        /// Serialize the specified object data into a string.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeToString<TEntity>(this TEntity data) where TEntity : class
        {
            var serializer = new DataContractSerializer(typeof(TEntity));

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, data);
                stream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Serialize the specified object data into an <typeparamref name="XElement"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static XElement SerializeToXml<TEntity>(this TEntity data) where TEntity : class
        {
            var serializer = new DataContractSerializer(typeof(TEntity));

            using (var stream = new MemoryStream())
            {
                var writer = XmlWriter.Create(stream);

                serializer.WriteObject(writer, data);
                writer.Flush();

                return XElement.Parse(writer.ToString());
            }
        }

        /// <summary>
        /// Deserialize the specified <typeparamref name="TempData"/> object into the specified type (TEntity).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static TEntity Deserialize<TEntity>(this TempData temp) where TEntity : class
        {
            return (TEntity)Deserialize(temp);
        }

        /// <summary>
        /// Deserialize the specified <typeparamref name="TempData"/> object into the specified type (TEntity).
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static object Deserialize(this TempData temp)
        {
            using (var stream = new MemoryStream())
            {
                var type = Type.GetType(temp.DataType);
                var data = System.Text.Encoding.UTF8.GetBytes(temp.Data);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                var serializer = new DataContractSerializer(type);
                return serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Get the deserialized value from the specified <typeparamref name="TempData"/>.
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static object Value(this TempData temp)
        {
            return temp.Deserialize();
        }

        /// <summary>
        /// Get the deserialized value from the specified <typeparamref name="TempData"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static TEntity Value<TEntity>(this TempData temp) where TEntity : class
        {
            return temp.Deserialize<TEntity>();
        }

        /// <summary>
        /// Get the specified value of type (TEntity) from the <typeparamref name="TempData"/> datastore in the database.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="service"></param>
        /// <param name="parentType"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public static TEntity Get<TEntity>(this ITempDataService service, Type parentType, int parentId) where TEntity : class
        {
            if (parentId == 0)
                return null;

            var temp = service.Get(parentType, parentId, typeof(TEntity));

            if (temp == null)
                return null;

            return temp.Deserialize<TEntity>();
        }

        /// <summary>
        /// Add a new object into the <typeparamref name="TempData"/> datastore.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="service"></param>
        /// <param name="parentType"></param>
        /// <param name="parentId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TempData Add<TEntity>(this ITempDataService service, Type parentType, int parentId, TEntity data) where TEntity : class
        {
            if (parentId <= 0)
                throw new ArgumentException($"The argument '{nameof(parentId)}' cannot be less than or equal to 0.");

            var result = data.SerializeToString();
            var temp = new TempData(parentType, parentId, typeof(TEntity), result);
            service.Add(temp);
            return temp;
        }

        /// <summary>
        /// Add or update an object in the <typeparamref name="TempData"/> datastore.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="service"></param>
        /// <param name="parentType"></param>
        /// <param name="parentId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TempData AddOrUpdate<TEntity>(this ITempDataService service, Type parentType, int parentId, TEntity data) where TEntity : class
        {
            if (parentId <= 0)
                throw new ArgumentException($"The argument '{nameof(parentId)}' cannot be less than or equal to 0.");

            var result = data.SerializeToString();
            var temp = new TempData(parentType, parentId, typeof(TEntity), result);
            service.AddOrUpdate(temp);
            return temp;
        }

        /// <summary>
        /// Get the object related to the GrantApplication of the specified type (TEntity).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="service"></param>
        /// <param name="grantApplicationId"></param>
        /// <returns></returns>
        public static TEntity GetGrantApplicationSection<TEntity>(this ITempDataService service, int grantApplicationId) where TEntity : class
        {
            return service.Get<TEntity>(typeof(GrantApplication), grantApplicationId);
        }

        /// <summary>
        /// Add or update the object in the <typeparamref name="TempData"/> datastore related to the GrantApplication.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="service"></param>
        /// <param name="grantApplicationId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TempData AddOrUpdateGrantApplicationSection<TEntity>(this ITempDataService service, int grantApplicationId, TEntity data) where TEntity : class
        {
            return service.AddOrUpdate<TEntity>(typeof(GrantApplication), grantApplicationId, data);
        }

        /// <summary>
        /// Delete the object in the <typeparamref name="TempData"/> datastore related to the GrantApplication.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="service"></param>
        /// <param name="grantApplicationId"></param>
        public static void DeleteGrantApplicationSection<TEntity>(this ITempDataService service, int grantApplicationId) where TEntity : class
        {
            service.Delete(typeof(GrantApplication), grantApplicationId, typeof(TEntity));
        }

        public static TEntity[] ResolveViewData<TEntity>(this ViewDataDictionary viewData, string[] dataNames)
        {
            var viewDataValues = new TEntity[dataNames.Length];
            TEntity entity;
            var counter = 0;

            foreach (var dataName in dataNames)
            {
                if (viewData[dataName] != null)
                {
                    entity = (TEntity)viewData[dataName];
                    viewDataValues[counter] = entity;
                }

                counter++;
            }

            return viewDataValues;
        }
    }
}