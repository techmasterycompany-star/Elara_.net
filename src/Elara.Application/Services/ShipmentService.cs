using AutoMapper;
using Elara.Application.DTOs.Common;
using Elara.Application.DTOs.Shipment;
using Elara.Application.Exceptions;
using Elara.Application.Interfaces.Repository;
using Elara.Application.Interfaces.Service;
using Elara.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elara.Application.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public ShipmentService(IShipmentRepository shipmentRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _shipmentRepository = shipmentRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<AdminShipmentListDto>> GetAllShipmentsAsync(AdminShipmentFilterDto shipmentRequest)
        {
            var shipments = await _shipmentRepository.GetAllShipmentsAsync(shipmentRequest);
            var shipmentDtos = _mapper.Map<IEnumerable<AdminShipmentListDto>>(shipments);

            return new PaginatedResponse<AdminShipmentListDto>
            {
                Data = shipmentDtos.ToList(),
                TotalCount = shipmentDtos.Count(),
                PageNumber = shipmentRequest.PageNumber,
                Limit = shipmentRequest.Limit,
                TotalPages = (int)Math.Ceiling((double)shipments.Count() / shipmentRequest.Limit)
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
                throw new ValidationException($"Shipment is already {updateRequest.Status}.");
            if (!IsValidTransition(shipment.Status, updateRequest.Status))
                throw new ValidationException($"Cannot change shipment status from {shipment.Status} to {updateRequest.Status}.");

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
    }
}
