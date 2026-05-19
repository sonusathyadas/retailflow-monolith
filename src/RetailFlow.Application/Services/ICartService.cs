using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Services
{
    public interface ICartService
    {
        CartDto GetCart(int userId);
        CartDto AddItem(int userId, AddCartItemRequest request);
        CartDto RemoveItem(int userId, string productId);
        void ClearCart(int userId);
    }
}
