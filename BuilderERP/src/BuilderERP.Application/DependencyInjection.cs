using System.Reflection;
using BuilderERP.Application.Common.Caching;
using BuilderERP.Application.Features.CashPurchaseOrders.Export;
using BuilderERP.Application.Features.CashRequisitions.Export;
using BuilderERP.Application.Features.EngineerWorkOrderRequisitions.Export;
using BuilderERP.Application.Features.EngineerWorkOrders.Export;
using BuilderERP.Application.Features.GoodsReceives.Export;
using BuilderERP.Application.Features.PurchaseOrders.Export;
using BuilderERP.Application.Features.PurchaseRequisitions.Export;
using BuilderERP.Application.Features.Reports;
using BuilderERP.Application.Features.Reports.Export;
using BuilderERP.Application.Features.Rfqs.Export;
using BuilderERP.Application.Features.VendorQuotations.Export;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BuilderERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(cfg => { }, assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
        });

        foreach (var specType in assembly.GetTypes().Where(t => typeof(IReportSpec).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
        {
            services.AddScoped(typeof(IReportSpec), specType);
        }
        services.AddScoped<ReportRegistry>();
        services.AddScoped<ExcelReportExporter>();
        services.AddScoped<PdfReportExporter>();
        services.AddScoped<PurchaseOrderPdfExporter>();
        services.AddScoped<CashPurchaseOrderPdfExporter>();
        services.AddScoped<EngineerWorkOrderPdfExporter>();
        services.AddScoped<PurchaseRequisitionPdfExporter>();
        services.AddScoped<CashRequisitionPdfExporter>();
        services.AddScoped<EngineerWorkOrderRequisitionPdfExporter>();
        services.AddScoped<RfqPdfExporter>();
        services.AddScoped<VendorQuotationPdfExporter>();
        services.AddScoped<GoodsReceivePdfExporter>();

        return services;
    }
}
