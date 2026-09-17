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

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<AdminOrderListDto>> GetAllOrdersAsync(AdminOrderFilterDto orderRequest)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(orderRequest);
            var orderDtos = _mapper.Map<IEnumerable<AdminOrderListDto>>(orders);

            return new PaginatedResponse<AdminOrderListDto>
            {
                Data = orderDtos.ToList(),
                PageNumber = orderRequest.PageNumber,
                Limit = orderRequest.Limit,
                TotalCount = orderDtos.Count(),
                TotalPages = (int)Math.Ceiling((double)orders.Count() / orderRequest.Limit)
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
                throw new ValidationException($"Order is already {updateRequest.Status}.");

            if (!IsValidTransition(order.Status, updateRequest.Status))
                throw new ValidationException($"Cannot change order status from {order.Status} to {updateRequest.Status}.");

            if (updateRequest.Status == OrderStatus.Delivered && order.Shipments.Any(s => s.Status != ShipmentStatus.Delivered))
                throw new ValidationException("Order cannot be marked as delivered until all shipments are delivered.");

            if (updateRequest.Status == OrderStatus.Shipped && order.Shipments.Any(s => s.Status == ShipmentStatus.Pending))
                throw new ValidationException("Order cannot be marked as shipped while it has pending shipments.");

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
