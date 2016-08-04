using Newtonsoft.Json.Linq;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Model
{
    [ImplementPropertyChanged]
    public class Filters
    {
        public static string Filename = "filters.json";
        public static string Path { get; set; }

        public const int ATTRIBUTE_CATEGORIES = 0;
        public const int ATTRIBUTE_LOCATIONS = 1;

        public const int ORDER_ASC = 0;
        public const int ORDER_DESC = 1;

        public const int SORT_ALPHABETICALLY = 0;
        public const int SORT_DATE_MODIFIED = 1;
        public const int SORT_DATE_PURCHASED = 2;
        public const int SORT_RECENTLY_ADDED = 3;
        public const int SORT_VALUE = 4;

        public int Attribute { get; set; }
        public int Order { get; set; }
        public int Sort { get; set; }

        public static Filters Read()
        {
            string json;

            try
            {
                json = Core.GetFilters();
            }
            catch (Exception)
            {
                json = null;
            }

            if (string.IsNullOrEmpty(json))
            {
                return new Filters();
            }
            else
            {
                return new Filters(JObject.Parse(json));
            }
        }

        public static void Write(string json)
        {
            Core.WriteFilters(json);
        }

        public Filters()
        {
            Attribute = ATTRIBUTE_LOCATIONS;
            Order = ORDER_DESC;
            Sort = SORT_RECENTLY_ADDED;
        }

        public Filters(int attribute, int order, int sort)
        {
            Attribute = attribute;
            Order = order;
            Sort = sort;
        }

        public Filters(JObject filters)
        {
            Attribute = (int)filters["attribute"];
            Order = (int)filters["order"];
            Sort = (int)filters["sort"];
        }

        public JObject ToJson()
        {
            JObject filters = new JObject();

            filters.Add("attribute", Attribute);
            filters.Add("order", Order);
            filters.Add("sort", Sort);

            return filters;
        }
    }
}
