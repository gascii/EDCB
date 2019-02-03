﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpgTimer
{
    public class ServiceItem : SelectableItem
    {
        public EpgServiceInfo ServiceInfo { get; set; }
        public UInt64 ID
        {
            get { return ServiceInfo.Create64Key(); }
        }
        public String ServiceName
        {
            get { return ServiceInfo.service_name; }
        }
        public ushort SID
        {
            get { return ServiceInfo.SID; }
        }
        public String NetworkName
        {
            get { return CommonManager.ConvertNetworkNameText(ServiceInfo.ONID, true); }
        }
    }
}
