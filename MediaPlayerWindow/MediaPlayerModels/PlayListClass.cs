using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace MediaPlayerModels
{   
    [DataContract]
    public class PlayListClass
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Filepath { get; set; }
    }
}
