using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Shipment;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Entities;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Application.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ISellerRepository _sellerRepository;

        public ShipmentService(IShipmentRepository shipmentRepository, IOrderRepository orderRepository, IMapper mapper, ISellerRepository sellerRepository)
        {
            _shipmentRepository = shipmentRepository;
            _orderRepository = orderRepository;
            _sellerRepository = sellerRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<AdminShipmentListDto>> GetAllShipmentsAsync(AdminShipmentFilterDto shipmentRequest)
        {
            var shipments = await _shipmentRepository.GetAllShipmentsAsync(shipmentRequest);
            var shipmentDtos = _mapper.Map<IEnumerable<AdminShipmentListDto>>(shipments.Items).ToList();

            return new PaginatedResponse<AdminShipmentListDto>
            {
                Data = shipmentDtos,
                TotalCount = shipments.TotalCount,
                PageNumber = shipmentRequest.PageNumber,
                Limit = shipmentRequest.Limit,
                TotalPages = (int)Math.Ceiling((double)shipments.TotalCount / shipmentRequest.Limit)
            };
        }

        public async Task<AdminShipmentDetailsDto> GetShipmentByIdAsync(long shipmentId)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(shipmentId);

            if (shipment == null)
                throw new NotFoundException("Shipment not found.");

            return _mapper.Map<AdminShipmentDetailsDto>(shipment);
        }
      
        public async Task CreateShipmentsForOrderAsync(long orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Cannot create shipments for a cancelled order.");

            var itemsBySeller = order.Items
                .Where(item => item.Product != null)
                .GroupBy(item => item.Product.SellerProfileId);

            foreach (var sellerItems in itemsBySeller)
            {
                if (order.Shipments.Any(shipment =>
                    !shipment.IsDeleted && shipment.SellerProfileId == sellerItems.Key))
                {
                    continue;
                }

                var shipment = new Shipment
                {
                    OrderId = order.Id,
                    SellerProfileId = sellerItems.Key,
                    Carrier = "Pending assignment",
                    TrackingNumber = "Pending assignment",
                    Status = ShipmentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Items = sellerItems.Select(item => new ShipmentItem
                    {
                        OrderItemId = item.Id,
                        Quantity = item.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }).ToList()
                };

                await _shipmentRepository.AddAsync(shipment);
            }

            await _shipmentRepository.SaveChangesAsync();
        }

        public async Task UpdateShipmentAsync(long shipmentId, AdminUpdateShipmentRequestDto request)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(shipmentId);

            if (shipment == null)
                throw new NotFoundException("Shipment not found.");

            if (request.Carrier != null)
                shipment.Carrier = request.Carrier;

            if (request.TrackingNumber != null)
                shipment.TrackingNumber = request.TrackingNumber;

            if (request.EstimatedDeliveryDate.HasValue)
                shipment.EstimatedDeliveryDate = request.EstimatedDeliveryDate;

            shipment.UpdatedAt = DateTime.UtcNow;

            await _shipmentRepository.UpdateShipmentAsync(shipment);
        }

        public async Task UpdateShipmentStatusAsync(long shipmentId, UpdateShipmentStatusRequestDto updateRequest)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(shipmentId);

            if (shipment == null)
                throw new NotFoundException("Shipment not found.");

            // validate business rules
            if (shipment.Status == updateRequest.Status)
                throw new ConflictException($"Shipment is already {updateRequest.Status}.");
            if (!IsValidTransition(shipment.Status, updateRequest.Status))
                throw new ConflictException($"Cannot change shipment status from {shipment.Status} to {updateRequest.Status}.");
            await ChangeShipmentStatusAsync(shipment, updateRequest.Status);
            await _shipmentRepository.UpdateShipmentAsync(shipment);
            await UpdateOrderStatusBasedOnShipmentsAsync(shipment.OrderId);
        }

        private async Task ChangeShipmentStatusAsync(Shipment shipment, ShipmentStatus newStatus)
        {
            if (shipment.Status == newStatus)
                throw new ConflictException($"Shipment is already {newStatus}.");

            if (!IsValidTransition(shipment.Status, newStatus))
                throw new ConflictException($"Cannot change shipment status from {shipment.Status} to {newStatus}.");

            var now = DateTime.UtcNow;

            shipment.Status = newStatus;

            if (newStatus == ShipmentStatus.Shipped)
                shipment.ShippedDate ??= now;

            if (newStatus == ShipmentStatus.Delivered)
                shipment.DeliveredDate ??= now;

            shipment.UpdatedAt = now;
        }

        private static bool IsValidTransition(ShipmentStatus currentStatus, ShipmentStatus newStatus)
        {
            return currentStatus switch
            {
                ShipmentStatus.Pending => newStatus == ShipmentStatus.Shipped,
                ShipmentStatus.Shipped => newStatus is ShipmentStatus.InTransit or ShipmentStatus.Delivered,
                ShipmentStatus.InTransit => newStatus is ShipmentStatus.Delivered or ShipmentStatus.Returned,
                ShipmentStatus.Delivered => false,
                ShipmentStatus.Returned => false,
                _ => false
            };
        }

        private async Task UpdateOrderStatusBasedOnShipmentsAsync(long orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                return;

            var shipments = await _shipmentRepository.GetShipmentsByOrderIdAsync(orderId);

            if (shipments.Count == 0)
                return;

            var activeShipments = shipments
                .Where(s => !s.IsDeleted && s.Status != ShipmentStatus.Returned)
                .ToList();

            if (activeShipments.Count == 0)
                return;

            OrderStatus? newStatus = null;

            if (activeShipments.All(s => s.Status == ShipmentStatus.Delivered))
            {
                newStatus = OrderStatus.Delivered;
            }
            else if (order.Status == OrderStatus.Confirmed &&
                     activeShipments.All(s => s.Status is ShipmentStatus.Shipped or ShipmentStatus.InTransit or ShipmentStatus.Delivered))
            {
                newStatus = OrderStatus.Shipped;
            }

            if (newStatus.HasValue && newStatus.Value != order.Status)
            {
                order.Status = newStatus.Value;

                var now = DateTime.UtcNow;

                order.StatusHistory.Add(new OrderStatusHistory
                {
                    OrderId = orderId,
                    Status = newStatus.Value.ToString(),
                    Notes = "Order status updated automatically based on shipment statuses.",
                    CreatedAt = now,
                    UpdatedAt = now
                });

                order.UpdatedAt = now;
            }

            if (order.Status == OrderStatus.Delivered &&
                order.Payment != null &&
                order.Payment.Method == PaymentMethodType.CashOnDelivery &&
                order.Payment.Status == PaymentStatus.Pending)
            {
                order.Payment.Status = PaymentStatus.Completed;
                order.Payment.PaidAt = DateTime.UtcNow;
                order.Payment.UpdatedAt = DateTime.UtcNow;
            }

            await _orderRepository.UpdateOrderAsync(order);
        }

        private static OrderStatus GetOrderStatusFromShipments(OrderStatus currentStatus, List<Shipment> shipments)
        {
            var activeShipments = shipments.Where(s => s.Status != ShipmentStatus.Returned).ToList();

            if (activeShipments.Count == 0)
                return currentStatus;

            if (activeShipments.All(s => s.Status == ShipmentStatus.Delivered))
                return OrderStatus.Delivered;

            if (currentStatus == OrderStatus.Confirmed && activeShipments.All(s => s.Status is ShipmentStatus.Shipped or ShipmentStatus.InTransit or ShipmentStatus.Delivered))
                return OrderStatus.Shipped;

            return currentStatus;
        }

        // Seller-specific methods
        public async Task<PaginatedResponse<SellerShipmentListDto>> GetSellerShipmentsAsync(long userId, SellerShipmentQuery query)
        {
            var sellerProfile = await _sellerRepository.GetByUserIdAsync(userId);

            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found.");

            var shipments = await _shipmentRepository.GetSellerShipmentsAsync(sellerProfile.Id, query);
            var shipmentDtos = _mapper.Map<IEnumerable<SellerShipmentListDto>>(shipments.Items);

            return new PaginatedResponse<SellerShipmentListDto>
            {
                Data = shipmentDtos,
                PageNumber = query.PageNumber,
                Limit = query.Limit,
                TotalCount = shipments.TotalCount,
                TotalPages = (int)Math.Ceiling((double)shipments.TotalCount / query.Limit)
            };
        }
        public async Task<SellerShipmentDetailsDto> GetSellerShipmentByIdAsync(long shipmentId, long userId)
        {
            var sellerProfile = await _sellerRepository.GetByUserIdAsync(userId);

            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found.");

            var shipment = await _shipmentRepository.GetSellerShipmentByIdAsync(shipmentId, sellerProfile.Id);

            if (shipment == null)
                throw new NotFoundException("Shipment not found.");

            return _mapper.Map<SellerShipmentDetailsDto>(shipment);
        }
        public async Task ShipSellerShipmentAsync(long shipmentId, long userId, ShipSellerShipmentDto dto)
        {
            var sellerProfile = await _sellerRepository.GetByUserIdAsync(userId);

            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found.");

            var shipment = await _shipmentRepository.GetSellerShipmentByIdAsync(shipmentId, sellerProfile.Id);

            if (shipment == null)
                throw new NotFoundException("Shipment not found.");

            if (shipment.Status != ShipmentStatus.Pending)
                throw new ConflictException($"Shipment cannot be shipped when its status is {shipment.Status}.");

            var order = await _orderRepository.GetOrderByIdAsync(shipment.OrderId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new ConflictException("Cannot ship a cancelled order.");

            if (order.Status != OrderStatus.Confirmed)
                throw new ConflictException("Shipment can only be shipped after the order is confirmed.");

            shipment.Carrier = dto.Carrier;
            shipment.TrackingNumber = dto.TrackingNumber;
            shipment.EstimatedDeliveryDate = dto.EstimatedDeliveryDate;

            await ChangeShipmentStatusAsync(shipment, ShipmentStatus.Shipped);

            await _shipmentRepository.UpdateShipmentAsync(shipment);

            await UpdateOrderStatusBasedOnShipmentsAsync(shipment.OrderId);
        }
        // Customer-specific methods
        public async Task<IEnumerable<CustomerShipmentListDto>> GetCustomerShipmentsByOrderIdAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetCustomerOrderDetailsAsync(orderId, userId);

            if (order == null)
                throw new NotFoundException("Order not found.");

            var shipments = await _shipmentRepository.GetCustomerShipmentsByOrderIdAsync(orderId, userId);

            return _mapper.Map<IEnumerable<CustomerShipmentListDto>>(shipments);
        }
        public async Task<CustomerShipmentDetailsDto> GetCustomerShipmentByIdAsync(long orderId, long shipmentId, long userId)
        {
            var shipment = await _shipmentRepository.GetCustomerShipmentByIdAsync(orderId, shipmentId, userId);

            if (shipment == null)
                throw new NotFoundException("Shipment not found.");

            return _mapper.Map<CustomerShipmentDetailsDto>(shipment);
        }
    }
}
