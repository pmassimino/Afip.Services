using System;
using System.Collections.Generic;
using System.Text;

namespace Afip.Services
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
    using System.ServiceModel;
    using System.Text;
    using System.Threading.Tasks;
    using Afip.Services.Model;
    using Afip.Services.Reference.WSAA;
    using Afip.Services.Reference.WSFEV1;
    using Microsoft.VisualBasic;

    public class FacturaWebService_V1 : ServiceBase
    {
        private ServiceSoapClient ws;
        private FEAuthRequest _AutRequest;       



        public FEAuthRequest AutRequest
        {
            get
            {
                if (_AutRequest == null & _Ticket != null)
                {
                    _AutRequest = new FEAuthRequest();
                    _AutRequest.Cuit = Empresa.Cuit;
                    _AutRequest.Sign = _Ticket.Sign;
                    _AutRequest.Token = _Ticket.Token;
                }
                return _AutRequest;
            }
        }

        public override void Inicializar()
        {
            base.Inicializar();
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            EndpointAddress AddressLogin = new EndpointAddress(this.UrlLogin);
            EndpointAddress AddressService = new EndpointAddress(this.UrlServicio);

            binding.MaxReceivedMessageSize = 65536999;
            binding.MaxBufferSize = 65536999;
            binding.CloseTimeout = TimeSpan.FromMinutes(this.closeTimeOut);
            binding.OpenTimeout = TimeSpan.FromMinutes(this.openTimeOut);
            binding.SendTimeout = TimeSpan.FromMinutes(this.sendTimeOut);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(this.receiveTimeOut);
            this.NombreServicio = "wsfe";
            this.wsaa = new LoginCMSClient(binding, AddressLogin);

            ws = new ServiceSoapClient(binding, AddressService);
            this.Inicializado = true;
            if (this.AuditMessage == true)
                // Codigo Inspector XML
                ws.Endpoint.EndpointBehaviors.Add(new InspectorBehavior());
        }

        // Obtiene el punto de venta
        public FEPtoVentaResponse GetPuntoEmision()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            FEPtoVentaResponse result=null;
            System.Exception exp;
            try
            {
                result = ws.FEParamGetPtosVenta(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result;
        }
       
        // Obtener tipo de comprobante
        public CbteTipo[] GetTipoCbte()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            CbteTipoResponse result=null;
           
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposCbte(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }
        // Obtener tipo de comprobante
        public Moneda[] GetMoneda()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            MonedaResponse result=null;
            
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposMonedas(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }
        // Obtener Pais
        public PaisTipo[] GetPais()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            FEPaisResponse result = null;
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposPaises(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }
        // Obtener Tipos Tributos
        public TributoTipo[] GetTipoTributo()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            FETributoResponse result=null;
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposTributos(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }
        // Obtener Tipos Iva
        public IvaTipo[] GetTipoIva()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            IvaTipoResponse result=null;
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposIva(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }
        // Obtener Tipos Iva
        public DocTipo[] GetTipoDocumento()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            DocTipoResponse result=null;
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposDoc(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }
        // Obtener Tipos Conceptos
        public ConceptoTipo[] GetTiposConceptos()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            ConceptoTipoResponse result=null;
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposConcepto(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }
        // Obtener Tipos Conceptos
        public OpcionalTipo[] GetOpcionalTipo()
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            OpcionalTipoResponse result=null;
            System.Exception exp;
            try
            {
                result = ws.FEParamGetTiposOpcional(this.AutRequest);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return result.ResultGet;
        }

        // Obtener Ultimo Comprobante autorizado
        public int GetUltimoNumeroComprobante(int pe, int Cbte)
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            FERecuperaLastCbteResponse result;
            int resultFinal = 0;
            System.Exception exp;
            try
            {
                result = ws.FECompUltimoAutorizado(this.AutRequest, pe, Cbte);
                resultFinal = result.CbteNro;
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return resultFinal;
        }
        // 'Chequea que este disponible el servicio
        public bool IsConnect()
        {
            bool _Connect = false;
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            if (isLogin)
            {
                try
                {
                    var result = ws.FEDummy();
                    if (result.AppServer == "OK")
                        _Connect = true;
                }
                catch (Exception ex)
                {
                }
            }
            return _Connect;
        }

        // Autorizar factura
        public FacResult Autorizar(Factura Factura)
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            FECAEResponse result=null;
            FacResult FacResult;
            System.Exception exp=null;
            FECAERequest request = WSFEVHelper.BuidFEXRequest(Factura);

            try
            {
                result = ws.FECAESolicitar(this.AutRequest, request);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            FacResult = WSFEVHelper.BuidFacExResult(result, exp);
            return FacResult;
        }
        // Autorizar Lote de Factura
        public FacLoteResult Autorizar(LoteFactura Lote)
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            var fac = Lote.Facturas[0];
            FECAEResponse result=null;
            FacLoteResult FacResult;
            System.Exception exp=null;
            FECAERequest request = WSFEVHelper.BuidFEXRequest(Lote);
            try
            {
                result = ws.FECAESolicitar(this.AutRequest, request);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            FacResult = WSFEVHelper.BuidLoteFacExResult(result, exp);
            return FacResult;
        }

        // Autorizar factura
        public FacResult GetComprobante(int id_Tipo, int PuntoVenta, int Numero)
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            FECompConsultaResponse result=null;
            FacResult FacResult;
            System.Exception exp=null;
            FECompConsultaReq request = new FECompConsultaReq();
            request.CbteTipo = id_Tipo;
            request.CbteNro = Numero;
            request.PtoVta = PuntoVenta;

            try
            {
                result = ws.FECompConsultar(this.AutRequest, request);
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            Factura Factura = new Factura();
            FacResult = WSFEVHelper.BuidFacExResult(result, exp);

            return FacResult;
        }
        // Obtener cotización afip
        public double GetCotizacion(string id_Moneda)
        {
            if (!this.isLogin)
            {
                var resulta = this.Login();
            }
            FECotizacionResponse result;
            double cotiz = 0;            
            System.Exception exp;

            try
            {
                result = ws.FEParamGetCotizacion(this.AutRequest, id_Moneda);
                cotiz = result.ResultGet.MonCotiz;
            }
            catch (Exception ex)
            {
                exp = ex;
            }
            return cotiz;
        }
        

              

      
        
    }

}
