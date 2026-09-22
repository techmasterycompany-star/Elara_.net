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
            }).ToList();

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
            var orderItems = order.Items.ToDictionary(i => i.Id);

            foreach (var item in result.Items)
            {
                if (!orderItems.TryGetValue(item.OrderItemId, out var entity))
                    continue;

                var shipped = entity.ShipmentItems
                    .Where(i => !i.Shipment.IsDeleted && i.Shipment.Status != ShipmentStatus.Returned)
                    .Sum(i => i.Quantity);

                item.QuantityShipped = shipped;
                item.QuantityRemaining = Math.Max(0, entity.Quantity - shipped);
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

            if (order == null)
                throw new NotFoundException("Order not found.");

            return _mapper.Map<AdminOrderDetailsDto>(order);
        }

        public async Task UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequestDto updateRequest)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status == updateRequest.Status)
                throw new ConflictException($"Order is already {updateRequest.Status}.");

            if (updateRequest.Status is OrderStatus.Shipped or OrderStatus.Delivered)
                throw new ConflictException("Shipped and Delivered statuses are updated automatically based on shipment statuses.");

            if (!IsValidManualTransition(order.Status, updateRequest.Status))
                throw new ConflictException($"Cannot change order status from {order.Status} to {updateRequest.Status}.");

            order.Status = updateRequest.Status;

            var now = DateTime.UtcNow;

            order.StatusHistory.Add(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = updateRequest.Status.ToString(),
                Notes = string.IsNullOrWhiteSpace(updateRequest.Notes) ? "No notes provided." : updateRequest.Notes,
                CreatedAt = now,
                UpdatedAt = now
            });

            await _orderRepository.UpdateOrderAsync(order);
        }

        private static bool IsValidManualTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled,
                OrderStatus.Confirmed => newStatus == OrderStatus.Cancelled,
                OrderStatus.Shipped => false,
                OrderStatus.Delivered => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }

        public async Task<PaginatedResponse<CustomerOrderListDto>> GetCustomerOrdersAsync(long userId, GetMyOrdersRequest request)
        {
            var orders = await _orderRepository.GetCustomerOrdersAsync(userId, request);
            var orderDtos = _mapper.Map<IEnumerable<CustomerOrderListDto>>(orders.Items).ToList();

            return new PaginatedResponse<CustomerOrderListDto>
            {
                Data = orderDtos,
                PageNumber = request.PageNumber,
                Limit = request.Limit,
                TotalCount = orders.TotalCount,
                TotalPages = (int)Math.Ceiling((double)orders.TotalCount / request.Limit)
            };
        }

        public async Task<CustomerOrderDetailsDto> GetCustomerOrderByIdAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetCustomerOrderDetailsAsync(orderId, userId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            return _mapper.Map<CustomerOrderDetailsDto>(order);
        }

        public async Task CancelCustomerOrderAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetCustomerOrderForUpdateAsync(orderId, userId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Order is already cancelled.");

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                throw new ConflictException($"Order cannot be cancelled when its status is {order.Status}.");

            order.Status = OrderStatus.Cancelled;

            var now = DateTime.UtcNow;

            order.StatusHistory.Add(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Cancelled.ToString(),
                Notes = "Order cancelled by customer.",
                CreatedAt = now,
                UpdatedAt = now
            });

            await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task<IEnumerable<CustomerOrderStatusHistoryDto>> GetOrderStatusHistoryAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetCustomerOrderDetailsAsync(orderId, userId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            return _mapper.Map<IEnumerable<CustomerOrderStatusHistoryDto>>(order.StatusHistory.OrderBy(h => h.CreatedAt));
        }
    }
}
