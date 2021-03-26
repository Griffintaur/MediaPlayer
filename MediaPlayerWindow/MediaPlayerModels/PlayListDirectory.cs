using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MediaPlayerModels
{
    [DataContract]
  public  class PlayListDirectory
    {
        
      private  List<PlayListClass> playListCollection=new List<PlayListClass>();
        [DataMember]
        public List<PlayListClass> PlayListCollection
        {
            get { return playListCollection; }
            set { playListCollection = value; }
        }
    }
}
