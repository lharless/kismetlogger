﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.2032
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=1.1.4322.2032.
// 
using System;
using System.Xml.Serialization;


/// <remarks/>
[System.Xml.Serialization.XmlRootAttribute("gps-run", IsNullable=false)]
public class gpsrun {
    
	[System.Xml.Serialization.XmlElementAttribute("network-file")]
	public string networkfile;

	/// <remarks/>
	[System.Xml.Serialization.XmlAttribute("gps-version")]
	public System.SByte gpsversion;
    
	/// <remarks/>
	//[System.Xml.Serialization.XmlIgnoreAttribute()]
	//public bool gpsversionSpecified;
    
	/// <remarks/>
	[System.Xml.Serialization.XmlAttribute("start-time")]
	public string starttime;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("gps-point", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public GpsrunGpspoint[] Items;
}

/// <remarks/>


/// <remarks/>
public class GpsrunGpspoint {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string bssid;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("time-sec")]
    public Decimal timesec;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute("time-usec")]
    public string timeusec;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.Decimal lat;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public Decimal lon;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public Decimal alt;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public Decimal spd;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public Decimal heading;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string fix;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool fixSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public Decimal signal;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.Decimal quality;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool qualitySpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public Decimal noise;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string source;
}
