using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Catalog.Product.Manage;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.Common.Manage;
using Microsoft.AspNetCore.Http;

namespace eShopSolution.Application.Catalog.Products
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);
        Task<int> Update(ProductUpdateRequest request);
        Task<int> Delete(int productId);
      //  Task<List<ProductViewModel>> GetAll();
        Task<PagedResult<ProductViewModel>> GetAllPaging(ViewModel.Common.Manage.GetProductPagingRequest request );
        Task<bool> UpdatePrice(int productId, decimal newPrice);
        Task AddViewCount(int productId);
        Task<bool> UpdateStock(int productId, int addedQuantity);

        Task<bool> AddImages(int productId, List<IFormFile> files);
        Task<bool> RemoveImage(int imageId);
        Task<bool> UpdateImage(int imageId, string caption, bool isDefault);

        Task<List<ProductImageViewModel>> GetListImages(int productId);
    }
}
