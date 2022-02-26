using System;
using System.Collections.Generic;
using System.Text;

namespace Afip.Services
{
    
    using System.IO;
   
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    public class InspectorBehavior : IEndpointBehavior
    {
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.ClientMessageInspectors.Add(new MyMessageInspector());
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class MyMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            // Para obtener el XML SOAP que se va a enviar al servicio basta con llamar a ToString del mensaje recibido.
            var xmlString = request.ToString();
            // Si necesitaramos acceder a algun campo de los que enviamos en el mensaje, 
            // procedemos a hacer una conversion del mensaje recibido al tipo de dato especifico, asi:
            // Se crea copia del mensaje original.
            var buffer = request.CreateBufferedCopy(int.MaxValue);
            // A partir de la copia se crea un nuevo mensaje, se obtiene el objeto original.
            var copyMessage = buffer.CreateMessage();
            // var msg = copyMessage.GetBody<MiTipoDeRequest>();
            // msg es ahora un objeto perfectamente tipado, del tipo "MiTipoDeRequest"
            // TODO: Hacer algo con el mensaje.
            // Como los mensajes son de un solo uso, se debe reiniciar el valor de "request" con una nueva copia del mensaje.
            request = buffer.CreateMessage();
            string patchlogfile = "c:/tmp/";
            string sufnamelogfile = System.DateTime.Now.ToString("ddmm_hhmmss");
            var namefile = patchlogfile + "Request" + sufnamelogfile + ".txt";
            var fileWriter = new StreamWriter(namefile);
            fileWriter.WriteLine(copyMessage);
            fileWriter.Flush();
            fileWriter.Close();

            buffer.Close();
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Para obtener el XML SOAP que se recibio desde el servicio basta con llamar a ToString del mensaje recibido.
            // Dim xmlString = reply.ToString
            // Si necesitaramos acceder a algun campo de los recibidos en el mensaje, 
            // procedemos a hacer una conversion del mensaje recibido al tipo de dato especifico, asi:
            // Se crea copia del mensaje original.
            // Dim buffer = reply.CreateBufferedCopy(Integer.MaxValue)
            // A partir de la copia se crea un nuevo mensaje, se obtiene el objeto original.
            // Dim copyMessage = buffer.CreateMessage
            // msg es ahora un objeto perfectamente tipado, del tipo "MiTipoDeResponse"
            // TODO: Hacer algo con el mensaje.
            // Como los mensajes son de un solo uso, se debe reiniciar el valor de "reply" con una nueva copia del mensaje.
            // reply = buffer.CreateMessage
            string patchlogfile = "c:/tmp/";
            string sufnamelogfile = System.DateTime.Now.ToString("ddmm_hhmmss");
            var namefile = patchlogfile + "Response" + sufnamelogfile + ".txt";
            var fileWriter = new StreamWriter(namefile);
            fileWriter.WriteLine(reply.ToString());
            fileWriter.Flush();
            fileWriter.Close();
        }
    }
}
