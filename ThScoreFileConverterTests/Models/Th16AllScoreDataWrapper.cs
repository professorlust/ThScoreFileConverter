﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ThScoreFileConverter.Models;

namespace ThScoreFileConverterTests.Models
{
    // NOTE: Setting the accessibility as public causes CS0050, CS0051 and CS0053.
    internal sealed class Th16AllScoreDataWrapper
    {
        private static Type ParentType = typeof(Th16Converter);
        private static string AssemblyNameToTest = ParentType.Assembly.GetName().Name;
        private static string TypeNameToTest = ParentType.FullName + "+AllScoreData";

        private readonly PrivateObject pobj = null;

        public Th16AllScoreDataWrapper()
            => this.pobj = new PrivateObject(AssemblyNameToTest, TypeNameToTest);

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public object Target
            => this.pobj.Target;

        public Th095HeaderWrapper<Th16Converter> Header
        {
            get
            {
                var header = this.pobj.GetProperty(nameof(Header));
                return (header != null) ? new Th095HeaderWrapper<Th16Converter>(header) : null;
            }
        }

        // NOTE: Th16Converter.ClearData is a private class.
        // public IReadOnlyDictionary<CharaWithTotal, ClearData> ClearData
        //     => this.pobj.GetProperty(nameof(ClearData)) as Dictionary<CharaWithTotal, ClearData>;
        public object ClearData
            => this.pobj.GetProperty(nameof(ClearData));
        public int? ClearDataCount
            => this.ClearData.GetType().GetProperty("Count").GetValue(this.ClearData) as int?;
        public Th16ClearDataWrapper ClearDataItem(Th16Converter.CharaWithTotal chara)
            => new Th16ClearDataWrapper(
                this.ClearData.GetType().GetProperty("Item").GetValue(this.ClearData, new object[] { chara }));

        public Th128StatusWrapper<Th16Converter> Status
        {
            get
            {
                var status = this.pobj.GetProperty(nameof(Status));
                return (status != null) ? new Th128StatusWrapper<Th16Converter>(status) : null;
            }
        }

        public void Set(Th095HeaderWrapper<Th16Converter> header)
            => this.pobj.Invoke(nameof(Set), new object[] { header.Target }, CultureInfo.InvariantCulture);
        public void Set(Th16ClearDataWrapper data)
            => this.pobj.Invoke(nameof(Set), new object[] { data.Target }, CultureInfo.InvariantCulture);
        public void Set(Th128StatusWrapper<Th16Converter> status)
            => this.pobj.Invoke(nameof(Set), new object[] { status.Target }, CultureInfo.InvariantCulture);
    }
}
