using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Drawing.Text;

namespace bwMonitor
{
    internal class netport
    {
        //atributos
        public long upload { get; set; }
        public long download { get; set; }
        public long bytesSent { get; set; }
        public long bytesReceived { get; set; }
        
        public string intName { get; set; }
        public int intId { get; set; }

        public long prevUpload { get; set; }
        public long prevDownload { get; set; }
        public long newUpload = 0;
        public long newDownload = 0;

        public string ipv4add { get; set; }


        //constructor
        public netport()
        {

        }

        //metodos:

        //mostrar bw subida
        public long uploadbw()
        {
            
            newUpload = bytesSent;
            upload = ((newUpload - prevUpload)*8 / 1000);
            prevUpload = newUpload;
            return upload;

        }

        //mostrar bw bajada
        public long downloadbw()
        {
            newDownload = bytesReceived;
            download = ((newDownload - prevDownload) * 8 /1000);
            prevDownload = newDownload;
            return download;
        }

    }
    
}
