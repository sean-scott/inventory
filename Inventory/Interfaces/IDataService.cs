using Inventory.Model;
using System.Collections.Generic;

namespace Inventory.Interfaces
{
    public interface IDataService
    {
        List<Item> AllItems();
        List<ShoppingItem> AllShoppingItems();

        List<string> GetCategories();
        List<string> GetLocations();

        Stack<string> GetAttributeStack();
        string GetAttributePath();
        void SetAttributePath(string path);

        List<Item> ItemsMatching(string query);
        List<ShoppingItem> ShoppingItemsMatching(string query);

        Item GetItem(int id);
        int GetItemIdForBarcode(string barcode);
        ShoppingItem GetShoppingItem(int id);
        ShoppingItem GetShoppingItemFromItem(int id);

        /// <summary>
        /// Clears attribute stack and pushes an empty string onto it.
        /// To be used when initializing this class, or changing attribute filter.
        /// i.e., Location -> Category, etc.
        /// </summary>
        void ResetAttributeStack();

        void PopAttribute();
        void PushAttribute(string path);

        /// <summary>
        /// Gets all child attributes from path (messy).
        /// </summary>
        List<string> GetChildPaths();

        /// <summary>
        /// Gets a clean list of user-readable paths.
        /// </summary>
        /// <returns></returns>
        List<string> GetPresentablePaths(List<string> paths);


        int SaveItem(Item item);
        void SaveShoppingItem(ShoppingItem shoppingItem);

        void DeleteItem(int id);
        void DeleteShoppingItem(int id);

        int ItemCount { get; }
        int ShoppingItemCount { get; }

        /// <summary>
        /// Standardizes an attribute string by removing spaces around "/"
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        string TrimPath(string attribute);

        /// <summary>
        /// Compares initial file against current values.
        /// </summary>
        /// <returns>If changes were made</returns>
        bool GetDiff(string initial, string current);
    }
}
