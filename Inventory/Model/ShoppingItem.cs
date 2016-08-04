using System;

using PropertyChanged;
using SQLite.Net.Attributes;

namespace Inventory.Model
{
    [ImplementPropertyChanged]
    public class ShoppingItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        
        public int ItemID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public bool MakeItem { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedSince { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ShoppingItem()
        {
            ID = 0;
            ItemID = 0;
            Name = string.Empty;
            Quantity = 1;
            Notes = string.Empty;
            MakeItem = false;
            IsCompleted = false;
            CompletedSince = new DateTime(1, 1, 1);
        }

        /// <summary>
        /// Constructor for unknown shopping item id (inserting).
        /// </summary>
        public ShoppingItem(int itemId, string name, int quantity, string notes, bool makeItem, bool isCompleted, DateTime completedSince)
        {
            ItemID = itemId;
            Name = name;
            Quantity = quantity;
            Notes = notes;
            MakeItem = makeItem;
            IsCompleted = IsCompleted;
            CompletedSince = completedSince;
        }

        /// <summary>
        /// Constructor for known shopping item id (updating).
        /// </summary>
        public ShoppingItem(int id, int itemId, string name, int quantity, string notes, bool makeItem, bool isCompleted, DateTime completedSince)
        {
            ID = id;
            ItemID = itemId;
            Name = name;
            Quantity = quantity;
            Notes = notes;
            MakeItem = makeItem;
            IsCompleted = isCompleted;
            CompletedSince = completedSince;
        }

        /// <summary>
        /// Human-readable formatted string of shopping item
        /// </summary>
        /// <returns>
        /// A formatted string representing the shopping item
        /// </returns>
        public override string ToString()
        {
            return string.Format("Shopping Item with\n" +
                "ID: {0}\n" +
                "Item ID: {1}\n" +
                "Name: {2}\n" +
                "Quantity: {3}\n" +
                "Notes: {4}\n" +
                "MakeItem: {5}\n" +
                "IsCompleted: {6}\n" +
                "CompletedSince: {7}", 
                ID, ItemID, Name, Quantity, Notes, MakeItem, IsCompleted, CompletedSince.ToString("yyyy-MM-dd"));
        }

        /// <summary>
        /// Compares completed time against current time.
        /// If greater than 1 minute, shopping item is flagged
        /// for automatic deletion.
        /// </summary>
        /// <returns></returns>
        public bool ShouldRemove()
        {
            //System.Diagnostics.Debug.WriteLine("Checking if should remove " + Name);

            if (IsCompleted)
            {
                //System.Diagnostics.Debug.WriteLine("Completed...");

                //System.Diagnostics.Debug.WriteLine("Now: " + DateTime.UtcNow);
                //System.Diagnostics.Debug.WriteLine("then: " + CompletedSince);

                double diff = (DateTime.UtcNow - CompletedSince).TotalMinutes;

                //System.Diagnostics.Debug.WriteLine("Diff is: " + diff + " minutes.");

                if (diff >= 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
