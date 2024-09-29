using Webber.Domain.Enums;

namespace Webber.Domain.Entities;
/// <summary>
/// Represents a coupon.
/// </summary>
public class Coupon : BaseEntity
{
    public string Code { get; set; }

    public string? Description { get; set; }

    public CouponType CouponType { get; set; }
    public decimal DiscountAmount { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? MinimumOrderAmount { get; set; }

    public bool IsActive { get; set; }
}