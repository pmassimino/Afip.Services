using System;
using System.Collections.Generic;
using System.Text;

namespace Afip.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography.Pkcs;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Xml;
    
    public class LoginTicketHelper
    {
        public XmlDocument XmlLoginTicketRequest = null/* TODO Change to default(_) if this is not a reference type */;
        public XmlDocument XmlLoginTicketResponse = null/* TODO Change to default(_) if this is not a reference type */;
        public string RutaDelCertificadoFirmante;
        // Public XmlStrLoginTicketRequestTemplate As String = "<loginTicketRequest><header><source></source><destination>cn=wsaahomo,o=afip,c=ar,serialNumber=CUIT 33693450239</destination><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>"
        public string XmlStrLoginTicketRequestTemplate = "<loginTicketRequest><header><destination>cn=wsaahomo,o=afip,c=ar,serialNumber=CUIT 33693450239</destination><uniqueId></uniqueId><generationTime></generationTime><expirationTime></expirationTime></header><service></service></loginTicketRequest>";
        private static UInt32 _globalUniqueID = 0; // OJO! NO ES THREAD-SAFE

        /// <summary>
        ///         ''' Construye un Login Ticket obtenido del WSAA
        ///         ''' </summary>
        ///         ''' <param name="argOrigenDn">DN origen (DN de la computadora que hace requerimiento)</param>
        ///         ''' <param name="argServicio">Servicio al que se desea acceder</param>
        ///         ''' <param name="argUrlWsaa">URL del WSAA</param>
        ///         ''' <param name="argDestinoDn">DN destino (DN del WSAA)</param>
        ///         ''' <param name="argRutaCertX509Firmante">Ruta del certificado X509 (con clave privada) usado para firmar</param>
        ///         ''' <param name="argVerbose">Nivel detallado de descripcion? true/false</param>
        ///         ''' <remarks></remarks>
        public string ObtenerLoginTicketRequest(string argOrigenDn, string argServicio, string argDestinoDn, string argRutaCertX509Firmante)
        {
            this.RutaDelCertificadoFirmante = argRutaCertX509Firmante;


            string cmsFirmadoBase64;
            XmlNode xmlNodoSource;
            XmlNode xmlNodoDestination;
            XmlNode xmlNodoUniqueId;
            XmlNode xmlNodoGenerationTime;
            XmlNode xmlNodoExpirationTime;
            XmlNode xmlNodoService;

            // PASO 1: Genero el Login Ticket Request
            try
            {
                XmlLoginTicketRequest = new XmlDocument();
                XmlLoginTicketRequest.LoadXml(XmlStrLoginTicketRequestTemplate);

                // xmlNodoSource = XmlLoginTicketRequest.SelectSingleNode("//source")
                xmlNodoDestination = XmlLoginTicketRequest.SelectSingleNode("//destination");
                xmlNodoUniqueId = XmlLoginTicketRequest.SelectSingleNode("//uniqueId");
                xmlNodoGenerationTime = XmlLoginTicketRequest.SelectSingleNode("//generationTime");
                xmlNodoExpirationTime = XmlLoginTicketRequest.SelectSingleNode("//expirationTime");
                xmlNodoService = XmlLoginTicketRequest.SelectSingleNode("//service");

                xmlNodoGenerationTime.InnerText = DateTime.Now.AddMinutes(-10).ToString("s");
                xmlNodoExpirationTime.InnerText = DateTime.Now.AddMinutes(+80).ToString("s");
                // xmlNodoSource.InnerText = argOrigenDn
                xmlNodoDestination.InnerText = argDestinoDn;
                xmlNodoUniqueId.InnerText = System.Convert.ToString(_globalUniqueID);
                xmlNodoService.InnerText = argServicio;

                _globalUniqueID += 1;
            }

            catch (Exception excepcionAlGenerarLoginTicketRequest)
            {
                throw new Exception("***Error GENERANDO el LoginTicketRequest : " + excepcionAlGenerarLoginTicketRequest.Message);
            }

            // PASO 2: Firmo el Login Ticket Request
            try
            {


                // PARA OBTENER EL CERTIFICADO DEL CERTSTORE MY DE WINDOWS DES-COMENTAR SIGUIENTE LINEA
                // Dim certFirmante As X509Certificate2 = CertAuthentication.ObtieneCertificadoDelCertStoreMy(argOrigenDn)

                // Obtengo el certificado que voy a usar para firmar el mensaje
                // Leo el certificado de disco

                X509Certificate2 certFirmante = CertificadosX509Lib.ObtieneCertificadoDesdeArchivo(RutaDelCertificadoFirmante);


                // Convierto el login ticket request a bytes, para firmar
                Encoding EncodedMsg = Encoding.UTF8;
                byte[] msgBytes = EncodedMsg.GetBytes(XmlLoginTicketRequest.OuterXml);

                // Firmo el msg y paso a Base64
                byte[] encodedSignedCms = CertificadosX509Lib.FirmaBytesMensaje(msgBytes, certFirmante);
                cmsFirmadoBase64 = Convert.ToBase64String(encodedSignedCms);
            }
            catch (Exception excepcionAlFirmar)
            {
                throw new Exception("***Error FIRMANDO el LoginTicketRequest : " + excepcionAlFirmar.Message);
            }


            return cmsFirmadoBase64;
        }
    }

    /// <summary>
    ///     ''' Libreria de utilidades para manejo de certificados
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    class CertificadosX509Lib
    {

        /// <summary>
        ///         ''' Lee un certificado del repositorio My 
        ///         ''' </summary>
        ///         ''' <param name="argDnCertificado">DN del certificado a buscar</param>
        ///         ''' <returns></returns>
        ///         ''' <remarks></remarks>
        public static X509Certificate2 ObtieneCertificadoDelCertStoreMy(string argDnCertificado
        )
        {

            // Abro el repositorio de certificados en modo lectura
            X509Store storeMy = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            storeMy.Open(OpenFlags.ReadOnly);

            // Muestro los certificados del repositorio


            // Busco el certificado a usar para firmar
            X509Certificate2Collection certificados = storeMy.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, argDnCertificado, false);


            // Si el certificado no fue hallado, lanzo excepcion...
            if (certificados.Count == 0)
            {
                throw new Exception("***No encontré el certificado: " + argDnCertificado);
                storeMy.Close();
                return null/* TODO Change to default(_) if this is not a reference type */;
            }

            storeMy.Close();
            // Respondo el primer certificado encontrado (por si hay mas de uno :-))
            return certificados[0];
        }

        /// <summary>
        ///         ''' Firma mensaje
        ///         ''' </summary>
        ///         ''' <param name="argBytesMsg">Bytes del mensaje</param>
        ///         ''' <param name="argCertFirmante">Certificado usado para firmar</param>
        ///         ''' <returns>Bytes del mensaje firmado</returns>
        ///         ''' <remarks></remarks>
        public static byte[] FirmaBytesMensaje(byte[] argBytesMsg, X509Certificate2 argCertFirmante
        )
        {
            try
            {
                // Pongo el mensaje en un objeto ContentInfo (requerido para construir el obj SignedCms)
                ContentInfo infoContenido = new ContentInfo(argBytesMsg);

                SignedCms cmsFirmado = new SignedCms(infoContenido);

                // Creo objeto CmsSigner que tiene las caracteristicas del firmante
                CmsSigner cmsFirmante = new CmsSigner(argCertFirmante);
                cmsFirmante.IncludeOption = X509IncludeOption.EndCertOnly;
                // Firmar con algoritmo SHA-2
                // cmsFirmante.DigestAlgorithm = New Oid("2.16.840.1.101.3.4.2.1")
                // Firmo el mensaje PKCS #7
                cmsFirmado.ComputeSignature(cmsFirmante);

                // Encodeo el mensaje PKCS #7.
                return cmsFirmado.Encode();
            }
            catch (Exception excepcionAlFirmar)
            {
                throw new Exception("***Error al firmar: " + excepcionAlFirmar.Message);
                return null;
            }
        }

        /// <summary>
        ///         ''' Lee certificado de disco
        ///         ''' </summary>
        ///         ''' <param name="argArchivo">Ruta del certificado a leer.</param>
        ///         ''' <returns>Un objeto certificado X509</returns>
        ///         ''' <remarks></remarks>
        public static X509Certificate2 ObtieneCertificadoDesdeArchivo(string argArchivo
        )
        {
            X509Certificate2 objCert = new X509Certificate2();
            try
            {
                //objCert.Import(System.IO.File.ReadAllBytes(argArchivo));
                var x509 = new X509Certificate2(File.ReadAllBytes(argArchivo));
                return x509;
                //return objCert;
            }
            catch (Exception excepcionAlImportarCertificado)
            {
                throw new Exception(excepcionAlImportarCertificado.Message + " " + excepcionAlImportarCertificado.StackTrace);
                return null/* TODO Change to default(_) if this is not a reference type */;
            }
        }
    }
}
