using BuilderERP.Domain.Enums;

namespace BuilderERP.Application.DTOs;

public class MaterialDto
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; }
    public Guid? CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public UnitOfMeasure PurchaseUnit { get; set; }
    public decimal UnitConversionFactor { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal MaxStockLevel { get; set; }
    public decimal AveragePurchasePrice { get; set; }
    public decimal LastPurchasePrice { get; set; }
    public decimal StandardPurchasePrice { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool IsBatchTracked { get; set; }
    public bool IsSerialTracked { get; set; }
}

public class CreateMaterialDto
{
    public string MaterialCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.Piece;
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Brand { get; set; }
    public UnitOfMeasure PurchaseUnit { get; set; } = UnitOfMeasure.Piece;
    public decimal UnitConversionFactor { get; set; } = 1;
    public decimal MinStockLevel { get; set; }
    public decimal MaxStockLevel { get; set; }
    public decimal StandardPurchasePrice { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool IsBatchTracked { get; set; }
    public bool IsSerialTracked { get; set; }
}

public class UpdateMaterialDto
{
    public Guid Id { get; set; }
    public string MaterialCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; }
    public decimal ReorderLevel { get; set; }
    public string? Barcode { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Brand { get; set; }
    public UnitOfMeasure PurchaseUnit { get; set; }
    public decimal UnitConversionFactor { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal MaxStockLevel { get; set; }
    public decimal StandardPurchasePrice { get; set; }
    public decimal VatPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal DiscountPercent { get; set; }
    public bool IsBatchTracked { get; set; }
    public bool IsSerialTracked { get; set; }
}
