using System;
using System.Collections.Generic;
using System.Text;

namespace Afip.Services.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic;

    public class EmpresaInfo
    {
        public long Cuit;
        public string NombreEmpresa;
        public string PathCertificado;
    }

    public class Ticket
    {
        public string Source; // DN del computador que realiza el requerimiento
        public string Destination; // DN del WSAA
        public UInt32 UniqueId; // Entero de 32 bits sin signo que identifica el requerimiento
        public DateTime GenerationTime; // Momento en que fue generado el requerimiento
        public DateTime ExpirationTime; // Momento en el que exoira la solicitud
        public string Service; // Identificacion del WSN para el cual se solicita el TA
        public string Sign; // Firma de seguridad recibida en la respuesta
        public string Token; // Token de seguridad recibido en la respuesta
    }

    public class Factura
    {
        public int Tipo_Cbte;
        public int Punto_Emision;
        public long Numero;
        public DateTime Fecha_cbte;
        public DateTime Fecha_Vencimiento;
        public int Tipo_Doc;
        public long Numero_Doc;
        public string TipoDoc;
        public string NombreCliente;
        public string DomicilioCliente;
        public string IdImpositivo;
        public int Tipo_Expo;
        public string MondedaId;
        public double MonedaCotizacion;
        public string Observaciones;
        public string FormaDePago;
        public string Incoterms;
        public int Idioma_cbte;
        public double Importe_Neto;
        public double Importe_Exento;
        public double Importe_CNG;
        public double Importe_Iva;
        public double Importe_Total;
        public double Importe_RNI;
        public double Importe_OTributos;
        public string cae;
        public DateTime FechaVencimientoCae;
        public DateTime FechaEmisionCae;
        public int id;
        public string PaisId;
        public string PermisoId;
        public int id_Concepto; // 1 - Productos  2 - Servicios  3 - Productos y Servicios.

        public System.Collections.Generic.List<DetalleFactura> Detalle = new System.Collections.Generic.List<DetalleFactura>();
        public List<Tributo> DetalleTributo = new List<Tributo>();
        public List<Iva> DetalleIva = new List<Iva>();
        public List<opcional> detalleOpcional = new List<opcional>();
        public List<cbteAsoc> ccbteAsoc = new List<cbteAsoc>();
        public void AddTributo(string Descripcion, double Base_imp, double Alicuota, int id, double importe)
        {
            Tributo item = new Tributo();
            item.Descripcion = Descripcion;
            item.Base_Imponible = Base_imp;
            item.Alicuota = Alicuota;
            item.id = id;
            item.Importe = importe;
            this.DetalleTributo.Add(item);

        }

        public void AddIva(string Descripcion, int id, double BaseImponible, double importe)
        {
            Iva item = new Iva();
            item.Descripcion = Descripcion;
            item.id = id;
            item.Importe = importe;
            item.Base_Imponible = BaseImponible;
            this.DetalleIva.Add(item);

        }
        public void AddOpcional(string id, string nombre, string valor)
        {
            opcional item = new opcional();
            item.nombre = nombre;
            item.id = id;
            item.valor = valor;
            this.detalleOpcional.Add(item);

        }
        public void AddCompAsoc(int Tipo_Cbte, int Punto_Emision, int Numero, DateTime fecha, string cuit)
        {
            cbteAsoc item = new cbteAsoc();
            item.Tipo_Cbte = Tipo_Cbte;
            item.Punto_Emision = Punto_Emision;
            item.Numero = Numero;
            item.fecha = fecha;
            item.cuit = cuit;
            this.ccbteAsoc.Add(item);

        }       
        // Detalle de Factura
        public void AddNewDetalle(DetalleFactura item)
        {
            Detalle.Add(item);
        }
        public class DetalleFactura
        {
            public string Codigo;
            public string Descripcion;
            public double Cantidad;
            public int UnidadId;
            public double PrecioUnitario;
            public double PrecioTotal;
        }
    }

    public class Tributo
    {
        public double Base_Imponible;
        public double Alicuota;
        public string Descripcion;
        public int id;
        public double Importe;
    }
    public class opcional
    {
        public string id;
        public string nombre;
        public string valor;
    }
    public class cbteAsoc
    {
        public int Tipo_Cbte;
        public int Punto_Emision;
        public int Numero;
        public DateTime fecha;
        public string cuit;
    }
    public class Iva
    {
        public double Base_Imponible;
        public double Alicuota;
        public string Descripcion;
        public int id;
        public double Importe;
    }
    public enum Tipo_Exportacion
    {
        Exportación_definitiva_de_bienes = 1,
        Servicios = 2,
        Otros = 3
    }
    public enum Idioma_Cbte
    {
        Español = 1,
        Ingles = 2,
        Portugués = 3
    }
    public class TipoCuitExtranjero
    {
        public string _id;
        public string _Descripcion;
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public string Descripcion
        {
            get
            {
                return _Descripcion;
            }
            set
            {
                _Descripcion = value;
            }
        }
        public TipoCuitExtranjero(string id, string Descripcion)
        {
            this.id = id;
            this.Descripcion = Descripcion;
        }
        public TipoCuitExtranjero()
        {
        }

    }

    public enum TipoExportacion
    {
        Exportacion_Definitiva_De_BIenes = 1,
        Servicios = 2,
        Otros = 4
    }
    public class LoginResult
    {
        public LoginResult()
        {
        }
        public LoginResult(bool Result, string Message)
        {
            this.Result = Result;
            this.Message = Message;
        }
        public LoginResult(bool Result, string Message, Ticket Ticket)
        {
            this.Result = Result;
            this.Message = Message;
            this.TicketRequest = Ticket;
        }
        public bool Result = false;
        public string Message;
        public Ticket TicketRequest;
    }
    public class FacResult
    {
        public bool Result;
        public string Message;
        public Factura Factura;
        public int errornumber;
    }

    public class LoteFactura
    {
        public List<Factura> Facturas = new List<Factura>();
        public int Punto_Emision;
        public int Tipo_Cbte;
        public void Add(Factura Factura)
        {
            this.Facturas.Add(Factura);
        }
    }
    public class FacLoteResult
    {
        public string Result;
        public string Message;
        public Factura[] Fac;
        public Factura GetFac(int i)
        {
            return Fac[i];
        }
    }

    public class AutFacturaResult
    {
        public bool Result;
        public string Message;
        public Factura Factura;
        public string Reproceso;
        public int id;

        // Obtiene el mensage de error por el codigo
        public string GetDescricionMotivo(string id)
        {
            string Descripcion = "";
            //Constants.ErrorAfipColection ErrorItems = new Constants.ErrorAfipColection();
            //Constants.ErroAfip ErrorItem;
            //ErrorItem = ErrorItems.Find(id);
            //if (ErrorItem != null)
            //    Descripcion = ErrorItem.Descripcion;
            return Descripcion;
        }
    }
   

public static class Constants
    {
        public static string UrlLogin = "https://wsaa.afip.gov.ar/ws/services/LoginCms";
        public static string UrlService = "https://servicios1.afip.gov.ar/wsfev1/service.asmx?WSDL";
        public static string DnDestino = "cn=wsaa,o=afip,c=ar,serialNumber=CUIT 33693450239";
        enum TipoComprobanteAfip
        {
            Facturas_A = 1,
            Notas_de_Débito_A = 2,
            Notas_de_Crédito_A = 3,
            Recibos_A = 4,
            Notas_de_Venta_al_contado_A = 5,
            Facturas_B = 6,
            Notas_de_Débito_B = 7,
            Notas_de_Crédito_B = 8,
            Recibos_B = 9,
            Notas_de_Venta_al_contado_B = 10,
            Comprobantes_A_3419 = 39,
            Comprobantes_B_3419 = 40,
            Cuenta_Venta_A = 60,
            Cuenta_Venta_B = 61,
            Liquidacion_A = 63,
            Liquidacion_B = 64
        }

        class ErroAfip
        {
            public ErroAfip(string CodigoError, string Descripcion)
            {
                this.CodigoError = CodigoError;
                this.Descripcion = Descripcion;
            }
            public string CodigoError;
            public string Descripcion;
        }

        class ErrorAfipColection
        {
            private List<ErroAfip> _ErrorColection = new List<ErroAfip>();
            public ErrorAfipColection()
            {
                {
                    var withBlock = _ErrorColection;
                    withBlock.Add(new ErroAfip("01", "LA CUIT INFORMADA NO CORRESPONDE A UN RESPONSABLE INSCRIPTO EN EL IVA ACTIVO"));
                    withBlock.Add(new ErroAfip("02", "LA CUIT INFORMADA NO SE ENCUENTRA AUTORIZADA A EMITIR COMPROBANTES ELECTRONICOS ORIGINALES O EL PERIODO DE INICIO AUTORIZADO ES POSTERIOR AL DE LA GENERACION DE LA SOLICITUD"));
                    withBlock.Add(new ErroAfip("03", "LA CUIT INFORMADA REGISTRA INCONVENIENTES CON EL DOMICILIO FISCAL"));
                    withBlock.Add(new ErroAfip("04", "EL PUNTO DE VENTA INFORMADO NO SE ENCUENTRA DECLARADO PARA SER UTILIZADO EN EL PRESENTE REGIMEN"));
                    withBlock.Add(new ErroAfip("05", "LA FECHA DEL COMPROBANTE INDICADA NO PUEDE SER ANTERIOR EN MAS DE CINCO DIAS, SI SE TRATA DE UNA VENTA, O ANTERIOR O POSTERIOR EN MAS DE DIEZ DIAS, SI SE TRATA DE UNA PRESTACION DE SERVICIOS, CONSECUTIVOS DE LA FECHA DE REMISION DEL ARCHIVO    Art. 22 de la RG N° 2177-"));
                    withBlock.Add(new ErroAfip("06", "LA CUIT INFORMADA NO SE ENCUENTRA AUTORIZADA A EMITIR COMPROBANTES CLASE A"));
                    withBlock.Add(new ErroAfip("07", "PARA LA CLASE DE COMPROBANTE SOLICITADO -COMPROBANTE CLASE A- DEBERA CONSIGNAR EN EL CAMPO CODIGO DE DOCUMENTO IDENTIFICATORIO DEL COMPRADOR EL CODIGO 80"));
                    withBlock.Add(new ErroAfip("08", "LA CUIT INDICADA EN EL CAMPO N° DE IDENTIFICACION DEL COMPRADOR ES INVALIDA"));
                    withBlock.Add(new ErroAfip("09", "LA CUIT INDICADA EN EL CAMPO N° DE IDENTIFICACION DEL COMPRADOR NO EXISTE EN EL PADRON UNICO DE CONTRIBUYENTES"));
                    withBlock.Add(new ErroAfip("10", "LA CUIT INDICADA EN EL CAMPO N° DE IDENTIFICACION DEL COMPRADOR NO CORRESPONDE A UN RESPONSABLE INSCRIPTO EN EL IVA ACTIVO"));
                    withBlock.Add(new ErroAfip("11", "EL N° DE COMPROBANTE DESDE INFORMADO NO ES CORRELATIVO AL ULTIMO N° DE COMPROBANTE REGISTRADO/HASTA SOLICITADO PARA ESE TIPO DE COMPROBANTE Y PUNTO DE VENTA"));
                    withBlock.Add(new ErroAfip("12", "EL RANGO INFORMADO SE ENCUENTRA AUTORIZADO CON ANTERIORIDAD PARA LA MISMA CUIT, TIPO DE COMPROBANTE Y PUNTO DE VENTA"));
                    withBlock.Add(new ErroAfip("13", "LA CUIT INDICADA SE ENCUENTRA COMPRENDIDA EN EL REGIMEN ESTABLECIDO POR LA RESOLUCION GENERAL N° 2177 Y/O EN EL TITULO I DE LA RESOLUCION GENERAL N° 1361 ART. 24 DE LA RG N° 2177-"));
                }
            }

            // Busca el error y devuelve la descripcion
            public ErroAfip Find(string id)
            {
                ErroAfip Item = null;
                foreach (var ErroAfip in _ErrorColection)
                {
                    if (id == ErroAfip.CodigoError)
                    {
                        Item = ErroAfip;
                        break;
                    }
                }
                return Item;
            }
        }

        // Convierte una cadena en el formato yyyyMMdd a Date
        public static DateTime ConverStringToDate(string Fecha)
        {
            if (Fecha == "NULL" | string.IsNullOrEmpty(Fecha))
                return DateTime.Now;
            var anio = System.Convert.ToInt32(Fecha.Substring(0,4));
            var dia = System.Convert.ToInt32(Fecha.Substring(6, 2));
            var mes = System.Convert.ToInt32(Fecha.Substring(4, 2));
            return new DateTime(anio, mes, dia);
        }
    }
}
