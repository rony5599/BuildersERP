using System.Reflection;
using BuilderERP.Application.Features.Reports;
using BuilderERP.Application.Features.Reports.Export;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BuilderERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        foreach (var specType in assembly.GetTypes().Where(t => typeof(IReportSpec).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract))
        {
            services.AddScoped(typeof(IReportSpec), specType);
        }
        services.AddScoped<ReportRegistry>();
        services.AddScoped<ExcelReportExporter>();
        services.AddScoped<PdfReportExporter>();

        return services;
    }
}
