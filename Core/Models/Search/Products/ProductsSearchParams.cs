using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Search.Products;

public class ProductsSearchParams
{
    public PaginationRequestModel PaginationRequest { get; set; }
    public string? CategoryName { get; set; }
    public string? Value { get; set; }
}
