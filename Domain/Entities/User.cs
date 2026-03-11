using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Entities;

public class User
{
    [Required]
    [JsonPropertyName("id")] // JSON-и баромад Id мешавад
    public int Id { get; set; }

    [JsonPropertyName("telegram_id")]
    public long TelegramId { get; set; }

    [Required]
    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [Required]
    [JsonPropertyName("address")]
    public required string Address { get; set; }

    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("is_blocked")]
    public bool IsBlocked { get; set; } = false;

    [JsonPropertyName("role")]

    public UserRole Role { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonIgnore] 
    public List<Order> Orders { get; set; } = new();
}