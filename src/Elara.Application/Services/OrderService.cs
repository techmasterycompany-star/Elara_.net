using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Order;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ISellerRepository _sellerRepository;

        public OrderService(IOrderRepository orderRepository, IMapper mapper, ISellerRepository sellerRepository)
        {
            _orderRepository = orderRepository;
            _sellerRepository = sellerRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<SellerOrderListDto>> GetSellerOrdersAsync(long userId, SellerOrderQuery query)
        {
            var sellerProfile = await _sellerRepository.GetByUserIdAsync(userId);

            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found.");

            var orders = await _orderRepository.GetSellerOrdersAsync(sellerProfile.Id, query);

            var orderDtos = orders.Items.Select(order =>
            {
                var items = order.Items.ToList();

                return new SellerOrderListDto
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    ShippingCity = order.ShippingCity,
                    ShippingCountry = order.ShippingCountry,
                    ItemCount = items.Count,
                    SellerSubtotal = items.Sum(i => i.Subtotal)
                };
            });

            return new PaginatedResponse<SellerOrderListDto>
            {
                Data = orderDtos,
                PageNumber = query.PageNumber,
                Limit = query.Limit,
                TotalCount = orders.TotalCount,
                TotalPages = (int)Math.Ceiling((double)orders.TotalCount / query.Limit)
            };
        }

        public async Task<SellerOrderDetailsDto> GetSellerOrderByIdAsync(long orderId, long userId)
        {
            var sellerProfile = await _sellerRepository.GetByUserIdAsync(userId);

            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found.");

            var order = await _orderRepository.GetSellerOrderDetailsAsync(orderId, sellerProfile.Id);

            if (order == null)
                throw new NotFoundException("Order not found.");

            var result = _mapper.Map<SellerOrderDetailsDto>(order);

            foreach (var item in result.Items)
            {
                var entity = order.Items.First(i => i.Id == item.OrderItemId);
                var shipped = entity.ShipmentItems.Sum(i => i.Quantity);

                item.QuantityShipped = shipped;
                item.QuantityRemaining = entity.Quantity - shipped;
            }

            return result;
        }

        public async Task<PaginatedResponse<AdminOrderListDto>> GetAllOrdersAsync(AdminOrderFilterDto orderRequest)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(orderRequest);
            var orderDtos = _mapper.Map<IEnumerable<AdminOrderListDto>>(orders.Items).ToList();

            return new PaginatedResponse<AdminOrderListDto>
            {
                Data = orderDtos,
                PageNumber = orderRequest.PageNumber,
                Limit = orderRequest.Limit,
                TotalCount = orders.TotalCount,
                TotalPages = (int)Math.Ceiling((double)orders.TotalCount / orderRequest.Limit)
            };
        }

        public async Task<AdminOrderDetailsDto> GetOrderByIdAsync(long orderId)
        {
            var order = await _orderRepository.GetOrderDetailsAsync(orderId);
            if (order == null) throw new NotFoundException("Order Not Found");

            return _mapper.Map<AdminOrderDetailsDto>(order);
        }

        public async Task UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequestDto updateRequest)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null) throw new NotFoundException("Order Not Found");

            // validate business rules
            if (order.Status == updateRequest.Status) 
                throw new ConflictException($"Order is already {updateRequest.Status}.");

            if (!IsValidTransition(order.Status, updateRequest.Status))
                throw new ConflictException($"Cannot change order status from {order.Status} to {updateRequest.Status}.");

            if (updateRequest.Status == OrderStatus.Delivered && order.Shipments.Any(s => s.Status != ShipmentStatus.Delivered))
                throw new ConflictException("Order cannot be marked as delivered until all shipments are delivered.");

            if (updateRequest.Status == OrderStatus.Shipped && order.Shipments.Any(s => s.Status == ShipmentStatus.Pending))
                throw new ConflictException("Order cannot be marked as shipped while it has pending shipments.");

            //Update status
            order.Status = updateRequest.Status;
            order.StatusHistory.Add(new OrderStatusHistory { 
                OrderId = orderId,
                Status = updateRequest.Status.ToString(), 
                CreatedAt = DateTime.UtcNow, 
                UpdatedAt = DateTime.UtcNow 
            });

            await _orderRepository.UpdateOrderAsync(order);
        }

        private static bool IsValidTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled,

                OrderStatus.Confirmed => newStatus is OrderStatus.Shipped or OrderStatus.Cancelled,

                OrderStatus.Shipped => newStatus is OrderStatus.Delivered,

                OrderStatus.Delivered => false,

                OrderStatus.Cancelled => false,

                _ => false
            };
        }
    }
}
