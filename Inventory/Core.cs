using Inventory.Model;
using Newtonsoft.Json.Linq;
using PCLStorage;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inventory
{
    public class Core
    {
        // Settings
        public static Dictionary<string, string> Settings = new Dictionary<string, string>
        {
            { "product_defaults", "Product Defaults" },
            { "cloud", "OneDrive Sync" },
        };

        // User (online)
        public static int UserID = 0;
        public static int HouseID = 0;
        public static int HouseOwnerID = 0;

        static bool IsOffline
        {
            get { return UserID == 0; }
        }


        // Storage
        static List<Item> coreItems = new List<Item>();

        static string folderName = "Stuff";

        static object locker = new object();

        //static SQLiteConnection database;

        static string DatabasePath
        {
            get { return Task.Run(() => GetPathForFile("stuff.db3")).Result; }
        }
        
        static string ProductDefaultsPath
        {
            get { return Task.Run(() => GetPathForFile(ProductDefaults.Filename)).Result; }
        }
        
        static string FiltersPath
        {
            get { return Task.Run(() => GetPathForFile(Filters.Filename)).Result; }
        }

        public static async Task<string> GetPathForFile(string filename)
        {
            System.Diagnostics.Debug.WriteLine("Core.GetPathForFile()");

            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // create a folder if one does not exist already
            IFolder folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            // create a file if one does not exist already
            IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

            System.Diagnostics.Debug.WriteLine("Path for file: " + file.Path);

            // get path
            return file.Path;
        }
        /*
        /// <summary>
        /// Removes image from local filesystem.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static async Task DeleteLocalPhotoAsync(string filename)
        {
            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // create a folder if one does not exist already
            IFolder folder = await rootFolder.GetFolderAsync(folderName);

            // delete
            IFile file = await folder.GetFileAsync(filename);
            await file.DeleteAsync();

            // something to do with thumbnail here?
        }

        private static async Task SaveLocalPhotoAsync(string filename, byte[] data)
        {
            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // create a folder if one does not exist already
            IFolder folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            // create a file, or replace existing
            IFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            await file.WriteAllTextAsync(Convert.ToBase64String(data));
        }

        private static async Task<byte[]> GetLocalPhotoAsync(string filename)
        {
            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // open folder
            IFolder folder = await rootFolder.GetFolderAsync(folderName);

            // open
            try
            {
                IFile file = await folder.GetFileAsync(filename);
                return Convert.FromBase64String(await file.ReadAllTextAsync());
            }
            catch (Exception)
            {
                return null;
            }
        }
        */

        // smelly... i'd like to merge these funcs into a getjson or something
        public static string GetFilters()
        {
            return Task.Run(() => GetFiltersAsync()).Result;
        }

        async static Task<string> GetFiltersAsync()
        {
            // create filters path
            Filters.Path = FiltersPath;

            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // open folder
            IFolder folder = await rootFolder.GetFolderAsync(folderName);

            // open
            try
            {
                IFile file = await folder.GetFileAsync(FiltersPath);
                return await file.ReadAllTextAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetProductDefaults()
        {
            return Task.Run(() => GetProductDefaultsAsync()).Result;
        }

        async static Task<string> GetProductDefaultsAsync()
        {
            // create product defaults for barcode scanning
            ProductDefaults.Path = ProductDefaultsPath;

            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // open folder
            IFolder folder = await rootFolder.GetFolderAsync(folderName);

            // open
            try
            {
                IFile file = await folder.GetFileAsync(ProductDefaultsPath);
                return await file.ReadAllTextAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void WriteFilters(string json)
        {
            Task.Run(() => WriteFiltersAsync(json));
        }

        async static Task WriteFiltersAsync(string json)
        {
            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // create a folder if one does not exist already
            IFolder folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            // create a file, or replace existing
            IFile file = await folder.CreateFileAsync(FiltersPath, CreationCollisionOption.ReplaceExisting);

            await file.WriteAllTextAsync(json);
        }


        public static void WriteProductDefaults(string json)
        {
            Task.Run(() => WriteProductDefaultsAsync(json));
        }

        async static Task WriteProductDefaultsAsync(string json)
        {
            // get hold of the file system
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            // create a folder if one does not exist already
            IFolder folder = await rootFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            // create a file, or replace existing
            IFile file = await folder.CreateFileAsync(ProductDefaultsPath, CreationCollisionOption.ReplaceExisting);

            await file.WriteAllTextAsync(json);
        }
        
        /*
        /// <summary>
        /// Gets up-to-date list of all Items in a House.
        /// </summary>
        /// <returns>List of Item objects representing the Items within a House.</returns>
        public async static Task<List<Item>> GetItems(string path)
        {
            if (IsOffline)
            {
                lock (locker)
                {
                    coreItems = database.Query<Item>("SELECT * FROM [Item]");
                }
            }
            else
            {
                JArray jArray = await Task.Run(() => Query.GetItems(HouseID));
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject job = (JObject)jArray[i];
                    coreItems.Add(new Item(job));
                }
            }

            // filter by specified attribute, if not empty
            if (!string.IsNullOrEmpty(path))
            {
                string withoutChild = path.Substring(0, path.LastIndexOf("/")).Trim();

                if (Filter == FILTER_LOCATIONS)
                {
                    coreItems = coreItems.Where(x => x.Location.StartsWith(path) || x.Location.StartsWith(withoutChild)).Distinct().ToList();
                }
                else if (Filter == FILTER_CATEGORIES)
                {
                    coreItems = coreItems.Where(x => x.Category.StartsWith(path) || x.Category.StartsWith(withoutChild)).Distinct().ToList();
                }
            }

            System.Diagnostics.Debug.WriteLine("Sorting with order {0} and type {1}", Order, Sort);

            // sort + order
            if (Order == ORDER_DESC)
            {
                if (Sort == SORT_RECENTLY_ADDED)
                {
                    coreItems = coreItems.OrderByDescending(x => x.ID).ToList();
                }
                else if (Sort == SORT_DATE_MODIFIED)
                {
                    // TODO
                    coreItems = coreItems.OrderByDescending(x => x.ID).ToList(); // change to modified string
                }
                else if (Sort == SORT_ALPHABETICALLY)
                {
                    coreItems = coreItems.OrderByDescending(x => x.Name).ToList();
                }
                else if (Sort == SORT_DATE_PURCHASED)
                {
                    coreItems = coreItems.OrderByDescending(x => x.DatePurchased).ToList();
                }
                else if (Sort == SORT_VALUE)
                {
                    coreItems = coreItems.OrderByDescending(x => x.Value).ToList();
                }
            }
            else
            {
                if (Sort == SORT_RECENTLY_ADDED)
                {
                    coreItems = coreItems.OrderBy(x => x.ID).ToList();
                }
                else if (Sort == SORT_DATE_MODIFIED)
                {
                    // TODO
                    coreItems = coreItems.OrderBy(x => x.ID).ToList(); // change to modified string
                }
                else if (Sort == SORT_ALPHABETICALLY)
                {
                    coreItems = coreItems.OrderBy(x => x.Name).ToList();
                }
                else if (Sort == SORT_DATE_PURCHASED)
                {
                    coreItems = coreItems.OrderBy(x => x.DatePurchased).ToList();
                }
                else if (Sort == SORT_VALUE)
                {
                    coreItems = coreItems.OrderBy(x => x.Value).ToList();
                }
            }

            return coreItems;
        }
        */

        /// <summary>
        /// Formats a list of child attributes for user readability.
        /// </summary>
        /// <param name="list">
        /// The unformatted list of child attributes.
        /// </param>
        /// <returns>
        /// The formatted list of child attributes.
        /// </returns>
        /*
        public static List<string> GetDisplayableChildAttributes(List<string> list)
        {
            List<string> listToDisplay = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                listToDisplay.Add(list[i].Replace("/", "").Trim());
            }

            return listToDisplay;
        }

        /// <summary>
        /// Gets all child attributes for a given path (unformatted).
        /// </summary>
        /// <param name="allAttributes">
        /// The entire list of attributes containing subattributes, etc.
        /// </param>
        /// <param name="path">
        /// The current "parent" (and corresponding parents) of the attribute.
        /// </param>
        /// <returns>
        /// An unformatted list of attributes that are children to the specified path.
        /// </returns>
        public static List<string> GetChildAttributes(List<string> allAttributes, string path)
        {
            // remove the excess
            for (int i = allAttributes.Count - 1; i >= 0; i--)
            {
                System.Diagnostics.Debug.WriteLine("checking {0}...", allAttributes[i]);

                if (!allAttributes[i].EndsWith("/"))
                {
                    allAttributes[i] += "/";
                }

                string tempAttr = Regex.Replace(allAttributes[i], @"\s+", "");
                string tempPath = Regex.Replace(path, @"\s+", "");

                System.Diagnostics.Debug.WriteLine("comparing attr {0} to path {1}...", tempAttr, tempPath);

                // tricky.... remove parent? (we're putting it back later.. so...)
                if (!tempAttr.StartsWith(tempPath) || tempAttr.Equals(tempPath))
                {
                    System.Diagnostics.Debug.WriteLine("attr {0} does not match or is the same. removing...", tempAttr);

                    allAttributes.RemoveAt(i);
                }
            }

            // get children
            for (int i = 0; i < allAttributes.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine("concatenating {0}...", allAttributes[i]);

                // clip parent from attribute
                if (path.Length > 0)
                {
                    allAttributes[i] = allAttributes[i].Substring(path.Length);

                    System.Diagnostics.Debug.WriteLine("w/o parent: " + allAttributes[i]);
                }

                // read to 1st index of "/"
                allAttributes[i] = allAttributes[i].Substring(0, allAttributes[i].IndexOf("/") + 1);

                System.Diagnostics.Debug.WriteLine("after concat: {0}", allAttributes[i]);
            }
            
            // merge directories that are named the same have children and don't (e.g. "Sean's Room/" and "Sean's Room /")
            if (allAttributes.Count > 1)
            {
                for (int i = allAttributes.Count - 1; i >= 0; i--)
                {
                    // remove spacing from this attribute
                    string tmp = allAttributes[i];
                    tmp = Regex.Replace(tmp, @"\s+", "");

                    System.Diagnostics.Debug.WriteLine("comparing {0}...", tmp);

                    foreach (string attr in allAttributes)
                    {
                        // remove spacing from the other attributes
                        string tmpAttr = attr;
                        tmpAttr = Regex.Replace(tmpAttr, @"\s+", "");

                        System.Diagnostics.Debug.WriteLine("against {0}...", tmpAttr);

                        // remove the shorter one (longer one will have children)
                        if (tmp.Equals(tmpAttr) && allAttributes.IndexOf(attr) != i)
                        {
                            System.Diagnostics.Debug.WriteLine("match at index {0} where {1} = {2} ({3})", allAttributes.IndexOf(attr), tmp, tmpAttr, attr);

                            if (allAttributes[i].Length < attr.Length)
                            {
                                System.Diagnostics.Debug.WriteLine("{0} is shorter. Removing...", allAttributes[i]);
                                allAttributes.RemoveAt(i);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("{0} is shorter. Removing...", attr);
                                allAttributes.RemoveAt(allAttributes.IndexOf(attr));
                            }

                            break;
                        }
                    }
                }
            }

            // insert (grand)parent directories
            if (AttributeStack.Count == 1)
            {
                System.Diagnostics.Debug.WriteLine("parent is house (root)...");

                allAttributes.Insert(0, "My House");
            }
            else if (AttributeStack.Count == 2)
            {
                System.Diagnostics.Debug.WriteLine("making parent...");

                allAttributes.Insert(0, path);
                allAttributes.Insert(0, "My House");
            }
            else if (AttributeStack.Count > 2)
            {
                System.Diagnostics.Debug.WriteLine("making parent and grandparent...");

                string parent = string.Empty;
                string grandparent = string.Empty;

                int count = 0;

                int grandparentStart = 0;
                int grandparentLength = 0;

                int parentStart = 0;
                int parentLength = 0;

                for (int i = 0; i < path.Length; i++)
                {
                    if (path[i] == '/')
                    {
                        if (count == AttributeStack.Count - 4)
                        {
                            grandparentStart = i;
                        }
                        else if (count == AttributeStack.Count - 3)
                        {
                            parentStart = i;
                            grandparentLength = i - grandparentStart + 1;
                        }
                        else if (count == AttributeStack.Count - 2)
                        {
                            parentLength = i - parentStart + 1;
                        }

                        count++;
                    }
                }

                parent = path.Substring(parentStart, parentLength);
                grandparent = path.Substring(grandparentStart, grandparentLength);
                
                allAttributes.Insert(0, parent);
                allAttributes.Insert(0, grandparent);
            }

            // remove duplicates
            allAttributes = allAttributes.Distinct().ToList();

            return allAttributes;
        }
        */
        /*
        public async static Task<byte[]> GetImage(string filename)
        {
            byte[] data = null;

            if (IsOffline)
            {
                lock (locker)
                {
                    data = Task.Run(() => GetLocalPhotoAsync(filename)).Result;
                }
            }
            else
            {
                data = await Task.Run(() => Query.GetImage(filename));
            }

            return data;
        }
        */
        /*
        public async static Task<byte[]> GetImageThumbnail(string filename)
        {
            byte[] data = null;

            if (IsOffline)
            {
                lock (locker)
                {
                    // hmm do we need to downsize it? idk
                    data = Task.Run(() => GetLocalPhotoAsync(filename)).Result;
                }
            }
            else
            {
                data = await Task.Run(() => Query.GetImage(string.Format("thumb_" + filename)));
            }

            return data;
        }
        */

            /*
        
        public async static Task<string> GetHouseName()
        {
            string name = "My House";

            if (!IsOffline)
            {
                JArray jArray = await Task.Run(() => Query.GetHouse(HouseID));

                try
                {
                    JObject house = (JObject)jArray[0];
                    name = (string)house["name"];
                }
                catch (Exception)
                {
                    name = null;
                }
            }

            return name;
        }
        */
    }
}
