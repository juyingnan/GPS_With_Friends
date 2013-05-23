﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace GWF_Services
{
    public class GWFMessage
    {
        public string id
        {
            get;
            set;
        }

        public GWFType type
        {
            get;
            set;
        }

        public Object content
        {
            get;
            set;
        }

        public GWFMessage()
        {
            this.type = GWFType.DEFAULT_TYPE;
            this.content = null;
            this.id = "";
        }

        //public void setID(string id)
        //{
        //    this.id = id;
        //}

        //public string getID()
        //{
        //    return this.id;
        //}

        //public void setType(GWFType type)
        //{
        //    this.type = type;
        //}

        //public GWFType getType()
        //{
        //    return this.type;
        //}

        //public void setContent(Object content)
        //{
        //    this.content = content;
        //}

        //public Object getContent()
        //{
        //    return this.content;
        //}

        public string serialize()
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                return Convert.ToBase64String(buffer);
            }
            catch (Exception e)
            {
                throw new Exception("Serialize failed: " + e.Message);
            }
        }

        public static GWFMessage deSerializeiFromString(string obj_str)
        {
            try
            {

                GWFMessage obj;
                IFormatter formatter = new BinaryFormatter();
                byte[] buffer = Convert.FromBase64String(obj_str);
                MemoryStream stream = new MemoryStream(buffer);
                obj = (GWFMessage)formatter.Deserialize(stream);

                stream.Flush();
                stream.Close();

                return obj;
            }
            catch (Exception e)
            {
                throw new Exception("Deserizalize failed: " + e.Message);
                return null;
            }
        }
    }
}