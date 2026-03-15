using SV22T1020350.DataLayers.SQLServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SV22T1020350.Models.Catalog;
using SV22T1020350.Models.Common;

namespace SV22T1020350.Admin.Controllers
{
    public class TestController : Controller
    {
        private readonly IConfiguration _configuration;

        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> TestSupplier()
        {
            var repo = new SupplierRepository(
                _configuration.GetConnectionString("LiteCommerceDB")
            );

            var input = new PaginationSearchInput()
            {
                Page = 1,
                PageSize = 10
            };

            var result = await repo.ListAsync(input);

            return Json(result);
        }
    }
}
