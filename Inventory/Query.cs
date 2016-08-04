using Inventory.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Inventory
{
    public class Query
    {
        private static string server = "http://192.168.1.133/inventory/";

        public async static Task<JArray> GetHouse(int houseId)
        {
            using (var client = new HttpClient())
            {
                JArray array = new JArray();

                var param = new Dictionary<string, string>
                {
                    {"hid", houseId.ToString()}
                };

                var content = new FormUrlEncodedContent(param);

                try
                {
                    var response = await client.PostAsync(string.Format("{0}getHouse.php", server), content);
                    response.EnsureSuccessStatusCode(); // throw if not a success code

                    var responseString = await response.Content.ReadAsStringAsync();

                    array = JArray.Parse(responseString);
                }
                catch (HttpRequestException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                return array;
            }
        }

        public async static Task<JArray> GetLocations(int houseId)
        {
            using (var client = new HttpClient())
            {
                JArray array = new JArray();

                var param = new Dictionary<string, string>
                {
                    {"hid", houseId.ToString()}
                };

                var content = new FormUrlEncodedContent(param);
                try
                {
                    var response = await client.PostAsync("http://192.168.1.133/inventory/getLocations.php", content);
                    response.EnsureSuccessStatusCode(); // throw if not a success code

                    var responseString = await response.Content.ReadAsStringAsync();

                    array = JArray.Parse(responseString);
                }
                catch (HttpRequestException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                return array;
            }
        }

        public async static Task<JArray> GetCategories(int houseId)
        {
            using (var client = new HttpClient())
            {
                JArray array = new JArray();

                var param = new Dictionary<string, string>
                {
                    {"hid", houseId.ToString()}
                };

                var content = new FormUrlEncodedContent(param);

                try
                {
                    var response = await client.PostAsync("http://192.168.1.133/inventory/getCategories.php", content);
                    response.EnsureSuccessStatusCode(); // throw if not a success code

                    var responseString = await response.Content.ReadAsStringAsync();

                    array = JArray.Parse(responseString);
                }
                catch (HttpRequestException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                return array;
            }
        }

        public async static Task<JArray> GetItems(int houseId)
        {
            using (var client = new HttpClient())
            {
                JArray array = new JArray();

                var param = new Dictionary<string, string>
                {
                    {"hid", houseId.ToString()}
                };

                var content = new FormUrlEncodedContent(param);

                try
                {
                    var response = await client.PostAsync(string.Format("{0}getItems.php", server), content);
                    response.EnsureSuccessStatusCode(); // throw if not a success code

                    var responseString = await response.Content.ReadAsStringAsync();

                    array = JArray.Parse(responseString);
                }
                catch (HttpRequestException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                return array;
            }
        }

        public async static Task<bool> AddItemAsProduct(JObject product)
        {
            string dateString = (string)product["datePurchased"];
            DateTime datePurchased;


            if (string.IsNullOrEmpty(dateString))
            {
                datePurchased = new DateTime(1, 1, 1);
            }
            else
            {
                datePurchased = DateTime.Parse(dateString);

                if (datePurchased.Year == 1)
                {
                    datePurchased = DateTime.Today;
                }
            }

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"hid", Core.HouseID.ToString()},
                    {"barcode", (string)product["barcode"]},
                    {"imageFileName", null},
                    {"name", (string)product["name"]},
                    {"datePurchased", datePurchased.ToString("yyyy-MM-dd")},
                    {"quantity", (string)product["quantity"]},
                    {"value", (string)product["value"]},
                    {"location", (string)product["location"]},
                    {"category", (string)product["category"]},
                    {"notes", (string)product["notes"]}
                };

                var content = new MyFormUrlEncodedContent(values);

                var response = await client.PostAsync("http://192.168.1.133/inventory/addItem.php", content);

                var responseString = await response.Content.ReadAsStringAsync();

                bool b = responseString == "1";

                return b;
            }
        }

        public async static Task<bool> Insert(Item item)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"barcode", item.Barcode},
                    {"imageFileName", item.ImageFilename},
                    {"name", item.Name},
                    {"datePurchased", item.DatePurchased.ToString("yyyy-MM-dd")},
                    {"quantity", item.Quantity.ToString()},
                    {"value", item.Value.ToString()},
                    {"location", item.Location},
                    {"category", item.Category},
                    {"notes", item.Notes}
                };
                
                var content = new MyFormUrlEncodedContent(values);

                var response = await client.PostAsync("http://192.168.1.133/inventory/addItem.php", content);

                var responseString = await response.Content.ReadAsStringAsync();

                bool b = responseString == "1";

                return b;
            }
        }

        public async static Task<bool> Update(Item item)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"id", item.ID.ToString()},
                    {"barcode", item.Barcode},
                    {"imageFileName", item.ImageFilename},
                    {"name", item.Name},
                    {"datePurchased", item.DatePurchased.ToString("yyyy-MM-dd")},
                    {"quantity", item.Quantity.ToString()},
                    {"value", item.Value.ToString()},
                    {"location", item.Location},
                    {"category", item.Category},
                    {"notes", item.Notes}
                };

                var content = new MyFormUrlEncodedContent(values);
                var response = await client.PostAsync("http://192.168.1.133/inventory/updateItem.php", content);

                var responseString = await response.Content.ReadAsStringAsync();
                
                bool b = responseString == "\\1";

                return b;
            }
        }

        public async static Task<bool> DeleteItem(int id)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"id", id.ToString()}
                };

                var content = new MyFormUrlEncodedContent(values);
                var response = await client.PostAsync("http://192.168.1.133/inventory/deleteItem.php", content);

                var responseString = await response.Content.ReadAsStringAsync();

                bool b = responseString == "1";

                return b;
            }
        }

        public async static Task UploadImage(string filename, byte[] imageData)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"filename", filename},
                    {"imageData", Convert.ToBase64String(imageData)}
                };

                var content = new MyFormUrlEncodedContent(values);

                var response = await client.PostAsync("http://192.168.1.133/inventory/uploadImage.php", content);

                var responseString = await response.Content.ReadAsStringAsync();
            }
        }

        public async static Task<byte[]> GetImage(string filename)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"filename", filename}
                };

                var content = new MyFormUrlEncodedContent(values);

                var response = await client.PostAsync("http://192.168.1.133/inventory/getImage.php", content);

                var bytes = await response.Content.ReadAsByteArrayAsync();

                return bytes;
            }
        }

        public async static Task<bool> DeleteImage(string filename)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    {"filename", filename}
                };

                var content = new MyFormUrlEncodedContent(values);

                var response = await client.PostAsync("http://192.168.1.133/inventory/deleteImage.php", content);

                var responseString = await response.Content.ReadAsStringAsync();

                bool b = responseString == "1";

                return b;
            }
        }

        /// <summary>
        /// Queries the SearchUPC database for a product that matches the Barcode.
        /// </summary>
        /// <param Name="barcode">The UPC code to be queried</param>
        /// <returns>A JArray of products</returns>
        public async static Task<JObject> GetProductJson(string barcode)
        {
            using (var client = new HttpClient())
            {
                JObject obj = new JObject();

                try
                {
                    string requestUri = string.Format("http://www.searchupc.com/handlers/upcsearch.ashx?request_type=3&access_token=B4EDA630-C081-496C-B70C-2E7A44CAFCBA&upc={0}", barcode);

                    var response = await client.GetAsync(requestUri);
                    response.EnsureSuccessStatusCode(); // throw if not a success code

                    var responseString = await response.Content.ReadAsStringAsync();

                    obj = JObject.Parse(responseString);
                }
                catch (HttpRequestException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }

                return obj;
            }
        }


        /// <summary>
        /// Custom class to replace the FormUrlEncodedContent
        /// via http://stackoverflow.com/a/23740338
        /// 
        /// FormUrlEncodedContent restricts the amount of data we can send to the database.
        /// It made uploading images impossible.
        /// This fixes it.
        /// </summary>
        public class MyFormUrlEncodedContent : ByteArrayContent
        {
            public MyFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
                : base(MyFormUrlEncodedContent.GetContentByteArray(nameValueCollection))
            {
                base.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }
            private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            {
                if (nameValueCollection == null)
                {
                    throw new ArgumentNullException("nameValueCollection");
                }
                StringBuilder stringBuilder = new StringBuilder();
                foreach (KeyValuePair<string, string> current in nameValueCollection)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('&');
                    }

                    stringBuilder.Append(MyFormUrlEncodedContent.Encode(current.Key));
                    stringBuilder.Append('=');
                    stringBuilder.Append(MyFormUrlEncodedContent.Encode(current.Value));
                }
                //return Encoding.Default.GetBytes(stringBuilder.ToString());
                return Encoding.UTF8.GetBytes(stringBuilder.ToString());
            }
            private static string Encode(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    return string.Empty;
                }
                return System.Net.WebUtility.UrlEncode(data).Replace("%20", "+");
            }
        }
    }
}
