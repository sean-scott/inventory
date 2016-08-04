using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PropertyChanged;
using System;
using System.Threading.Tasks;

namespace Inventory.Model
{
    [ImplementPropertyChanged]
    public class ProductDefaults
    {
        public static string Filename = "product_defaults.json";
        public static string Path { get; set; }

        public bool AutoAdd { get; set; }
        public bool HasDate { get; set; }
        public DateTime DatePurchased { get; set; }
        public int Quantity { get; set; }
        
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ShoppingQuantity { get; set; }

        public string Location { get; set; }
        public string Category { get; set; }
        public string Notes { get; set; }

        public static ProductDefaults Read()
        {
            string json = Core.GetProductDefaults();

            if (string.IsNullOrEmpty(json))
            {
                return new ProductDefaults();
            }
            else
            {
                return new ProductDefaults(JObject.Parse(json));
            }
        }

        public static void Write(string json)
        {
            Core.WriteProductDefaults(json);
        }

        public ProductDefaults()
        {
            AutoAdd = false;
            HasDate = true;
            DatePurchased = new DateTime(1,1,1);
            Quantity = 1;
            ShoppingQuantity = -1;
            Location = string.Empty;
            Category = string.Empty;
            Notes = string.Empty;
        }

        public ProductDefaults(JObject defaults)
        {
            AutoAdd = (bool)defaults["autoAdd"];
            HasDate = (bool)defaults["hasDate"];
            DatePurchased = (DateTime)defaults["datePurchased"];
            Quantity = (int)defaults["quantity"];
            ShoppingQuantity = (int)defaults["shoppingQuantity"];
            Location = (string)defaults["location"];
            Category = (string)defaults["category"];
            Notes = (string)defaults["notes"];
        }

        /// <summary>
        /// Converts ProductDefaults to JSON object.
        /// </summary>
        /// <returns>JSON representation of ProductDefaults</returns>
        public JObject ToJson()
        {
            JObject defaults = new JObject();

            defaults.Add("autoAdd", AutoAdd);
            defaults.Add("hasDate", HasDate);
            defaults.Add("datePurchased", DatePurchased);
            defaults.Add("quantity", Quantity);
            defaults.Add("shoppingQuantity", ShoppingQuantity);
            defaults.Add("location", Location);
            defaults.Add("category", Category);
            defaults.Add("notes", Notes);

            return defaults;
        }
    }
}
