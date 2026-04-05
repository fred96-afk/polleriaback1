using System.Net.Http.Json;
using DbModel.Tables;
using IBusiness;
using IRepository;
using Microsoft.Extensions.Configuration;
using Models.Orders;

namespace Business;

public class NubeFactBusiness(
    IHttpClientFactory httpClientFactory,
    IOrderRepository orderRepository,
    IOrderDetailRepository detailRepository,
    IClientRepository clientRepository,
    IInvoiceRepository invoiceRepository,
    IConfiguration configuration) : INubeFactBusiness
{
    private readonly string _token = configuration["NubeFact:Token"] ?? throw new InvalidOperationException("NubeFact Token not found.");
    private readonly string _endpoint = configuration["NubeFact:Endpoint"] ?? throw new InvalidOperationException("NubeFact Endpoint not found.");

    public async Task<bool> GenerateInvoiceAsync(int orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null) return false;

        var details = await detailRepository.FindAsync(d => d.OrderId == orderId);
        var client = order.ClientId.HasValue ? await clientRepository.GetByIdAsync(order.ClientId.Value) : null;

        var clientName = client?.Name ?? "CLIENTE GENERICO";
        var clientEmail = "correo@ejemplo.com"; // Debería estar en el cliente

        var requestBody = new
        {
            operacion = "generar_comprobante",
            tipo_de_comprobante = 2, // 1 = Factura, 2 = Boleta
            serie = "BBB1",
            numero = 0, // 0 para autogenerar
            sunat_transaction = 1,
            cliente_tipo_de_documento = 1, // 1 = DNI
            cliente_numero_de_documento = "00000000",
            cliente_denominacion = clientName,
            cliente_direccion = client?.Address ?? "-",
            cliente_email = clientEmail,
            fecha_de_emision = DateTime.UtcNow.ToString("dd-MM-yyyy"),
            moneda = 1, // 1 = Soles
            porcentaje_de_igv = 18.00,
            total_gravada = order.TotalAmount / 1.18m,
            total_igv = order.TotalAmount - (order.TotalAmount / 1.18m),
            total = order.TotalAmount,
            items = details.Select(d => new
            {
                unidad_de_medida = "NIU",
                codigo = d.ProductId.ToString(),
                descripcion = $"Producto {d.ProductId}",
                cantidad = d.Quantity,
                valor_unitario = d.UnitPrice / 1.18m,
                precio_unitario = d.UnitPrice,
                subtotal = d.Subtotal / 1.18m,
                tipo_de_igv = 1, // Gravado - Operación Onerosa
                igv = d.Subtotal - (d.Subtotal / 1.18m),
                total = d.Subtotal
            }).ToList()
        };

        var httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Token token=\"{_token}\"");

        var response = await httpClient.PostAsJsonAsync(_endpoint, requestBody);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<NubeFactResponse>();
            if (result != null && !string.IsNullOrEmpty(result.serie))
            {
                var invoice = new Invoice
                {
                    OrderId = orderId,
                    Serie = result.serie,
                    Number = int.Parse(result.numero),
                    ExternalId = result.key,
                    PdfUrl = result.enlace_del_pdf,
                    XmlUrl = result.enlace_del_xml,
                    CdrUrl = result.enlace_del_cdr
                };
                await invoiceRepository.AddAsync(invoice);
                await invoiceRepository.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }

    private class NubeFactResponse
    {
        public string serie { get; set; } = string.Empty;
        public string numero { get; set; } = string.Empty;
        public string key { get; set; } = string.Empty;
        public string enlace_del_pdf { get; set; } = string.Empty;
        public string enlace_del_xml { get; set; } = string.Empty;
        public string enlace_del_cdr { get; set; } = string.Empty;
    }
}
