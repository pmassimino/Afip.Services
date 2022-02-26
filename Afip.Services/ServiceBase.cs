using System;

namespace Afip.Services
{
    using System;    
    using System.IO;    
    using System.Threading.Tasks;
    using System.Xml;
    using Afip.Services.Model;
    using Afip.Services.Reference.WSAA;

    public class ServiceBase
    {
        private string _urlLogin;
        private string _urlServicio;
        public LoginCMSClient wsaa;
        public  string pathTicketResponse = @"c:\sae\";
        public bool Inicializado = false;
        private EmpresaInfo _Empresa;
        private string DEFAULT_DNDESTINO;
        private string DEFAULT_SERVICIO = "wsctg";
        public Ticket _Ticket;
        private bool _AuditMessage;

        private int _closeTimeOut = 3;
        private int _openTimeOut = 3;
        private int _receiveTimeOut = 3;
        private int _sendTimeOut = 3;

        // //Url Der servicio de Facturación eletrónica
        public string UrlServicio
        {
            get
            {
                return _urlServicio;
            }
            set
            {
                _urlServicio = value;
            }
        }
        public string NombreServicio
        {
            get
            {
                return DEFAULT_SERVICIO;
            }
            set
            {
                DEFAULT_SERVICIO = value;
            }
        }
        public string PathTicket
        {
            get
            {
                return pathTicketResponse;
            }
            set
            {
                pathTicketResponse = value;
            }
        }
        // //Url Der servicio de Login
        public string UrlLogin
        {
            get
            {
                return _urlLogin;
            }
            set
            {
                _urlLogin = value;
            }
        }
        // //DN Destino
        public string DNDestino
        {
            get
            {
                return DEFAULT_DNDESTINO;
            }
            set
            {
                DEFAULT_DNDESTINO = value;
            }
        }
        // //Datos de La empresa 
        public EmpresaInfo Empresa
        {
            get
            {
                return _Empresa;
            }
            set
            {
                _Empresa = value;
            }
        }


        public bool isLogin
        {
            get
            {
                if (_Ticket == null)
                    return false;
                else if (_Ticket.ExpirationTime > DateTime.Now)
                    return true;
                else
                    return false;
            }
        }

        public LoginResult Login()
        {
            if (this.Inicializado == false)
                this.Inicializar();
            string TicketRequest;
            string TicketResponse = this.ReadFileTicket();
            LoginResult LoginResult = new LoginResult();
            // evaluar ticket guarddo
            if (!string.IsNullOrEmpty(TicketResponse))
            {
                this.ExtraerTicket(TicketResponse);
                // validar expirqción
                if (_Ticket.ExpirationTime > DateTime.Now)
                    LoginResult = new LoginResult(true, "", _Ticket);
            }
            if (LoginResult.Result == false)
            {
                TicketRequest = this.GenerarLoginRequest();
                try
                {
                    TicketResponse =  wsaa.loginCms(TicketRequest);
                    this.ExtraerTicket(TicketResponse);
                    LoginResult = new LoginResult(true, "", _Ticket);
                    // Grabar ticket response para reutilizar
                    this.SaveFileTicket(TicketResponse);
                }
                catch (Exception ex)
                {
                    LoginResult = new LoginResult(false, ex.Message);
                }
            }


            return LoginResult;
        }
        // //Auditar
        public bool AuditMessage
        {
            get
            {
                return _AuditMessage;
            }
            set
            {
                _AuditMessage = value;
            }
        }
        public int closeTimeOut
        {
            get
            {
                return _closeTimeOut;
            }
            set
            {
                _closeTimeOut = value;
            }
        }
        public int openTimeOut
        {
            get
            {
                return _openTimeOut;
            }
            set
            {
                _openTimeOut = value;
            }
        }
        public int receiveTimeOut
        {
            get
            {
                return _receiveTimeOut;
            }
            set
            {
                _receiveTimeOut = value;
            }
        }
        public int sendTimeOut
        {
            get
            {
                return _sendTimeOut;
            }
            set
            {
                _sendTimeOut = value;
            }
        }


        public virtual void Inicializar()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }





        public ServiceBase()
        {
            this._urlLogin = Constants.UrlLogin;
            this._urlServicio = Constants.UrlService;
            this.DEFAULT_DNDESTINO = Constants.DnDestino;
        }

        public ServiceBase(string urlLogin, string urlServicio)
        {
            this._urlLogin = urlLogin;
            this._urlServicio = urlServicio;
        }

        public ServiceBase(EmpresaInfo Empresa)
        {
            this._Empresa = Empresa;
        }



        public void ExtraerTicket(string TicketResponse)
        {
            XmlDocument XmlLoginTicketResponse;
            XmlLoginTicketResponse = new XmlDocument();
            XmlLoginTicketResponse.LoadXml(TicketResponse);
            _Ticket = new Ticket();
            _Ticket.Source = XmlLoginTicketResponse.SelectSingleNode("//source").InnerText;
            _Ticket.Destination = XmlLoginTicketResponse.SelectSingleNode("//destination").InnerText;
            _Ticket.UniqueId = UInt32.Parse(XmlLoginTicketResponse.SelectSingleNode("//uniqueId").InnerText);
            _Ticket.GenerationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//generationTime").InnerText);
            _Ticket.ExpirationTime = DateTime.Parse(XmlLoginTicketResponse.SelectSingleNode("//expirationTime").InnerText);
            _Ticket.Sign = XmlLoginTicketResponse.SelectSingleNode("//sign").InnerText;
            _Ticket.Token = XmlLoginTicketResponse.SelectSingleNode("//token").InnerText;
        }

        // Genera el Ticket Request para auntentificarme al web service
        public string GenerarLoginRequest()
        {
            string DEFAULT_DNORIGEN = "serialNumber=CUIT " + this.Empresa.Cuit.ToString(); // + ",cn=pablo massimino,ou=desarrollo,o=soltec,st=cordoba,c=ar"
                                                                                         // Dim DEFAULT_DNDESTINO As String = My.Settings.DnDestino '"cn=wsaa,o=afip,c=ar,serialNumber=CUIT 33693450239"
            string DEFAULT_CERTSIGNER = this.Empresa.PathCertificado;
            LoginTicketHelper aut = new LoginTicketHelper();
            string TicketRequest;
            TicketRequest = aut.ObtenerLoginTicketRequest(DEFAULT_DNORIGEN, DEFAULT_SERVICIO, DEFAULT_DNDESTINO, DEFAULT_CERTSIGNER);
            return TicketRequest;
        }

        public bool SaveFileTicket(string ticket)
        {
            var namefile = this.pathTicketResponse + "ticketResponse" + this.Empresa.Cuit.ToString().Trim() + this.NombreServicio + ".txt";
            var fileWriter = new StreamWriter(namefile);
            fileWriter.WriteLine(ticket);
            fileWriter.Flush();
            fileWriter.Close();
            return true;
        }
        public string ReadFileTicket()
        {
            string fileReader = "";
            var namefile = this.pathTicketResponse + "ticketResponse" + this.Empresa.Cuit.ToString().Trim() + this.NombreServicio + ".txt";
            try
            {
                fileReader = File.ReadAllText(namefile);
            }
            catch (Exception ex)
            {
            }
            return fileReader;
        }
    }
}
