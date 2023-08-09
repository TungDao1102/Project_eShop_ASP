using eShopSolution.ViewModel.Common;

namespace eShopSolution.ViewModel.Common.Manage
{
	public class GetProductPagingRequest : PagingRequestBase

	{
		public string Keyword { get; set; }

		public List<int> CategoryId { get; set; }
	}
}
