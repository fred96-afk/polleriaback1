using System.Text;

namespace Business.Templates;

public static class EmailTemplates
{
    private const string BrandName = "Pollería El Gigante";
    private const string BrandTagline = "Tradición, Sabor & Fuego a la Brasa";
    private const string BrandColorPrimary = "#d32f2f";
    private const string BrandColorDark = "#b71c1c";
    private const string BrandColorAccent = "#ff9800";
    private const string SupportPhone = "(01) 456-7890";
    private const string SupportWhatsapp = "+51 987 654 321";
    private const string SupportEmail = "contacto@elgigante.pe";

    /// <summary>
    /// 1. Plantilla de Verificación de Cuenta / Bienvenida
    /// </summary>
    public static string AccountVerification(string name, string verificationLink, string? frontendUrl = null)
    {
        var content = $@"
            <div style=""text-align: center; margin-bottom: 25px;"">
                <div style=""display: inline-block; width: 64px; height: 64px; line-height: 64px; background: #fee2e2; border-radius: 50%; font-size: 32px;"">
                    🍗
                </div>
                <h2 style=""color: #1e293b; font-size: 24px; margin: 15px 0 5px; font-weight: 800;"">¡Bienvenido a {BrandName}!</h2>
                <p style=""color: #64748b; font-size: 15px; margin: 0;"">Estamos felices de que formes parte de nuestra familia</p>
            </div>

            <p style=""color: #334155; font-size: 15px; line-height: 1.6; margin-bottom: 20px;"">
                Hola <strong>{Escape(name)}</strong>, gracias por registrarte en nuestra plataforma. Para garantizar la seguridad de tu cuenta y comenzar a disfrutar de nuestras promociones exclusivas, por favor activa tu cuenta:
            </p>

            <div style=""text-align: center; margin: 30px 0;"">
                <a href=""{verificationLink}"" 
                   style=""background: linear-gradient(135deg, {BrandColorPrimary} 0%, {BrandColorDark} 100%); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-size: 15px; font-weight: 700; display: inline-block; box-shadow: 0 4px 12px rgba(211, 47, 47, 0.35);"">
                    Confirmar mi Correo Electrónico
                </a>
            </div>

            <div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 16px; margin: 25px 0;"">
                <p style=""margin: 0 0 8px; font-size: 12px; color: #64748b; font-weight: 600; text-transform: uppercase;"">¿El botón no funciona? Copia este enlace en tu navegador:</p>
                <p style=""margin: 0; font-size: 13px; color: {BrandColorPrimary}; word-break: break-all;"">
                    <a href=""{verificationLink}"" style=""color: {BrandColorPrimary}; text-decoration: underline;"">{verificationLink}</a>
                </p>
            </div>

            <div style=""border-top: 1px solid #f1f5f9; padding-top: 20px; margin-top: 25px;"">
                <h4 style=""color: #334155; font-size: 14px; margin: 0 0 12px;"">¿Qué puedes hacer con tu cuenta?</h4>
                <table style=""width: 100%; border-collapse: collapse;"">
                    <tr>
                        <td style=""padding: 6px 0; vertical-align: top; width: 24px; font-size: 16px;"">🔥</td>
                        <td style=""padding: 6px 0; color: #475569; font-size: 14px;"">Ordenar tus pollos a la brasa favoritos para delivery o recojo en tienda.</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; vertical-align: top; width: 24px; font-size: 16px;"">🎁</td>
                        <td style=""padding: 6px 0; color: #475569; font-size: 14px;"">Acceder a promociones y cupones de descuento exclusivos.</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; vertical-align: top; width: 24px; font-size: 16px;"">🧾</td>
                        <td style=""padding: 6px 0; color: #475569; font-size: 14px;"">Consultar tu historial de compras y comprobantes de pago electrónicos.</td>
                    </tr>
                </table>
            </div>

            <p style=""color: #94a3b8; font-size: 12px; margin-top: 25px; line-height: 1.5;"">
                Si no realizaste este registro, puedes desestimar este mensaje con total seguridad.
            </p>
        ";

        return WrapInLayout("Verifica tu cuenta", "¡Bienvenido a Pollería El Gigante! Confirma tu correo para activar tu cuenta.", content);
    }

    /// <summary>
    /// 2. Plantilla de Recuperación de Cuenta / Restablecer Contraseña
    /// </summary>
    public static string PasswordReset(string name, string resetLink, int expirationHours = 1)
    {
        var content = $@"
            <div style=""text-align: center; margin-bottom: 25px;"">
                <div style=""display: inline-block; width: 64px; height: 64px; line-height: 64px; background: #fef3c7; border-radius: 50%; font-size: 32px;"">
                    🔐
                </div>
                <h2 style=""color: #1e293b; font-size: 24px; margin: 15px 0 5px; font-weight: 800;"">Restablecer Contraseña</h2>
                <p style=""color: #64748b; font-size: 15px; margin: 0;"">Recibimos una solicitud para cambiar tu contraseña</p>
            </div>

            <p style=""color: #334155; font-size: 15px; line-height: 1.6; margin-bottom: 20px;"">
                Hola <strong>{Escape(name)}</strong>, hemos recibido una petición para restablecer la contraseña asociada a tu cuenta en <strong>{BrandName}</strong>.
            </p>

            <p style=""color: #334155; font-size: 15px; line-height: 1.6;"">
                Para ingresar una nueva contraseña, haz clic en el siguiente botón:
            </p>

            <div style=""text-align: center; margin: 30px 0;"">
                <a href=""{resetLink}"" 
                   style=""background: linear-gradient(135deg, {BrandColorPrimary} 0%, {BrandColorDark} 100%); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-size: 15px; font-weight: 700; display: inline-block; box-shadow: 0 4px 12px rgba(211, 47, 47, 0.35);"">
                    Crear Nueva Contraseña
                </a>
            </div>

            <div style=""background: #fffbeb; border-left: 4px solid #f59e0b; padding: 14px 16px; border-radius: 4px; margin: 25px 0;"">
                <p style=""margin: 0; font-size: 13px; color: #92400e; line-height: 1.5;"">
                    ⏰ <strong>Aviso de seguridad:</strong> Este enlace expirará en <strong>{expirationHours} hora(s)</strong> por motivos de seguridad.
                </p>
            </div>

            <div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 16px; margin: 20px 0;"">
                <p style=""margin: 0 0 6px; font-size: 12px; color: #64748b; font-weight: 600;"">Enlace alternativo:</p>
                <p style=""margin: 0; font-size: 12px; color: {BrandColorPrimary}; word-break: break-all;"">
                    <a href=""{resetLink}"" style=""color: {BrandColorPrimary}; text-decoration: underline;"">{resetLink}</a>
                </p>
            </div>

            <p style=""color: #64748b; font-size: 13px; line-height: 1.5; margin-top: 20px;"">
                🔒 <strong>¿No solicitaste este cambio?</strong> Puedes ignorar este correo; tu contraseña actual continuará siendo totalmente segura y válida.
            </p>
        ";

        return WrapInLayout("Restablecer Contraseña", "Solicitud de recuperación de contraseña en Pollería El Gigante.", content);
    }

    /// <summary>
    /// 3. Plantilla de Confirmación de Compra / Pedido Realizado
    /// </summary>
    public static string OrderConfirmation(
        string customerName,
        string orderId,
        decimal total,
        string? pdfUrl = null,
        string? paymentMethod = null,
        string? orderType = null,
        DateTime? orderDate = null,
        string? trackingUrl = null)
    {
        var displayDate = (orderDate ?? DateTime.UtcNow).ToString("dd/MM/yyyy hh:mm tt");
        var displayPayment = string.IsNullOrWhiteSpace(paymentMethod) ? "Pago en línea / Tarjeta" : paymentMethod;
        var displayType = string.IsNullOrWhiteSpace(orderType) ? "Delivery a Domicilio" : orderType;

        var pdfButtonHtml = !string.IsNullOrWhiteSpace(pdfUrl)
            ? $@"
                <div style=""text-align: center; margin: 25px 0 10px;"">
                    <a href=""{pdfUrl}"" 
                       style=""background: linear-gradient(135deg, {BrandColorPrimary} 0%, {BrandColorDark} 100%); color: #ffffff; padding: 13px 28px; text-decoration: none; border-radius: 8px; font-size: 14px; font-weight: 700; display: inline-block; box-shadow: 0 4px 10px rgba(211, 47, 47, 0.3);"">
                        📄 Descargar Comprobante (PDF)
                    </a>
                </div>"
            : "";

        var trackingButtonHtml = !string.IsNullOrWhiteSpace(trackingUrl)
            ? $@"
                <div style=""text-align: center; margin: 10px 0 20px;"">
                    <a href=""{trackingUrl}"" 
                       style=""background: #f1f5f9; color: #334155; padding: 11px 24px; text-decoration: none; border-radius: 8px; font-size: 13px; font-weight: 600; display: inline-block; border: 1px solid #cbd5e1;"">
                        🛵 Ver Estado de mi Pedido
                    </a>
                </div>"
            : "";

        var content = $@"
            <div style=""text-align: center; margin-bottom: 25px;"">
                <div style=""display: inline-block; width: 64px; height: 64px; line-height: 64px; background: #dcfce7; border-radius: 50%; font-size: 32px;"">
                    ✅
                </div>
                <h2 style=""color: #1e293b; font-size: 24px; margin: 15px 0 5px; font-weight: 800;"">¡Gracias por tu compra!</h2>
                <p style=""color: #16a34a; font-size: 15px; margin: 0; font-weight: 600;"">Tu pedido ha sido confirmado con éxito</p>
            </div>

            <p style=""color: #334155; font-size: 15px; line-height: 1.6;"">
                Hola <strong>{Escape(customerName)}</strong>, tu pedido ya está siendo preparado en nuestra cocina con todo el sabor tradicional y la calidad que nos caracteriza.
            </p>

            <!-- Card de Resumen de Pedido -->
            <div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 10px; padding: 20px; margin: 25px 0;"">
                <div style=""border-bottom: 1px solid #e2e8f0; padding-bottom: 12px; margin-bottom: 15px;"">
                    <span style=""font-size: 12px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px;"">Resumen del Pedido</span>
                    <span style=""float: right; background: #dcfce7; color: #166534; font-size: 11px; font-weight: 700; padding: 3px 10px; border-radius: 12px;"">CONFIRMADO</span>
                </div>

                <table style=""width: 100%; border-collapse: collapse; font-size: 14px;"">
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">N° de Pedido:</td>
                        <td style=""padding: 6px 0; color: #1e293b; font-weight: 700; text-align: right;"">#{Escape(orderId)}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">Fecha y Hora:</td>
                        <td style=""padding: 6px 0; color: #1e293b; text-align: right;"">{displayDate}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">Modalidad:</td>
                        <td style=""padding: 6px 0; color: #1e293b; text-align: right;"">{Escape(displayType)}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">Método de Pago:</td>
                        <td style=""padding: 6px 0; color: #1e293b; text-align: right;"">{Escape(displayPayment)}</td>
                    </tr>
                    <tr style=""border-top: 1px solid #cbd5e1;"">
                        <td style=""padding: 12px 0 0; color: #0f172a; font-size: 16px; font-weight: 800;"">Total del Pedido:</td>
                        <td style=""padding: 12px 0 0; color: {BrandColorPrimary}; font-size: 20px; font-weight: 800; text-align: right;"">S/ {total:F2}</td>
                    </tr>
                </table>
            </div>

            {pdfButtonHtml}
            {trackingButtonHtml}

            <div style=""background: #f1f5f9; border-radius: 8px; padding: 14px; margin-top: 25px; text-align: center;"">
                <p style=""margin: 0; font-size: 13px; color: #475569;"">
                    ¿Tienes alguna duda sobre tu entrega? Escríbenos a nuestro WhatsApp 
                    <strong style=""color: #0f172a;"">{SupportWhatsapp}</strong>
                </p>
            </div>
        ";

        return WrapInLayout($"Confirmación de Pedido #{orderId}", $"¡Gracias por tu compra! Tu pedido #{orderId} está confirmado y en preparación.", content);
    }

    /// <summary>
    /// 4. Plantilla de Notificación de Reporte Generado para el Administrador
    /// </summary>
    public static string AdminReportCompleted(
        string adminName,
        string reportTitle,
        string reportType,
        string downloadUrl,
        DateTime? startDate = null,
        DateTime? endDate = null,
        DateTime? generatedAt = null)
    {
        var displayGenDate = (generatedAt ?? DateTime.UtcNow).ToString("dd/MM/yyyy HH:mm:ss") + " UTC";
        var dateRangeText = (startDate.HasValue && endDate.HasValue)
            ? $"{startDate.Value:dd/MM/yyyy} al {endDate.Value:dd/MM/yyyy}"
            : "Periodo histórico completo";

        var isExcel = reportType.Equals("Excel", StringComparison.OrdinalIgnoreCase);
        var typeBadgeColor = isExcel ? "#16a34a" : "#dc2626";
        var typeBadgeBg = isExcel ? "#dcfce7" : "#fee2e2";
        var iconEmoji = isExcel ? "📊" : "📑";

        var content = $@"
            <div style=""text-align: center; margin-bottom: 25px;"">
                <div style=""display: inline-block; width: 64px; height: 64px; line-height: 64px; background: #e0f2fe; border-radius: 50%; font-size: 32px;"">
                    {iconEmoji}
                </div>
                <h2 style=""color: #1e293b; font-size: 24px; margin: 15px 0 5px; font-weight: 800;"">Reporte Listo para Descarga</h2>
                <p style=""color: #0284c7; font-size: 14px; margin: 0; font-weight: 600;"">Panel de Control Administrativo</p>
            </div>

            <p style=""color: #334155; font-size: 15px; line-height: 1.6;"">
                Estimado(a) <strong>{Escape(adminName)}</strong>, te informamos que el procesamiento en segundo plano del reporte solicitado ha culminado satisfactoriamente.
            </p>

            <!-- Card de Detalles del Reporte -->
            <div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 10px; padding: 20px; margin: 25px 0;"">
                <div style=""border-bottom: 1px solid #e2e8f0; padding-bottom: 12px; margin-bottom: 14px;"">
                    <span style=""font-size: 12px; font-weight: 700; color: #64748b; text-transform: uppercase;"">Detalles del Archivo</span>
                    <span style=""float: right; background: {typeBadgeBg}; color: {typeBadgeColor}; font-size: 11px; font-weight: 700; padding: 3px 10px; border-radius: 12px;"">
                        {reportType.ToUpperInvariant()}
                    </span>
                </div>

                <table style=""width: 100%; border-collapse: collapse; font-size: 14px;"">
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">Nombre:</td>
                        <td style=""padding: 6px 0; color: #1e293b; font-weight: 600; text-align: right;"">{Escape(reportTitle)}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">Rango Evaluado:</td>
                        <td style=""padding: 6px 0; color: #1e293b; text-align: right;"">{dateRangeText}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">Fecha de Emisión:</td>
                        <td style=""padding: 6px 0; color: #1e293b; text-align: right;"">{displayGenDate}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #64748b;"">Estado:</td>
                        <td style=""padding: 6px 0; color: #16a34a; font-weight: 700; text-align: right;"">✔ Completado</td>
                    </tr>
                </table>
            </div>

            <div style=""text-align: center; margin: 30px 0;"">
                <a href=""{downloadUrl}"" 
                   style=""background: linear-gradient(135deg, #0284c7 0%, #0369a1 100%); color: #ffffff; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-size: 15px; font-weight: 700; display: inline-block; box-shadow: 0 4px 12px rgba(2, 132, 199, 0.35);"">
                    ⬇ Descargar Reporte ({reportType.ToUpperInvariant()})
                </a>
            </div>

            <div style=""background: #fffbeb; border-left: 4px solid #f59e0b; padding: 12px 16px; border-radius: 4px; margin: 20px 0;"">
                <p style=""margin: 0; font-size: 12px; color: #92400e; line-height: 1.5;"">
                    💡 <strong>Nota del Sistema:</strong> Los reportes generados se almacenan temporalmente en el servidor. Recomendamos descargar el archivo y archivarlo en tu almacenamiento seguro.
                </p>
            </div>
        ";

        return WrapInLayout($"Reporte Listo: {reportTitle}", $"Tu reporte administrativo en formato {reportType} ha sido generado exitosamente.", content);
    }

    /// <summary>
    /// 5. Plantilla de Aviso de Mantenimiento de la Web / Plataforma
    /// </summary>
    public static string MaintenanceNotice(
        string recipientName,
        string scheduledDate,
        string estimatedDuration,
        string details,
        string? supportContact = null)
    {
        var supportPhone = string.IsNullOrWhiteSpace(supportContact) ? SupportWhatsapp : supportContact;

        var content = $@"
            <div style=""text-align: center; margin-bottom: 25px;"">
                <div style=""display: inline-block; width: 64px; height: 64px; line-height: 64px; background: #fef2f2; border-radius: 50%; font-size: 32px;"">
                    🛠️
                </div>
                <h2 style=""color: #1e293b; font-size: 24px; margin: 15px 0 5px; font-weight: 800;"">Aviso de Mantenimiento Programado</h2>
                <p style=""color: #ea580c; font-size: 14px; margin: 0; font-weight: 600;"">Actualización y Optimización de Plataforma</p>
            </div>

            <p style=""color: #334155; font-size: 15px; line-height: 1.6;"">
                Estimado(a) <strong>{Escape(recipientName)}</strong>,
            </p>

            <p style=""color: #334155; font-size: 15px; line-height: 1.6;"">
                En <strong>{BrandName}</strong> nos comprometemos a brindarte la mejor experiencia digital, velocidad y máxima seguridad en tus pedidos. Por este motivo, nuestro equipo técnico llevará a cabo una ventana de mantenimiento programado en nuestros servidores.
            </p>

            <!-- Card de Ventana de Mantenimiento -->
            <div style=""background: #fff7ed; border: 1px solid #ffedd5; border-radius: 10px; padding: 20px; margin: 25px 0;"">
                <div style=""border-bottom: 1px solid #fed7aa; padding-bottom: 10px; margin-bottom: 12px;"">
                    <span style=""font-size: 12px; font-weight: 700; color: #9a3412; text-transform: uppercase;"">Horario de la Intervención</span>
                </div>

                <table style=""width: 100%; border-collapse: collapse; font-size: 14px;"">
                    <tr>
                        <td style=""padding: 6px 0; color: #7c2d12; font-weight: 600;"">📅 Fecha de Inicio:</td>
                        <td style=""padding: 6px 0; color: #1e293b; text-align: right; font-weight: 700;"">{Escape(scheduledDate)}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #7c2d12; font-weight: 600;"">⏱ Duración Estimada:</td>
                        <td style=""padding: 6px 0; color: #1e293b; text-align: right; font-weight: 700;"">{Escape(estimatedDuration)}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 6px 0; color: #7c2d12; font-weight: 600;"">🌐 Servicios Afectados:</td>
                        <td style=""padding: 6px 0; color: #dc2626; text-align: right; font-weight: 700;"">Web & Tienda Online</td>
                    </tr>
                </table>

                <div style=""border-top: 1px solid #fed7aa; margin-top: 12px; padding-top: 10px;"">
                    <p style=""margin: 0; font-size: 13px; color: #9a3412; line-height: 1.5;"">
                        <strong>Motivo:</strong> {Escape(details)}
                    </p>
                </div>
            </div>

            <!-- Canales Alternativos -->
            <div style=""background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 18px; margin: 25px 0;"">
                <h4 style=""color: #1e293b; font-size: 14px; margin: 0 0 10px; font-weight: 700;"">¿Deseas realizar un pedido durante este tiempo?</h4>
                <p style=""color: #475569; font-size: 13px; margin: 0 0 10px; line-height: 1.5;"">
                    Nuestros locales físicos y líneas telefónicas continuarán atendiendo con normalidad:
                </p>
                <table style=""width: 100%; border-collapse: collapse; font-size: 13px;"">
                    <tr>
                        <td style=""padding: 4px 0; color: #334155;"">💬 <strong>WhatsApp de Pedidos:</strong></td>
                        <td style=""padding: 4px 0; text-align: right; color: {BrandColorPrimary}; font-weight: 700;"">{supportPhone}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 4px 0; color: #334155;"">📞 <strong>Central Telefónica:</strong></td>
                        <td style=""padding: 4px 0; text-align: right; color: #334155;"">{SupportPhone}</td>
                    </tr>
                </table>
            </div>

            <p style=""color: #64748b; font-size: 13px; line-height: 1.5; text-align: center; margin-top: 25px;"">
                Agradecemos sinceramente tu comprensión y paciencia mientras optimizamos nuestra plataforma.
            </p>
        ";

        return WrapInLayout("Aviso de Mantenimiento", "Mantenimiento preventivo y actualización programada de la plataforma web.", content);
    }

    /// <summary>
    /// Envoltorio maestro responsivo (Layout HTML) con identidad corporativa de Pollería El Gigante
    /// </summary>
    private static string WrapInLayout(string pageTitle, string preheaderText, string bodyContent)
    {
        return $@"<!DOCTYPE html>
<html lang=""es"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <title>{Escape(pageTitle)} - {BrandName}</title>
    <!--[if mso]>
    <style type=""text/css"">
        table {{ border-collapse: collapse; }}
        .fallback-font {{ font-family: Arial, sans-serif !important; }}
    </style>
    <![endif]-->
</head>
<body style=""margin: 0; padding: 0; background-color: #f1f5f9; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; -webkit-font-smoothing: antialiased; -moz-osx-font-smoothing: grayscale; color: #334155;"">
    <!-- Texto de vista previa en bandeja de entrada -->
    <div style=""display: none; max-height: 0px; overflow: hidden; mso-hide: all;"">
        {Escape(preheaderText)} &nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;
    </div>

    <!-- Contenedor General -->
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f1f5f9; min-width: 100%;"">
        <tr>
            <td align=""center"" style=""padding: 30px 15px;"">
                <!-- Tabla Principal Centrada (Max 600px) -->
                <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px; background-color: #ffffff; border-radius: 14px; overflow: hidden; box-shadow: 0 4px 20px rgba(0, 0, 0, 0.06); border: 1px solid #e2e8f0;"">
                    
                    <!-- ENCABEZADO DE MARCA -->
                    <tr>
                        <td align=""center"" style=""background: linear-gradient(135deg, {BrandColorDark} 0%, {BrandColorPrimary} 50%, #ef4444 100%); padding: 32px 20px; text-align: center;"">
                            <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"">
                                <tr>
                                    <td align=""center"" style=""padding-bottom: 8px;"">
                                        <span style=""font-size: 38px; line-height: 1; display: inline-block;"">🔥</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"">
                                        <h1 style=""color: #ffffff; font-size: 22px; margin: 0; font-weight: 900; letter-spacing: 2px; text-transform: uppercase;"">
                                            {BrandName}
                                        </h1>
                                        <p style=""color: #fecaca; font-size: 12px; margin: 6px 0 0; text-transform: uppercase; letter-spacing: 1.5px; font-weight: 600;"">
                                            {BrandTagline}
                                        </p>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <!-- CUERPO PRINCIPAL DEL CORREO -->
                    <tr>
                        <td style=""padding: 35px 30px;"">
                            {bodyContent}
                        </td>
                    </tr>

                    <!-- PIE DE PÁGINA CORPORATIVO -->
                    <tr>
                        <td style=""background-color: #0f172a; padding: 25px 30px; text-align: center; color: #94a3b8; font-size: 12px; line-height: 1.6;"">
                            <p style=""margin: 0 0 8px; color: #ffffff; font-weight: 700; font-size: 13px;"">
                                {BrandName}
                            </p>
                            <p style=""margin: 0 0 8px;"">
                                📍 Av. Principal 123, Lima - Perú &bull; 📞 {SupportPhone} &bull; 💬 {SupportWhatsapp}
                            </p>
                            <p style=""margin: 0 0 12px; color: #64748b;"">
                                Horario de atención: Lunes a Domingo de 12:00 PM a 11:00 PM
                            </p>
                            <div style=""border-top: 1px solid #1e293b; padding-top: 12px; margin-top: 12px; font-size: 11px; color: #475569;"">
                                Este correo fue generado automáticamente por nuestro sistema.<br>
                                &copy; {DateTime.UtcNow.Year} {BrandName}. Todos los derechos reservados.
                            </div>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }

    private static string Escape(string? input) =>
        System.Net.WebUtility.HtmlEncode(input ?? string.Empty);
}