using eShopSolution.Application.Common;
using eShopSolution.Data.EF;
using eShopSolution.Data.Entities;
using eShopSolution.Utility;
using eShopSolution.ViewModel.Catalog.Product;
using eShopSolution.ViewModel.Catalog.Product.Manage;
using eShopSolution.ViewModel.Common;
using eShopSolution.ViewModel.Common.Manage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace eShopSolution.Application.Catalog.Products
{
    public class ManageProductService : IManageProductService
	{
		private readonly EShopDbContext _context;
		private readonly IStorageService _storageService;
		public ManageProductService(EShopDbContext context, IStorageService storageService)
		{
			_context = context;
			_storageService = storageService;
		}

		public Task<bool> AddImages(int productId, List<IFormFile> files)
		{
			throw new NotImplementedException();
		}

		public async Task AddViewCount(int productId)
		{
			var product = await _context.Products.FindAsync(productId);
			if (product is null)
			{
				throw new Exception("product bi null");
			}
			product.ViewCount++;
			await _context.SaveChangesAsync();
		}

		public async Task<int> Create(ProductCreateRequest request)
		{
			var product = new Product()
			{
				DateCreated = DateTime.Now,
				OriginalPrice = request.OriginalPrice,
				Price = request.Price,
				Stock = request.Stock,
				ViewCount = 0,
				ProductTranslations = new List<ProductTranslation>()
				{
					new ProductTranslation()
					{
						Name = request.Name,
						Description = request.Description,
						Details = request.Details,
						LanguageId = request.LanguageId,
						SeoAlias = request.SeoAlias,
						SeoDescription = request.SeoDescription,
						SeoTitle = request.SeoTitle
					}
				}
			};

			if (request.ThumbnailImage is not null)
			{
				product.ProductImages = new List<ProductImage>
				{
					new ProductImage()
					{
						Caption = "Thumbnail Images of " + request.Name,
						DateCreated = DateTime.Now,
						FileSize = request.ThumbnailImage.Length,
						ImagePath =await SaveFile(request.ThumbnailImage),
						IsDefault = true,
						SortOrder = 1
					}
				};
			}

			_context.Products.Add(product);
			return await _context.SaveChangesAsync();
		}

		public async Task<int> Delete(int productId)
		{
			var product = await _context.Products.FindAsync(productId);
			if (product == null) throw new EShopException($"Cannot find a product: {productId}");
			product.ProductImages = null; 
			// lam den video 13 doan exception phut 16
			var thumnailImage = await _context.ProductImages.Where(x => x.ProductId == productId).ToListAsync();
		
			foreach(var item in thumnailImage)
			{
				await _storageService.DeleteFileAsync(item.ImagePath);
				_context.ProductImages.Remove(item);
			}
			_context.Products.Remove(product);
			
			return await _context.SaveChangesAsync();
		}

		//public async Task<List<ProductViewModel>> GetAll()
		//{
		//    throw new NotImplementedException();
		//}

		//public Task<PagedResult<ProductViewModel>> GetAllPaging(string keyword, int pageIndex, int pageSize)
		//{
		//    throw new NotImplementedException();
		//}

		public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request)
		{
			var query = from p in _context.Products
						join pt in _context.ProductTranslations on p.Id equals pt.ProductId
						join pic in _context.ProductInCategories on p.Id equals pic.ProductId
						join c in _context.Categories on pic.CategoryId equals c.Id
						select new { p, pt, pic };
			if (!string.IsNullOrEmpty(request.Keyword))
			{
				query = query.Where(x => x.pt.Name.Contains(request.Keyword));
			}
			if (request.CategoryId.Count > 0)
			{
				query = query.Where(p => request.CategoryId.Contains(p.pic.CategoryId));
			}
			int totalRow = await query.CountAsync();
			var data = await query.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).Select(x => new ProductViewModel
			{
				Name = x.pt.Name,
				DateCreated = x.p.DateCreated,
				Description = x.pt.Description,
				Details = x.pt.Details,
				LanguageId = x.pt.LanguageId,
				OriginalPrice = x.p.OriginalPrice,
				Price = x.p.Price,
				SeoAlias = x.pt.SeoAlias,
				SeoDescription = x.pt.SeoDescription,
				SeoTitle = x.pt.SeoTitle,
				Stock = x.p.ViewCount
			}).ToListAsync();

			var pageResult = new PagedResult<ProductViewModel>()
			{
				TotalRecord = totalRow,
				Items = data
			};
			return pageResult;
		}

		public Task<List<ProductImageViewModel>> GetListImages(int productId)
		{
			throw new NotImplementedException();
		}

		public Task<bool> RemoveImage(int imageId)
		{
			throw new NotImplementedException();
		}

		public async Task<int> Update(ProductUpdateRequest request)
		{
			var product = await _context.Products.FindAsync(request.Id);
			var productTranslation = await _context.ProductTranslations.SingleOrDefaultAsync(x => x.ProductId == request.Id && x.LanguageId == request.LanguageId);
			if (product == null || productTranslation == null) { throw new EShopException($"Cannot find a product: {request.Id}"); }
			// do khong su dung auto mapper nen phai anh xa tung cai 1
			productTranslation.Name = request.Name;
			productTranslation.Description = request.Description;
			productTranslation.Details = request.Details;
			productTranslation.SeoDescription = request.SeoDescription;
			productTranslation.SeoTitle = request.SeoTitle;
			productTranslation.SeoAlias = request.SeoAlias;

			if (request.ThumbnailImage is not null)
			{
				var thumnailImage = await _context.ProductImages.SingleOrDefaultAsync(x => x.IsDefault == true && x.ProductId == request.Id);
				if (thumnailImage != null)
				{ 
					thumnailImage.FileSize = request.ThumbnailImage.Length;
					thumnailImage.ImagePath = await SaveFile(request.ThumbnailImage);
					_context.ProductImages.Update(thumnailImage);
				}
			}

			return await _context.SaveChangesAsync();
		}

		public Task<bool> UpdateImage(int imageId, string caption, bool isDefault)
		{
			throw new NotImplementedException();
		}

		public async Task<bool> UpdatePrice(int productId, decimal newPrice)
		{
			var product = await _context.Products.FindAsync(productId);
			if (product == null) { throw new EShopException($"Cannot find a product: {productId}"); }
			product.Price = newPrice;
			await _context.SaveChangesAsync();
			return true;

		}

		public async Task<bool> UpdateStock(int productId, int addedQuantity)
		{
			var product = await _context.Products.FindAsync(productId);
			if (product == null) { throw new EShopException($"Cannot find a product: {productId}"); }
			product.Stock += addedQuantity;
			await _context.SaveChangesAsync();
			return true;
		}

		private async Task<string> SaveFile(IFormFile file)
		{
			var originalFilename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
			var filename = $"{Guid.NewGuid()}{Path.GetExtension(originalFilename)}";
			await _storageService.SaveFileAsync(file.OpenReadStream(), filename);
			return filename;
		}
	}
}

