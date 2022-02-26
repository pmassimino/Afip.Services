using Afip.Services.Model;
using Afip.Services.Reference.WSFEV1;
using System;
using System.Collections.Generic;
using System.Text;

namespace Afip.Services
{
    static class WSFEVHelper
    {
        // Contruye un ClsFEXRequest apartir de un FactInfo
        public static FECAERequest BuidFEXRequest(Factura Fac)
        {
            FECAERequest FacRequest = new FECAERequest();
            FECAECabRequest Cabecera = new FECAECabRequest();
            // Cabecera
            Cabecera.CantReg = 1;
            Cabecera.CbteTipo = Fac.Tipo_Cbte;
            Cabecera.PtoVta = Fac.Punto_Emision;
            FacRequest.FeCabReq = Cabecera;
            // Detalle Lote
            FECAEDetRequest[] Facturas = new FECAEDetRequest[1];
            Facturas[0] = BuildFactura(Fac);
            FacRequest.FeDetReq = Facturas;
            return FacRequest;
        }
        // Contruye un ClsFEXRequest apartir de un Lote
        public static FECAERequest BuidFEXRequest(LoteFactura Lote)
        {
            FECAERequest FacRequest = new FECAERequest();
            FECAECabRequest Cabecera = new FECAECabRequest();
            // Cabecera
            Cabecera.CantReg = Lote.Facturas.Count;
            Cabecera.CbteTipo = Lote.Tipo_Cbte;
            Cabecera.PtoVta = Lote.Punto_Emision;
            FacRequest.FeCabReq = Cabecera;
            // Detalle Lote
            FECAEDetRequest[] Facturas = new FECAEDetRequest[Lote.Facturas.Count - 1 + 1];
            int i = 0;
            foreach (var Item in Lote.Facturas)
            {
                Facturas[i] = BuildFactura(Item);
                i += 1;
            }
            FacRequest.FeDetReq = Facturas;
            return FacRequest;
        }
        public static FECAEDetRequest BuildFactura(Factura Fac)
        {
            FECAEDetRequest result = new FECAEDetRequest();
            result.CbteDesde = Fac.Numero;
            result.CbteHasta = Fac.Numero;
            result.Concepto = Fac.id_Concepto;
            result.DocNro = Fac.Numero_Doc;
            result.DocTipo = Fac.Tipo_Doc;

            // importes
            result.ImpIVA = Fac.Importe_Iva;
            result.ImpNeto = Fac.Importe_Neto;
            result.ImpOpEx = Fac.Importe_Exento;
            result.ImpTotal = Fac.Importe_Total;
            result.ImpTotConc = Fac.Importe_CNG;
            result.ImpTrib = Fac.Importe_OTributos;
            result.MonId = string.IsNullOrEmpty(Fac.MondedaId) ? "PES" : Fac.MondedaId;
            result.MonCotiz = Fac.MonedaCotizacion == 0 ? 1 : Fac.MonedaCotizacion;
            Afip.Services.Reference.WSFEV1.Tributo[] pTributos = new Afip.Services.Reference.WSFEV1.Tributo[Fac.DetalleTributo.Count - 1 + 1];
            int i = 0;

            foreach (var Item in Fac.DetalleTributo)
            {
                Afip.Services.Reference.WSFEV1.Tributo itemt = new Afip.Services.Reference.WSFEV1.Tributo();
                itemt.Desc = Item.Descripcion;
                itemt.Importe = Item.Importe;
                itemt.Id = Convert.ToInt16(Item.id);
                itemt.Alic = Item.Alicuota;
                itemt.BaseImp = Item.Base_Imponible;
                pTributos[i] = itemt;
                i += 1;
            }

            if (pTributos.Length > 0)
                result.Tributos = pTributos;
            result.CbteFch = Fac.Fecha_cbte.ToString("yyyyMMdd");
            if (Fac.id_Concepto != 1)
            {
                result.FchServDesde = Fac.Fecha_cbte.ToString("yyyyMMdd");
                result.FchServHasta = Fac.Fecha_cbte.ToString("yyyyMMdd");
                result.FchVtoPago = Fac.Fecha_Vencimiento.ToString("yyyyMMdd");
            }
            // Factura d Crédito MYPyme - informar solo para facturas
            if (Array.IndexOf(new int[] { 201, 206, 211 }, Fac.Tipo_Cbte) != -1)
                result.FchVtoPago = Fac.Fecha_Vencimiento.ToString("yyyyMMdd");

            Opcional[] pOpcional = new Opcional[Fac.detalleOpcional.Count - 1 + 1];
            i = 0;
            foreach (var Item in Fac.detalleOpcional)
            {
                Opcional itemO = new Opcional();
                itemO.Id = Item.id;
                itemO.Valor = Item.valor;
                pOpcional[i] = itemO;
                i += 1;
            }
            if (pOpcional.Length > 0)
                result.Opcionales = pOpcional;
            // Comprobantes Asociados
            CbteAsoc[] pcbteAsoc = new CbteAsoc[Fac.ccbteAsoc.Count - 1 + 1];
            i = 0;
            foreach (var Item in Fac.ccbteAsoc)
            {
                CbteAsoc itemC = new CbteAsoc();
                itemC.Tipo = Item.Tipo_Cbte;
                itemC.PtoVta = Item.Punto_Emision;
                itemC.Nro = Item.Numero;
                itemC.Cuit = Item.cuit;
                itemC.CbteFch = Item.fecha.ToString("yyyyMMdd");
                pcbteAsoc[i] = itemC;
                i += 1;
            }
            if (pcbteAsoc.Length > 0)
                result.CbtesAsoc = pcbteAsoc;
            result.CbteDesde = Fac.Numero;
            result.CbteHasta = Fac.Numero;
            // Datos del Cliente

            // importes
            result.ImpTotal = Fac.Importe_Total;
            result.ImpIVA = Fac.Importe_Iva;
            result.ImpNeto = Fac.Importe_Neto;
            result.ImpOpEx = Fac.Importe_Exento;
            result.ImpTotConc = Fac.Importe_CNG;
            result.ImpTrib = Fac.Importe_OTributos;


            // IVA
            AlicIva[] pIva = new AlicIva[Fac.DetalleIva.Count - 1 + 1];
            i = 0;
            foreach (var Item in Fac.DetalleIva)
            {
                AlicIva itemi = new AlicIva();
                itemi.BaseImp = Item.Base_Imponible;
                itemi.Id = Item.id;
                itemi.Importe = Item.Importe;
                pIva[i] = itemi;
                i += 1;
            }
            if (pIva.Length > 0)
                result.Iva = pIva;

            return result;
        }
        // Contruye un FactInfo apartir de un ClsFEXRequest
        public static FacResult BuidFacExResult(FECAEResponse Response, System.Exception exp)
        {
            FacResult FacResult = new FacResult();
            var FacResponse = Response.FeCabResp;
            var FacDetalle = Response.FeDetResp;
            int lengdet = 0;
            if (FacDetalle != null)
                lengdet = FacDetalle.Length;
            Factura Factura = new Factura();

            if (FacResponse != null)
            {
                FacResult.Result = FacResponse.Resultado == "A" ? true : false;
                if (FacDetalle != null)
                {
                    int i = 0;
                    foreach (var Item in FacDetalle)
                    {
                        Factura.cae = Item.CAE;
                        Factura.FechaVencimientoCae = Constants.ConverStringToDate(Item.CAEFchVto);
                        Factura.Fecha_cbte = Constants.ConverStringToDate(Item.CbteFch);
                        Factura.Numero_Doc = Item.DocNro;
                        Factura.Punto_Emision = FacResponse.PtoVta;
                        Factura.Numero = Item.CbteHasta;
                        Factura.id_Concepto = Item.Concepto;
                        Factura.Tipo_Doc = Item.DocTipo;
                        Factura.Numero_Doc = Item.DocNro;
                        if (Item.Observaciones != null)
                        {
                            foreach (var itemo in Item.Observaciones)
                                Factura.Observaciones += itemo.Code.ToString() + "-" + itemo.Msg;
                        }
                        if (Item.Resultado == "R")
                        {
                            FacResult.Result = false;
                            FacResult.Message += Factura.Observaciones;
                        }
                        i += 1;
                    }
                    if (FacResponse.Resultado == "R")
                    {
                        if (Response.Errors != null)
                        {
                            foreach (var iteme in Response.Errors)
                                FacResult.Message = iteme.Code.ToString() + "-" + iteme.Msg;
                            FacResult.Result = false;
                        }
                    }
                }
            }
            // error de conexion
            if (FacResponse == null)
            {
                FacResult.Result = false;
                if (exp != null)
                    FacResult.Message = exp.Message;
            }
            // Error de afip
            if (Response == null)
            {
                if (Response.Errors != null)
                {
                    foreach (var Item in Response.Errors)
                    {
                        FacResult.Message = FacResult.Message + Item.Code.ToString() + "-" + Item.Msg;
                        // Agregar codigo de error
                        FacResult.errornumber = Item.Code;
                    }
                }
            }
            FacResult.Factura = Factura;
            return FacResult;
        }
        // 'Contruye un FactInfo apartir de un ClsFEXRequest
        public static  FacLoteResult BuidLoteFacExResult(FECAEResponse Response, System.Exception exp)
        {
            FacLoteResult Result = new FacLoteResult();

            int lengdet = 0;
            if (Response != null)
            {
                if (Response.FeDetResp != null)
                    lengdet = Response.FeDetResp.Length - 1;
            }
            Factura[] Facturas = new Factura[lengdet + 1];

            if (Response != null)
            {
                // Result.Result = Response.FeCabResp.Resultado
                if (Response.FeDetResp != null)
                {
                    int i = 0;
                    foreach (var Item in Response.FeDetResp)
                    {
                        Factura Factura = new Factura();
                        Factura.cae = Item.CAE;
                        Factura.FechaVencimientoCae = Constants.ConverStringToDate(Item.CAEFchVto);
                        Factura.Fecha_cbte = Constants.ConverStringToDate(Item.CbteFch);
                        Factura.Numero_Doc = Item.DocNro;
                        Factura.Punto_Emision = Response.FeCabResp.PtoVta;
                        Factura.id_Concepto = Item.Concepto;
                        Factura.Tipo_Doc = Item.DocTipo;
                        Factura.Numero_Doc = Item.DocNro;
                        Factura.Numero = Item.CbteHasta;
                        Factura.Punto_Emision = Response.FeCabResp.PtoVta;
                        if (Item.Observaciones != null)
                        {
                            foreach (var itemo in Item.Observaciones)
                                Factura.Observaciones += itemo.Code.ToString() + "-" + itemo.Msg;
                        }
                        if (Item.Resultado == "R")
                            Result.Message += Factura.Observaciones;
                        Facturas[i] = Factura;
                        i += 1;
                    }
                    Result.Result = Response.FeCabResp.Resultado;
                    if (Response.FeCabResp.Resultado == "R")
                    {
                        if (Response.Errors == null)
                        {
                            foreach (var iteme in Response.Errors)
                                Result.Message = iteme.Code.ToString() + "-" + iteme.Msg;
                        }
                    }
                }
            }
            // Error de afip
            if (Response != null)
            {
                if (Response.Errors != null)
                {
                    foreach (var Item in Response.Errors)
                        Result.Message = Result.Message + Item.Code.ToString() + "-" + Item.Msg;
                }
                // error de conexion
                if (Response.FeCabResp == null)
                {
                    Result.Result = "R";
                    if (exp != null)
                        Result.Message = exp.Message;
                }
            }
            Result.Fac = Facturas;
            return Result;
        }
        // Contruye un FactInfo apartir de un ClsFEXRequest para el metodo consultar
        public static FacResult BuidFacExResult(FECompConsultaResponse Response, System.Exception exp)
        {
            Factura Fac = new Factura();
            FacResult FacResult = new FacResult();

            if (Response != null)
            {
                if (Response.ResultGet != null)
                {
                    Fac.Numero = Response.ResultGet.CbteDesde;
                    Fac.Punto_Emision = Response.ResultGet.PtoVta;
                    Fac.Tipo_Cbte = Response.ResultGet.CbteTipo;
                    Fac.Fecha_cbte = Constants.ConverStringToDate(Response.ResultGet.CbteFch);
                    Fac.Fecha_Vencimiento = Constants.ConverStringToDate(Response.ResultGet.FchVtoPago);
                    Fac.cae = Response.ResultGet.CodAutorizacion;

                    // Moneda
                    Fac.MondedaId = Response.ResultGet.MonId;
                    Fac.MonedaCotizacion = Response.ResultGet.MonCotiz;
                    if (Response.ResultGet.Observaciones != null)
                    {
                        foreach (var Item in Response.ResultGet.Observaciones)
                            Fac.Observaciones = Fac.Observaciones + Item.Code.ToString() + "-" + Item.Msg + " ";
                    }
                    // importes
                    Fac.Tipo_Doc = Response.ResultGet.DocTipo;
                    Fac.Numero_Doc = Response.ResultGet.DocNro;
                    Fac.Importe_Total = Response.ResultGet.ImpTotal;
                    Fac.Importe_CNG = Response.ResultGet.ImpTotConc;
                    Fac.Importe_Exento = Response.ResultGet.ImpOpEx;
                    Fac.Importe_Neto = Response.ResultGet.ImpNeto;
                    Fac.Importe_OTributos = Response.ResultGet.ImpTrib;
                    Fac.Importe_Iva = Response.ResultGet.ImpIVA;
                    // Detalle Iva
                    if (Response.ResultGet.Iva != null)
                    {
                        foreach (var Item in Response.ResultGet.Iva)
                            Fac.AddIva("IVA", Item.Id, Item.BaseImp, Item.Importe);
                    }
                    // Detalle Otros Tributos
                    if (Response.ResultGet.Tributos != null)
                    {
                        foreach (var Item in Response.ResultGet.Tributos)
                            Fac.AddTributo("OTRO TRIBUTO", Item.BaseImp, Item.Alic, Item.Id, Item.Importe);
                    }
                    FacResult.Factura = Fac;
                    // Detalle de factura
                    if (Response.ResultGet.Resultado == "A")
                        FacResult.Result = true;
                    else
                        FacResult.Result = false;
                }
                if (Response.Errors != null)
                {
                    foreach (var Item in Response.Errors)
                    {
                        FacResult.errornumber = Item.Code;
                        FacResult.Message = FacResult.Message + Item.Msg;
                    }
                }
            }

            return FacResult;
        }
    }
}
