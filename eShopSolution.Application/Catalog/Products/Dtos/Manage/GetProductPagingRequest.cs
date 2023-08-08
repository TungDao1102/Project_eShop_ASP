using eShopSolution.Application.Dtos;

namespace eShopSolution.Application.Catalog.Products.Dtos.Manage
{
	public class GetProductPagingRequest : PagingRequestBase

	{
		public string Keyword { get; set; }

		public List<int> CategoryId { get; set; }
	}
}
