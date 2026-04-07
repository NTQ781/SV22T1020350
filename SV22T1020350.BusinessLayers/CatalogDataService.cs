using SV22T1020350.DataLayers.Interfaces;
using SV22T1020350.DataLayers.SQLServer;
using SV22T1020350.Models.Catalog;
using SV22T1020350.Models.Common;

namespace SV22T1020350.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng xử lý dữ liệu liên quan đến danh mục hàng hóa của hệ thống, 
    /// bao gồm: mặt hàng (Product), thuộc tính của mặt hàng (ProductAttribute) và ảnh của mặt hàng (ProductPhoto).
    /// </summary>
    public static class CatalogDataService
    {
        private static readonly IProductRepository productDB;
        private static readonly IGenericRepository<Category> categoryDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static CatalogDataService()
        {
            categoryDB = new CategoryRepository(Configuration.ConnectionString);
            productDB = new ProductRepository(Configuration.ConnectionString);
        }

        #region Category

        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Category>> ListCategoriesAsync(PaginationSearchInput input)
        {
            return await categoryDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một loại hàng dựa vào mã loại hàng.
        /// </summary>
        public static async Task<Category?> GetCategoryAsync(int CategoryID)
        {
            return await categoryDB.GetAsync(CategoryID);
        }

        /// <summary>
        /// Bổ sung một loại hàng mới vào hệ thống.
        /// </summary>
        public static async Task<int> AddCategoryAsync(Category data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                throw new Exception("Tên loại hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.Description)) data.Description = "";

            data.CategoryName = data.CategoryName.Trim();
            data.Description = data.Description.Trim();

            return await categoryDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một loại hàng.
        /// </summary>
        public static async Task<bool> UpdateCategoryAsync(Category data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.CategoryName))
                throw new Exception("Tên loại hàng không được để trống");

            if (string.IsNullOrWhiteSpace(data.Description)) data.Description = "";

            data.CategoryName = data.CategoryName.Trim();
            data.Description = data.Description.Trim();

            return await categoryDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một loại hàng dựa vào mã loại hàng.
        /// </summary>
        public static async Task<bool> DeleteCategoryAsync(int CategoryID)
        {
            if (await categoryDB.IsUsedAsync(CategoryID))
                return false;

            return await categoryDB.DeleteAsync(CategoryID);
        }

        /// <summary>
        /// Kiểm tra xem một loại hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static async Task<bool> IsUsedCategoryAsync(int CategoryID)
        {
            return await categoryDB.IsUsedAsync(CategoryID);
        }

        #endregion

        #region Product

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang.
        /// </summary>
        public static async Task<PagedResult<Product>> ListProductsAsync(ProductSearchInput input)
        {
            return await productDB.ListAsync(input);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một mặt hàng.
        /// </summary>
        public static async Task<Product?> GetProductAsync(int productID)
        {
            return await productDB.GetAsync(productID);
        }

        /// <summary>
        /// Bổ sung một mặt hàng mới vào hệ thống.
        /// </summary>
        public static async Task<int> AddProductAsync(Product data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.ProductName))
                throw new Exception("Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                throw new Exception("Đơn vị tính không được để trống");
            if (data.Price < 0)
                throw new Exception("Giá hàng không được nhỏ hơn 0");

            if (string.IsNullOrWhiteSpace(data.ProductDescription)) data.ProductDescription = "";
            if (string.IsNullOrWhiteSpace(data.Photo)) data.Photo = "";

            data.ProductName = data.ProductName.Trim();
            data.Unit = data.Unit.Trim();
            data.ProductDescription = data.ProductDescription.Trim();

            return await productDB.AddAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một mặt hàng.
        /// </summary>
        public static async Task<bool> UpdateProductAsync(Product data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.ProductName))
                throw new Exception("Tên mặt hàng không được để trống");
            if (string.IsNullOrWhiteSpace(data.Unit))
                throw new Exception("Đơn vị tính không được để trống");
            if (data.Price < 0)
                throw new Exception("Giá hàng không được nhỏ hơn 0");

            if (string.IsNullOrWhiteSpace(data.ProductDescription)) data.ProductDescription = "";
            if (string.IsNullOrWhiteSpace(data.Photo)) data.Photo = "";

            data.ProductName = data.ProductName.Trim();
            data.Unit = data.Unit.Trim();
            data.ProductDescription = data.ProductDescription.Trim();

            return await productDB.UpdateAsync(data);
        }

        /// <summary>
        /// Xóa một mặt hàng dựa vào mã mặt hàng.
        /// </summary>
        public static async Task<bool> DeleteProductAsync(int productID)
        {
            if (await productDB.IsUsedAsync(productID))
                return false;

            return await productDB.DeleteAsync(productID);
        }

        /// <summary>
        /// Kiểm tra xem một mặt hàng có đang được sử dụng trong dữ liệu hay không.
        /// </summary>
        public static async Task<bool> IsUsedProductAsync(int productID)
        {
            return await productDB.IsUsedAsync(productID);
        }

        #endregion

        #region ProductAttribute

        /// <summary>
        /// Lấy danh sách các thuộc tính của một mặt hàng.
        /// </summary>
        public static async Task<List<ProductAttribute>> ListAttributesAsync(int productID)
        {
            return await productDB.ListAttributesAsync(productID);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một thuộc tính của mặt hàng.
        /// </summary>
        public static async Task<ProductAttribute?> GetAttributeAsync(long attributeID)
        {
            return await productDB.GetAttributeAsync(attributeID);
        }

        /// <summary>
        /// Bổ sung một thuộc tính mới cho mặt hàng.
        /// </summary>
        public static async Task<long> AddAttributeAsync(ProductAttribute data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                throw new Exception("Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                throw new Exception("Giá trị thuộc tính không được để trống");

            data.AttributeName = data.AttributeName.Trim();
            data.AttributeValue = data.AttributeValue.Trim();

            return await productDB.AddAttributeAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một thuộc tính mặt hàng.
        /// </summary>
        public static async Task<bool> UpdateAttributeAsync(ProductAttribute data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                throw new Exception("Tên thuộc tính không được để trống");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                throw new Exception("Giá trị thuộc tính không được để trống");

            data.AttributeName = data.AttributeName.Trim();
            data.AttributeValue = data.AttributeValue.Trim();

            return await productDB.UpdateAttributeAsync(data);
        }

        /// <summary>
        /// Xóa một thuộc tính của mặt hàng.
        /// </summary>
        public static async Task<bool> DeleteAttributeAsync(long attributeID)
        {
            return await productDB.DeleteAttributeAsync(attributeID);
        }

        #endregion

        #region ProductPhoto

        /// <summary>
        /// Lấy danh sách ảnh của một mặt hàng.
        /// </summary>
        public static async Task<List<ProductPhoto>> ListPhotosAsync(int productID)
        {
            return await productDB.ListPhotosAsync(productID);
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một ảnh của mặt hàng.
        /// </summary>
        public static async Task<ProductPhoto?> GetPhotoAsync(long photoID)
        {
            return await productDB.GetPhotoAsync(photoID);
        }

        /// <summary>
        /// Bổ sung một ảnh mới cho mặt hàng.
        /// </summary>
        public static async Task<long> AddPhotoAsync(ProductPhoto data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.Photo))
                throw new Exception("Đường dẫn ảnh không được để trống");

            if (string.IsNullOrWhiteSpace(data.Description)) data.Description = "";

            data.Photo = data.Photo.Trim();
            data.Description = data.Description.Trim();

            return await productDB.AddPhotoAsync(data);
        }

        /// <summary>
        /// Cập nhật thông tin của một ảnh mặt hàng.
        /// </summary>
        public static async Task<bool> UpdatePhotoAsync(ProductPhoto data)
        {
            if (data == null)
                throw new Exception("Dữ liệu không hợp lệ");
            if (string.IsNullOrWhiteSpace(data.Photo))
                throw new Exception("Đường dẫn ảnh không được để trống");

            if (string.IsNullOrWhiteSpace(data.Description)) data.Description = "";

            data.Photo = data.Photo.Trim();
            data.Description = data.Description.Trim();

            return await productDB.UpdatePhotoAsync(data);
        }

        /// <summary>
        /// Xóa một ảnh của mặt hàng.
        /// </summary>
        public static async Task<bool> DeletePhotoAsync(long photoID)
        {
            return await productDB.DeletePhotoAsync(photoID);
        }

        #endregion
    }
}