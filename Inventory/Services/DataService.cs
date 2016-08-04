using Inventory.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Model;
using MvvmCross.Plugins.Sqlite;
using SQLite.Net;
using Inventory.Constants;
using Inventory.Messages;
using MvvmCross.Plugins.Messenger;
using System.Text.RegularExpressions;
using Inventory.Resources;

namespace Inventory.Services
{
    public class DataService : IDataService
    {
        private readonly IMvxSqliteConnectionFactory _connectionFactory;
        private readonly SQLiteConnection _connection;
        private readonly IMvxMessenger _messenger;

        private Stack<string> _attributeStack = new Stack<string>();
        private string _attributePath = "";

        public DataService(IMvxSqliteConnectionFactory factory, IMvxMessenger messenger)
        {
            _connectionFactory = factory;
            _connection = _connectionFactory.GetConnection(DatabaseConstants.DB_NAME);
            _connection.CreateTable<Item>();
            _connection.CreateTable<ShoppingItem>();

            _messenger = messenger;
        }

        public int ItemCount
        {
            get
            {
                return _connection.Table<Item>().Count();
            }
        }

        public int ShoppingItemCount
        {
            get
            {
                return _connection.Table<ShoppingItem>().Count();
            }
        }

        public void ResetAttributeStack()
        {
            _attributePath = "";
            _attributeStack.Clear();
            _attributeStack.Push("");
        }

        public void PopAttribute()
        {
            _attributeStack.Pop();
        }

        public void PushAttribute(string path)
        {
            _attributeStack.Push(path);
        }

        public Stack<string> GetAttributeStack()
        {
            return _attributeStack;
        }

        public string GetAttributePath()
        {
            return _attributePath; 
        }

        public void SetAttributePath(string path)
        {
            _attributePath = path;
        }

        public List<string> GetChildPaths()
        {
            System.Diagnostics.Debug.WriteLine("DataService.GetChildPaths()");

            // Filter by categories or locations
            var attributeType = Filters.Read().Attribute;
            var paths = new List<string>();

            if (attributeType == Filters.ATTRIBUTE_CATEGORIES)
            {
                System.Diagnostics.Debug.WriteLine("For categories...");

                paths = GetCategories();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("For locations...");

                paths = GetLocations();
            }

            // Remove the excess
            for (int i = paths.Count - 1; i >= 0; i--)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("checking '{0}'...", paths[i]));

                if (!paths[i].EndsWith("/"))
                {
                    paths[i] += "/";
                }

                string tempAttr = Regex.Replace(paths[i], @"\s+", "");
                string tempPath = Regex.Replace(_attributePath, @"\s+", "");

                System.Diagnostics.Debug.WriteLine(string.Format("comparing attr '{0}' to path '{1}'...", tempAttr, tempPath));

                // tricky.... remove parent? (we're putting it back later.. so...)
                if (!tempAttr.StartsWith(tempPath) || tempAttr.Equals(tempPath))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("attr '{0}' does not match or is the same. removing...", tempAttr));

                    paths.RemoveAt(i);
                }
            }

            // Get children
            for (int i = 0; i < paths.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("concatenating '{0}'...", paths[i]));

                // clip parent from attribute
                if (_attributePath.Length > 0)
                {
                    paths[i] = paths[i].Substring(_attributePath.Length);

                    System.Diagnostics.Debug.WriteLine(string.Format("w/o parent: '{0}'", paths[i]));
                }

                // read to 1st index of "/"
                paths[i] = paths[i].Substring(0, paths[i].IndexOf("/") + 1);

                System.Diagnostics.Debug.WriteLine(string.Format("after concat: '{0}'", paths[i]));
            }

            // Merge directories that are named the same
            // where some have children and some don't 
            // (e.g. "Sean's Room/" and "Sean's Room /")
            if (paths.Count > 1)
            {
                for (int i = paths.Count - 1; i >= 0; i--)
                {
                    // remove spacing from this attribute
                    string tmp = paths[i];
                    tmp = Regex.Replace(tmp, @"\s+", "");

                    System.Diagnostics.Debug.WriteLine(string.Format("comparing '{0}'...", tmp));

                    foreach (string attr in paths)
                    {
                        // remove spacing from the other attributes
                        string tmpAttr = attr;
                        tmpAttr = Regex.Replace(tmpAttr, @"\s+", "");

                        System.Diagnostics.Debug.WriteLine(string.Format("against '{0}'...", tmpAttr));

                        // remove the shorter one (longer one will have children)
                        if (tmp.Equals(tmpAttr) && paths.IndexOf(attr) != i)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("match at index {0} where '{1}' = '{2}' ({3})", paths.IndexOf(attr), tmp, tmpAttr, attr));

                            if (paths[i].Length < attr.Length)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("{0} is shorter. Removing...", paths[i]));
                                paths.RemoveAt(i);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("{0} is shorter. Removing...", attr));
                                paths.RemoveAt(paths.IndexOf(attr));
                            }

                            break;
                        }
                    }
                }
            }

            // Alphabetize
            paths.Sort();

            // Insert (grand)parent directories
            if (_attributeStack.Count == 1)
            {
                System.Diagnostics.Debug.WriteLine("parent is house (root)...");

                paths.Insert(0, "My House");
                //paths.Insert(0, Strings.HouseLabel);
            }
            else if (_attributeStack.Count == 2)
            {
                System.Diagnostics.Debug.WriteLine("making parent...");

                paths.Insert(0, _attributePath);
                paths.Insert(0, "My House");
                //paths.Insert(0, Strings.HouseLabel);
            }
            else if (_attributeStack.Count > 2)
            {
                System.Diagnostics.Debug.WriteLine("making parent and grandparent...");

                string parent = string.Empty;
                string grandparent = string.Empty;

                int count = 0;

                int grandparentStart = 0;
                int grandparentLength = 0;

                int parentStart = 0;
                int parentLength = 0;

                for (int i = 0; i < _attributePath.Length; i++)
                {
                    if (_attributePath[i] == '/')
                    {
                        if (count == _attributeStack.Count - 4)
                        {
                            grandparentStart = i;
                        }
                        else if (count == _attributeStack.Count - 3)
                        {
                            parentStart = i;
                            grandparentLength = i - grandparentStart + 1;
                        }
                        else if (count == _attributeStack.Count - 2)
                        {
                            parentLength = i - parentStart + 1;
                        }

                        count++;
                    }
                }

                parent = _attributePath.Substring(parentStart, parentLength);
                grandparent = _attributePath.Substring(grandparentStart, grandparentLength);

                paths.Insert(0, parent);
                paths.Insert(0, grandparent);
            }

            // Remove duplicates
            paths = paths.Distinct().ToList();

            // Remove empties or solo '/'s
            paths = paths.Where(x => !string.IsNullOrEmpty(x) && !x.Equals("/")).ToList();
            
            return paths;
        }

        public List<string> GetPresentablePaths(List<string> paths)
        {
            System.Diagnostics.Debug.WriteLine("DataService.GetPresentablePaths()");

            var listToDisplay = new List<string>();
            //var paths = GetChildPaths();

            for (int i = 0; i < paths.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine("Getting path: " + paths[i]);

                listToDisplay.Add(paths[i].Replace("/", "").Trim());
            }

            // Remove blanks
            listToDisplay = listToDisplay.Where(x => !string.IsNullOrEmpty(x)).ToList();

            return listToDisplay;
        }

        public List<Item> AllItems()
        {
            var filters = Filters.Read();

            // Get all items
            var items = _connection.Table<Item>().ToList();

            // Get only those in path, if not empty
            if (!string.IsNullOrEmpty(_attributePath))
            {
                System.Diagnostics.Debug.WriteLine("Getting all items with path: " + _attributePath);

                string withoutChild = _attributePath.Substring(0, _attributePath.LastIndexOf("/")).Trim();

                System.Diagnostics.Debug.WriteLine("Without child: " + withoutChild);

                if (filters.Attribute == Filters.ATTRIBUTE_CATEGORIES)
                {
                    items = items.Where(x => x.Category.StartsWith(_attributePath) ||
                    x.Category.StartsWith(withoutChild))
                    .Distinct()
                    .ToList();
                }
                else
                {
                    items = items.Where(x => x.Location.StartsWith(_attributePath) ||
                    x.Location.StartsWith(withoutChild))
                    .Distinct()
                    .ToList();
                }
            }

            if (filters.Order == Filters.ORDER_ASC)
            {
                switch (filters.Sort)
                {
                    case Filters.SORT_ALPHABETICALLY:
                        items = items
                            .OrderBy(x => x.Name)
                            .ToList();
                        break;
                    case Filters.SORT_DATE_MODIFIED: // add new property to Item.cs ("LastEditTime")
                        items = items
                            .OrderBy(x => x.ID)
                            .ToList();
                        break;
                    case Filters.SORT_DATE_PURCHASED:
                        items = items
                            .OrderBy(x => x.DatePurchased)
                            .ToList();
                        break;
                    case Filters.SORT_RECENTLY_ADDED:
                        items = items
                            .OrderBy(x => x.ID)
                            .ToList();
                        break;
                    case Filters.SORT_VALUE:
                        items = items
                            .OrderBy(x => x.Value)
                            .ToList();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (filters.Sort)
                {
                    case Filters.SORT_ALPHABETICALLY:
                        items = items
                            .OrderByDescending(x => x.Name)
                            .ToList();
                        break;
                    case Filters.SORT_DATE_MODIFIED: // add new property to Item.cs ("LastEditTime")
                        items = items
                            .OrderByDescending(x => x.ID)
                            .ToList();
                        break;
                    case Filters.SORT_DATE_PURCHASED:
                        items = items
                            .OrderByDescending(x => x.DatePurchased)
                            .ToList();
                        break;
                    case Filters.SORT_RECENTLY_ADDED:
                        items = items
                            .OrderByDescending(x => x.ID)
                            .ToList();
                        break;
                    case Filters.SORT_VALUE:
                        items = items
                            .OrderByDescending(x => x.Value)
                            .ToList();
                        break;
                    default:
                        break;
                }
            }

            return items;
        }

        public List<ShoppingItem> AllShoppingItems()
        {
            var allShoppingItems = _connection.Table<ShoppingItem>()
                .OrderByDescending(x => x.ID)
                .ToList();

            // cleanup old, completed shopping items
            for (int i = allShoppingItems.Count() - 1; i >= 0; i--)
            {
                if (allShoppingItems[i].ShouldRemove())
                {
                    DeleteShoppingItem(allShoppingItems[i].ID);
                    allShoppingItems.RemoveAt(i);
                }
            }

            return allShoppingItems;
        }

        public void DeleteItem(int id)
        {
            _connection.Delete<Item>(id);

            // remove shopping item if necessary
            try
            {
                DeleteShoppingItem(GetShoppingItemFromItem(id).ID);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("No corresponding shopping list item");
            }

            var message = new CollectionChangedMessage(this, true);
            _messenger.Publish(message);
        }

        public void DeleteShoppingItem(int id)
        {
            _connection.Delete<ShoppingItem>(id);

            var message = new CollectionChangedMessage(this, true);
            _messenger.Publish(message);
        }

        public List<string> GetCategories()
        {
            return _connection.Table<Item>()
                .Select(x => x.Category)
                .Distinct()
                .ToList();
        }

        public bool GetDiff(string initial, string current)
        {
            bool changesMade = false;

            if (!string.IsNullOrEmpty(initial))
            {
                if (string.Compare(initial, current) != 0)
                {
                    changesMade = true;
                }
            }

            return changesMade;
        }

        public List<string> GetLocations()
        {
            return _connection.Table<Item>()
                .Select(x => x.Location)
                .Distinct()
                .ToList();
        }

        public Item GetItem(int id)
        {
            return _connection.Table<Item>()
                .FirstOrDefault(x => x.ID == id);
        }

        public int GetItemIdForBarcode(string barcode)
        {
            int id;

            try
            {
                id = _connection.Table<Item>()
                    .FirstOrDefault(x => x.Barcode.Equals(barcode)).ID;
            }
            catch (Exception)
            {
                id = 0;
            }

            return id;
        }

        public ShoppingItem GetShoppingItem(int id)
        {
            return _connection.Table<ShoppingItem>()
                .FirstOrDefault(x => x.ID == id);
        }

        public ShoppingItem GetShoppingItemFromItem(int id)
        {
            return _connection.Table<ShoppingItem>()
                .FirstOrDefault(x => x.ItemID == id);
        }

        public List<Item> ItemsMatching(string query)
        {
            return _connection.Table<Item>()
                .Where(x => x.Name.ToLower().Contains(query.ToLower()) ||
                x.Notes.ToLower().Contains(query.ToLower()) ||
                x.Location.ToLower().Contains(query.ToLower()) ||
                x.Category.ToLower().Contains(query.ToLower())).ToList();
        }

        public int SaveItem(Item item)
        {
            System.Diagnostics.Debug.WriteLine("Saving item with name: " + item.Name);

            item.Category = TrimPath(item.Category);
            item.Location = TrimPath(item.Location);

            if (item.ID == 0)
            {
                _connection.Insert(item);
            }
            else
            {
                _connection.Update(item);
            }

            // Add to shopping list if necessary
            if (item.QuantityForShoppingList > -1)
            {
                ShoppingItem shoppingItem = GetShoppingItemFromItem(item.ID);

                if (item.Quantity <= item.QuantityForShoppingList)
                {
                    if (shoppingItem == null)
                    {
                        shoppingItem = new ShoppingItem(item.ID, item.Name, 1, item.Notes, false, false, new DateTime(1, 1, 1));
                    }

                    // Set our desired shopping quantity to threshold + 1
                    shoppingItem.Quantity = item.QuantityForShoppingList - item.Quantity + 1;

                    SaveShoppingItem(shoppingItem);
                }
            }

            var message = new CollectionChangedMessage(this, true);
            _messenger.Publish(message);

            return item.ID;
        }

        public void SaveShoppingItem(ShoppingItem shoppingItem)
        {
            if (shoppingItem.ID == 0)
            {
                _connection.Insert(shoppingItem);
            }
            else
            {
                _connection.Update(shoppingItem);
            }

            // add to inventory?
            if (shoppingItem.IsCompleted)
            {
                if (shoppingItem.MakeItem && shoppingItem.ItemID == 0)
                {
                    var item = new Item();

                    item.Name = shoppingItem.Name;
                    item.Notes = shoppingItem.Notes;
                    item.Quantity = shoppingItem.Quantity;
                    item.QuantityForShoppingList = 0;
                    item.DatePurchased = DateTime.Now;

                    shoppingItem.ItemID = SaveItem(item);
                    shoppingItem.MakeItem = false;

                    SaveShoppingItem(shoppingItem);
                }
            }

            var message = new CollectionChangedMessage(this, true);
            _messenger.Publish(message);
        }

        public List<ShoppingItem> ShoppingItemsMatching(string query)
        {
            throw new NotImplementedException();
        }

        public string TrimPath(string attributePath)
        {
            try
            {
                string[] path = attributePath.Split('/');
                attributePath = string.Empty;

                for (int i = 0; i < path.Length; i++)
                {
                    path[i] = path[i].Trim();
                    attributePath += path[i];

                    if (i < path.Length - 1)
                    {
                        attributePath += "/";
                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Nothing to trim");
            }

            return attributePath;
        }
    }
}
