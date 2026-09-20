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
            if (shipment == null) throw new NotFoundException("Shipment Not Found");

            return _mapper.Map<AdminShipmentDetailsDto>(shipment);
        }

        public async Task UpdateShipmentAsync(long shipmentId, AdminUpdateShipmentRequestDto request)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(shipmentId);
            if (shipment == null) throw new NotFoundException("Shipment Not Found");
            // Update the shipment details
            if (request.Carrier != null)
                shipment.Carrier = request.Carrier;

            if (request.TrackingNumber != null)
                shipment.TrackingNumber = request.TrackingNumber;

            if (request.EstimatedDeliveryDate.HasValue)
                shipment.EstimatedDeliveryDate = request.EstimatedDeliveryDate;

            await _shipmentRepository.UpdateShipmentAsync(shipment);
        }

        public async Task UpdateShipmentStatusAsync(long shipmentId, UpdateShipmentStatusRequestDto updateRequest)
        {
            var shipment = await _shipmentRepository.GetShipmentByIdAsync(shipmentId);
            if (shipment == null) throw new NotFoundException("Shipment Not Found");

            // validate business rules
            if (shipment.Status == updateRequest.Status) 
                throw new ConflictException($"Shipment is already {updateRequest.Status}.");
            if (!IsValidTransition(shipment.Status, updateRequest.Status))
                throw new ConflictException($"Cannot change shipment status from {shipment.Status} to {updateRequest.Status}.");

            shipment.Status = updateRequest.Status;

            // Update dates based on status
            var now = DateTime.UtcNow;

            if (updateRequest.Status == ShipmentStatus.Delivered)
                shipment.DeliveredDate ??= now;
            else if (updateRequest.Status == ShipmentStatus.Shipped)
                shipment.ShippedDate ??= now;

            await _shipmentRepository.UpdateShipmentAsync(shipment);
            await UpdateOrderStatusBasedOnShipmentsAsync(shipment.OrderId);
        }

        private static bool IsValidTransition(ShipmentStatus currentStatus, ShipmentStatus newStatus)
        {
            return currentStatus switch
            {
                ShipmentStatus.Pending => newStatus == ShipmentStatus.Shipped,

                ShipmentStatus.Shipped => newStatus == ShipmentStatus.InTransit,

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

            if (order.Status is OrderStatus.Cancelled or OrderStatus.Delivered)
                return;

            var shipments = await _shipmentRepository.GetShipmentsByOrderIdAsync(orderId);

            if (shipments.Count == 0)
                return;

            if (shipments.All(s => s.Status == ShipmentStatus.Delivered))
            {
                order.Status = OrderStatus.Delivered;
            }
            else if (shipments.All(s => s.Status != ShipmentStatus.Pending))
            {
                order.Status = OrderStatus.Shipped;
            }

            await _orderRepository.UpdateOrderAsync(order);
        }

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

        public async Task<SellerShipmentDetailsDto> CreateSellerShipmentAsync(long orderId, long userId, CreateSellerShipmentDto dto)
        {
            var sellerProfile = await _sellerRepository.GetByUserIdAsync(userId);

            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found.");

            if (dto.Items == null || dto.Items.Count == 0)
                throw new ValidationException("Shipment must contain at least one item.");

            var order = await _orderRepository.GetSellerOrderDetailsAsync(orderId, sellerProfile.Id);

            if (order == null)
                throw new NotFoundException("Order not found.");

            var orderItems = order.Items.ToDictionary(i => i.Id);

            var shipmentItems = new List<ShipmentItem>();

            foreach (var requestItem in dto.Items)
            {
                if (!orderItems.TryGetValue(requestItem.OrderItemId, out var orderItem))
                    throw new NotFoundException($"Order item {requestItem.OrderItemId} not found.");

                if (requestItem.Quantity <= 0)
                    throw new ValidationException("Shipment quantity must be greater than zero.");

                var alreadyShipped = orderItem.ShipmentItems.Sum(i => i.Quantity);
                var remainingQuantity = orderItem.Quantity - alreadyShipped;

                if (requestItem.Quantity > remainingQuantity)
                    throw new ConflictException($"Shipment quantity exceeds the remaining quantity for order item {requestItem.OrderItemId}.");

                shipmentItems.Add(new ShipmentItem
                {
                    OrderItemId = requestItem.OrderItemId,
                    Quantity = requestItem.Quantity
                });
            }

            var shipment = new Shipment
            {
                OrderId = orderId,
                SellerProfileId = sellerProfile.Id,
                Carrier = dto.Carrier,
                TrackingNumber = dto.TrackingNumber,
                Status = ShipmentStatus.Pending,
                EstimatedDeliveryDate = dto.EstimatedDeliveryDate,
                Items = shipmentItems
            };

            await _shipmentRepository.AddAsync(shipment);
            await _shipmentRepository.SaveChangesAsync();

            return _mapper.Map<SellerShipmentDetailsDto>(shipment);
        }

        public async Task UpdateSellerShipmentAsync(long shipmentId, long userId, UpdateSellerShipmentDto dto)
        {
            var sellerProfile = await _sellerRepository.GetByUserIdAsync(userId);

            if (sellerProfile == null)
                throw new NotFoundException("Seller profile not found.");

            var shipment = await _shipmentRepository.GetSellerShipmentByIdAsync(shipmentId, sellerProfile.Id);

            if (shipment == null)
                throw new NotFoundException("Shipment not found.");

            if (dto.Carrier != null)
                shipment.Carrier = dto.Carrier;

            if (dto.TrackingNumber != null)
                shipment.TrackingNumber = dto.TrackingNumber;

            if (dto.EstimatedDeliveryDate.HasValue)
                shipment.EstimatedDeliveryDate = dto.EstimatedDeliveryDate;

            if (dto.DeliveredDate.HasValue)
                shipment.DeliveredDate = dto.DeliveredDate;

            if (dto.Status.HasValue && dto.Status.Value != shipment.Status)
            {
                if (!IsValidTransition(shipment.Status, dto.Status.Value))
                    throw new ConflictException($"Cannot change shipment status from {shipment.Status} to {dto.Status.Value}.");

                shipment.Status = dto.Status.Value;

                var now = DateTime.UtcNow;

                if (dto.Status.Value == ShipmentStatus.Shipped)
                    shipment.ShippedDate ??= now;

                if (dto.Status.Value == ShipmentStatus.Delivered)
                    shipment.DeliveredDate ??= now;
            }

            shipment.UpdatedAt = DateTime.UtcNow;

            await _shipmentRepository.UpdateShipmentAsync(shipment);
        }


    }
}
