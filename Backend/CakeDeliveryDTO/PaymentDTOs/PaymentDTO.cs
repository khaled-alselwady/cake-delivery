﻿namespace CakeDeliveryDTO
{
    /// <summary>Used when creating a new payment.Used to delete a delivery
    /// </summary>
    /// 
    public record PaymentDTO(
    int ? PaymentID,
    int? OrderID,
    string PaymentMethod,
    DateTime PaymentDate,
    decimal AmountPaid,
    string PaymentStatus
);
}
