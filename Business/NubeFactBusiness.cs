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
    private static readonly TimeZoneInfo PeruTimeZone = ResolvePeruTimeZone();

    public async Task<NubeFactResult> GenerateInvoiceAsync(int orderId)
    {
        var existingInvoices = await invoiceRepository.FindAsync(i => i.OrderId == orderId);
        var existingInvoice = existingInvoices.FirstOrDefault();
        if (existingInvoice != null) return new NubeFactResult(true, existingInvoice.PdfUrl);

        var order = await orderRepository.GetByIdAsync(orderId);
        if (order == null) return new NubeFactResult(false, Error: "Pedido no encontrado.");

        var details = await detailRepository.FindAsync(d => d.OrderId == orderId);
        if (!details.Any()) return new NubeFactResult(false, Error: "El pedido no tiene productos.");

        var client = order.ClientId.HasValue ? await clientRepository.GetByIdAsync(order.ClientId.Value) : null;

        var clientName = client?.Name ?? "CLIENTE GENERICO";
        var clientEmail = "correo@ejemplo.com"; 
        var clientDocument = ResolveClientDocument(client);

        var total = Math.Round(order.TotalAmount, 2);
        var totalGravada = Math.Round(total / 1.18m, 2);
        var totalIgv = total - totalGravada;
        var peruNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, PeruTimeZone);

        var requestBody = new
        {
            operacion = "generar_comprobante",
            tipo_de_comprobante = "2", // Boleta
            serie = "BBB1",
            numero = "0",
            sunat_transaction = "1",
            cliente_tipo_de_documento = clientDocument.TypeCode,
            cliente_numero_de_documento = clientDocument.Number,
            cliente_denominacion = clientName,
            cliente_direccion = client?.Address ?? "-",
            cliente_email = clientEmail,
            fecha_de_emision = peruNow.ToString("dd-MM-yyyy"),
            moneda = "1", // Soles
            porcentaje_de_igv = "18.00",
            total_gravada = totalGravada.ToString("F2"),
            total_igv = totalIgv.ToString("F2"),
            total = total.ToString("F2"),
            codigo_unico = BuildUniqueCode(order.Id, peruNow),
            items = details.Select(d => {
                var itemTotal = Math.Round(d.Subtotal, 2);
                var itemUnitPrice = Math.Round(d.UnitPrice, 2);
                var itemValorUnitario = Math.Round(itemUnitPrice / 1.18m, 2);
                var itemSubtotal = Math.Round(itemTotal / 1.18m, 2);
                var itemIgv = itemTotal - itemSubtotal;

                return new
                {
                    unidad_de_medida = "NIU",
                    codigo = $"P{d.ProductId}",
                    descripcion = $"Producto {d.ProductId}",
                    cantidad = d.Quantity.ToString("F0"),
                    valor_unitario = itemValorUnitario.ToString("F2"),
                    precio_unitario = itemUnitPrice.ToString("F2"),
                    subtotal = itemSubtotal.ToString("F2"),
                    tipo_de_igv = "1",
                    igv = itemIgv.ToString("F2"),
                    total = itemTotal.ToString("F2")
                };
            }).ToList()
        };

        var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Token token=\"{_token}\"");

        try
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var url = _endpoint.EndsWith(".json") ? _endpoint : $"{_endpoint}.json";
            
            var response = await httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<NubeFactResponse>(responseContent);
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
                    return new NubeFactResult(true, invoice.PdfUrl);
                }
            }
            else
            {
                var errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject<NubeFactResponse>(responseContent);
                return new NubeFactResult(false, Error: errorResult?.errors ?? $"NubeFact Error: {response.StatusCode}");
            }
        }
        catch (TaskCanceledException)
        {
            return new NubeFactResult(false, Error: "Tiempo de espera agotado con NubeFact (30s).");
        }
        catch (Exception ex)
        {
            return new NubeFactResult(false, Error: $"Error: {ex.Message}");
        }

        return new NubeFactResult(false, Error: "Error desconocido al procesar NubeFact.");
    }

    private static (string TypeCode, string Number) ResolveClientDocument(Client? client)
    {
        if (client == null || 
            string.IsNullOrWhiteSpace(client.DocumentType) || 
            string.IsNullOrWhiteSpace(client.DocumentNumber))
        {
            return ("1", "00000000");
        }

        var typeCode = client.DocumentType.Trim().ToUpperInvariant() switch
        {
            "DNI" => "1",
            "RUC" => "6",
            _ => "1" // Default to DNI if unknown
        };

        var documentNumber = client.DocumentNumber.Trim().ToUpperInvariant();
        
        // If it was supposed to be DNI but doesn't look like one (8 digits), 
        // or RUC but doesn't look like one (11 digits), we might still want to try,
        // but for now let's just use what we have or generic if empty.
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            return ("1", "00000000");
        }

        return (typeCode, documentNumber);
    }

    private static string BuildUniqueCode(int orderId, DateTime peruNow)
    {
        return $"PED-{orderId}-{peruNow:yyyyMMddHHmmssfff}";
    }

    private static TimeZoneInfo ResolvePeruTimeZone()
    {
        var candidates = new[]
        {
            "SA Pacific Standard Time", // Windows
            "America/Lima" // Linux/macOS
        };

        foreach (var candidate in candidates)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(candidate);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Utc;
    }

    private class NubeFactResponse
    {
        public string serie { get; set; } = string.Empty;
        public string numero { get; set; } = string.Empty;
        public string key { get; set; } = string.Empty;
        public string enlace_del_pdf { get; set; } = string.Empty;
        public string enlace_del_xml { get; set; } = string.Empty;
        public string enlace_del_cdr { get; set; } = string.Empty;
        public string? errors { get; set; }
    }
}
