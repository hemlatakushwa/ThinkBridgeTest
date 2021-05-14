using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ThinkBridge.Models
{
    public class DBProducts : Product
    {
        DataAccessLayer dal = new DataAccessLayer();

        /// <summary>
        /// Function to get list of products
        /// </summary>
        public async Task<List<Product>> GetProducts(int page_no = 1, string search = "", string filter_by = "", int id = 0)
        {
            List<Product> objlist = new List<Product>();

            var limit = 30;
            var offset = limit * (page_no - 1);

            #region Search & filter conditions
            var condition = "";
            if (id != 0)
                condition = " and id=" + id;
            #endregion

            DataAccessLayer dal = new DataAccessLayer();

            var query = " select id, name, mrp, price, discount, description, images" +
                        " from products" +
                        " where active=true" +
                        " order by id desc" +
                        " limit " + limit + " offset " + offset;
            DataTable dt = dal.GetDataTable(query);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Product product = new Product();
                product.id = Convert.ToInt32(dt.Rows[i]["id"]);
                product.name = Convert.ToString(dt.Rows[i]["name"]);
                product.mrp = Convert.ToDecimal(dt.Rows[i]["mrp"]);
                product.price = Convert.ToDecimal(dt.Rows[i]["price"]);
                product.description = Convert.ToString(dt.Rows[i]["description"]);
                product.images = (int[])dt.Rows[i]["images"];
                objlist.Add(product);
            }

            return objlist;
        }

        /// <summary>
        /// Function to save documents
        /// </summary>
        private void SaveImage(HttpRequest request, int id)
        {
            var file_path = @"D:\Images\";

            if (request.Form.Files.Count > 0)
            {
                Bitmap bmpImage = new Bitmap(request.Form.Files[0].OpenReadStream());
                int source_height = bmpImage.Height;
                int source_width = bmpImage.Width;

                int width = source_width;
                int height = source_height;
                if (source_height > 1000 && source_height > source_width)
                {
                    height = 1000;
                    width = source_width * height / source_height;
                }
                else if (source_width > 1000)
                {
                    width = 1000;
                    height = source_height * width / source_width;
                }
                var new_image = new Bitmap(bmpImage, new Size(width, height));
                var file_name = id + "-.jpg";

                new_image.Save(file_path + file_name);
            }
        }

        /// <summary>
        /// Function to add a new product
        /// </summary>
        public async Task<dynamic> AddProduct(string name, decimal mrp, decimal price, decimal discount, string description, HttpRequest request)
        {
            var error = "";
            var success = true;

            #region Checking conditions
            if (name == "")
                error = "Name cannot be empty";
            else if (mrp == 0)
                error = "MRP cannot be 0";
            else if (price == 0)
                error = "Price cannot be 0";
            #endregion

            if (error == "")
            {
                var query = " insert into product (name, mrp, price, discount, description)" +
                            " values ('" + name + "','" + mrp + "','" + price + "','" + discount + "','" + description.Replace("'", "''") + "')";
                error = dal.ExecuteScaler(query);
            }
            else
                success = false;

            if (request.Form.Files.Count > 0)
                SaveImage(request, id);

            dynamic obj = new ExpandoObject();
            obj.error = error;
            obj.success = success;

            return await obj;
        }

        /// <summary>
        /// Function to update exist product
        /// </summary>
        public async Task<dynamic> UpdateProduct(int id, string name, decimal mrp, decimal price, decimal discount, string description, HttpRequest request)
        {
            var error = "";
            var success = true;

            #region Checking conditions
            if (id <= 0)
                error = "Product not selected";
            if (name == "")
                error = "Name cannot be empty";
            else if (mrp == 0)
                error = "MRP cannot be 0";
            else if (price == 0)
                error = "Price cannot be 0";
            #endregion

            #region Checking exist Product
            var query = " select count(*)" +
                        " from products" +
                        " where id=" + id;
            var exist_product = dal.ExecuteScaler(query);
            if (Convert.ToInt32(exist_product) == 0)
                error = "Product not found";
            #endregion


            if (error == "")
            {
                var update_query = " update products set name='" + name + "', mrp=" + mrp + ", price=" + price + ", discount=" + discount + ", description='" + description.Replace("'", "''") + "'" +
                                   " where id=" + id;
                dal.ExecuteScaler(update_query);
            }
            else
                success = false;

            if (request.Form.Files.Count > 0)
                SaveImage(request, id);

            dynamic obj = new ExpandoObject();
            obj.error = error;
            obj.success = success;

            return await obj;
        }

        /// <summary>
        /// Function to delete exist product
        /// </summary>
        public async Task<dynamic> DeleteProduct(int id)
        {
            var error = "";
            var success = true;

            #region Checking exist Product
            var query = " select count(*)" +
                        " from products" +
                        " where id=" + id;
            var exist_product = dal.ExecuteScaler(query);
            if (Convert.ToInt32(exist_product) == 0)
                error = "Product not found";
            #endregion

            if (error == "")
            {
                var update_query = " delete from products" +
                                   " where id=" + id;
                dal.ExecuteScaler(update_query);
            }
            else
                success = false;

            dynamic obj = new ExpandoObject();
            obj.error = error;
            obj.success = success;

            return await obj;
        }
    }
}