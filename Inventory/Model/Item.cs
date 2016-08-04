using System;

using Newtonsoft.Json.Linq;
using SQLite.Net.Attributes;
using PropertyChanged;

namespace Inventory.Model
{
    [ImplementPropertyChanged]
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Barcode { get; set; }
        public string ImageFilename { get; set; }
        public string Name { get; set; }
        public DateTime DatePurchased { get; set; }
        public int Quantity { get; set; }
        public int QuantityForShoppingList { get; set; }
        public decimal Value { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string Notes { get; set; }

        public Item()
        {
            ID = 0;
            Barcode = string.Empty;
            ImageFilename = string.Empty;
            Name = string.Empty;
            DatePurchased = new DateTime(1,1,1);
            Quantity = -1;
            QuantityForShoppingList = -1;
            Value = -1;
            Location = string.Empty;
            Category = string.Empty;
            Notes = string.Empty;
        }

        /// <summary>
        /// Item constructor without identifier.
        /// Useful for creating Item object when ID is not known.
        /// </summary>
        /// <param Name="barcode"></param>
        /// <param Name="imageFileName"></param>
        /// <param Name="name"></param>
        /// <param Name="DatePurchased"></param>
        /// <param Name="Quantity"></param>
        /// <param Name="Value"></param>
        /// <param Name="location"></param>
        /// <param Name="Category"></param>
        /// <param Name="Notes"></param>
        public Item(string barcode, string imageFileName, string name, DateTime datePurchased, int quantity, int quantityForShoppingList, decimal value, string location, string category, string notes)
        {
            Barcode = barcode;
            ImageFilename = imageFileName;
            Name = name;
            DatePurchased = datePurchased;
            Quantity = quantity;
            QuantityForShoppingList = quantityForShoppingList;
            Value = value;
            Location = location;
            Category = category;
            Notes = notes;
        }

        /// <summary>
        /// Item constructor with identifier.
        /// Typically used when updating an Item object's values when it already has an assigned identifier.
        /// </summary>
        /// <param Name="id"></param>
        /// <param Name="barcode"></param>
        /// <param Name="imageFileName"></param>
        /// <param Name="name"></param>
        /// <param Name="DatePurchased"></param>
        /// <param Name="Quantity"></param>
        /// <param Name="Value"></param>
        /// <param Name="location"></param>
        /// <param Name="Category"></param>
        /// <param Name="Notes"></param>
        public Item(int id, string barcode, string imageFileName, string name, DateTime datePurchased, int quantity, int quantityForShoppingList, decimal value, string location, string category, string notes)
        {
            ID = id;
            Barcode = barcode;
            ImageFilename = imageFileName;
            Name = name;
            DatePurchased = datePurchased;
            Quantity = quantity;
            QuantityForShoppingList = quantityForShoppingList;
            Value = value;
            Location = location;
            Category = category;
            Notes = notes;
        }

        /// <summary>
        /// Constructs Item from JSON object.
        /// </summary>
        /// <param Name="job">The JSON representation of the Item</param>
        public Item(JObject job)
        {
            ID = (int)job["id"];
            Barcode = (string)job["barcode"];
            ImageFilename = (string)job["imageFileName"];
            Name = (string)job["name"];
            DatePurchased = (DateTime)job["datePurchased"];
            Quantity = (int)job["quantity"];
            QuantityForShoppingList = (int)job["quantityForShoppingList"];
            Value = (decimal)job["value"];
            Location = (string)job["location"];
            Category = (string)job["category"];
            Notes = (string)job["notes"];
        }

        /// <summary>
        /// Converts Item to JSON object.
        /// </summary>
        /// <returns>JObject representation of Item.</returns>
        public JObject ToJson()
        {
            JObject jObject = new JObject();

            jObject.Add("id", ID);
            jObject.Add("barcode", Barcode);
            jObject.Add("imageFileName", ImageFilename);
            jObject.Add("name", Name);
            jObject.Add("datePurchased", DatePurchased);
            jObject.Add("quantity", Quantity);
            jObject.Add("quantityForShoppingList", QuantityForShoppingList);
            jObject.Add("value", Value.ToString("0.##"));
            jObject.Add("location", Location);
            jObject.Add("category", Category);
            jObject.Add("notes", Notes);

            return jObject;
        }
    }
}
