using System;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Entities
{
    public class Order
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("productId")]
        public int ProductId { get; set; }

        [JsonPropertyName("address")]
        public required string Address { get; set; }

        [JsonPropertyName("paymentMethod")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [JsonPropertyName("status")]
        public OrderStatus Status { get; set; } = OrderStatus.Processing;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("user")]
        public User? User { get; set; }

        [JsonPropertyName("product")]
        public Product? Product { get; set; }
    }
}