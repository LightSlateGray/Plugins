﻿using AnyStatus.API;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

//todo: unautherized state when 401
//https://www.visualstudio.com/en-us/docs/integrate/api/rm/contracts#ReleaseStatus

namespace AnyStatus
{
    [DisplayName("VSTS Release")]
    [DisplayColumn("Continuous Integration")]
    [Description("Visual Studio Team Services - Release Status and Notifications")]
    public class VSTSRelease_v1 : VstsPlugin, IHealthCheck, ISchedulable, IWebPage
    {
        private const string Category = "Release Definition";

        public VSTSRelease_v1() : base(aggregate: true) { }

        [Required]
        [Category(Category)]
        [PropertyOrder(40)]
        [DisplayName("Relese Definition Name")]
        [Description("Required (case-sensitive). Enter your Visual Studio Team Services release definition name.")]
        public string ReleaseDefinitionName { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public long? DefinitionId { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public string URL => $"https://{Account}.visualstudio.com/{Uri.EscapeDataString(Project)}/_release?definitionId={DefinitionId}&_a=releases";

        public override object Clone()
        {
            var clone = (VSTSRelease_v1)base.Clone();

            clone.DefinitionId = null;

            return clone;
        }
    }
}