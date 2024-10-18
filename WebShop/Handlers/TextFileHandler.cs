
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;

namespace WebShop.Handlers
{
    public class TextFileHandler
    {

        public static List<string> ReadTextFile(string path)
        {
            path = HostingEnvironment.MapPath(path);

            List<string> lines = new List<string>();

            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }

            }

            return lines;
        }

        public static void InsertEntityIntoFile<T>(T entity, string path) where T : class
        {

            path = HostingEnvironment.MapPath(path);

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(entity);
            }
        }

        public static void InsertEntitiesIntoFile<T>(List<T> entities, string path) where T : class
        {

            path = HostingEnvironment.MapPath(path);

            using (StreamWriter sw = File.CreateText(path))
            {
                entities.ForEach(x => sw.WriteLine(x));
            }
        }

    }
}