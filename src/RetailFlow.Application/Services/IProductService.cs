using System.Collections.Generic;
using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Services
{
    public interface IProductService
    {
        ProductDto GetById(string id);
        IEnumerable<ProductDto> Search(ProductSearchRequest request);
        ProductDto Create(CreateProductRequest request);
        ProductDto Update(string id, UpdateProductRequest request);
        void Delete(string id);
    }
}
