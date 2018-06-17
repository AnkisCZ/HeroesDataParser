﻿using System.IO;
using Xunit;

namespace HeroesData.FileWriter.Tests
{
    public class XmlTests : FileOutputTestBase
    {
        private readonly string DefaultCreatedFile = Path.Combine("output", "xml", "heroesdata.xml");

        [Fact]
        public void XmlWriterNoBuildNumberTest()
        {
            FileOutputNoBuildNumber.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutputTest.xml");
        }

        [Fact]
        public void XmlWriterHasBuildNumberTest()
        {
            FileOutputHasBuildNumber.CreateXml();
            CompareFile(Path.Combine("output", "xml", $"heroesdata_{BuildNumber}.xml"), "XmlOutputTest.xml");
        }

        [Fact]
        public void XmlWriterNoCreateTest()
        {
            if (File.Exists(DefaultCreatedFile)) // not really needed
                File.Delete(DefaultCreatedFile);

            FileOutputHasBuildNumber.CreateXml(false);
            Assert.False(File.Exists(DefaultCreatedFile), "heroesdata.xml should not have been created");
        }

        [Fact]
        public void XmlWriterFalseSettingsTest()
        {
            FileOutputFalseSettings.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutputFalseSettingsTest.xml");
        }

        [Fact]
        public void XmlWriterFileSplitTest()
        {
            FileOutputFileSplit.CreateXml();
            CompareFile(Path.Combine("output", "xml", "Alarak.xml"), "Alarak.xml");
            CompareFile(Path.Combine("output", "xml", "Alexstrasza.xml"), "Alexstrasza.xml");
        }

        [Fact]
        public void XmlWriterRawDescriptionTest()
        {
            FileOutputRawDescription.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutput0.xml");
        }

        [Fact]
        public void XmlWriterPlainTextTest()
        {
            FileOutputPlainText.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutput1.xml");
        }

        [Fact]
        public void XmlWriterPlainTextWithNewlinesTest()
        {
            FileOutputPlainTextWithNewlines.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutput2.xml");
        }

        [Fact]
        public void XmlWriterPlainTextWithScalingTest()
        {
            FileOutputPlainTextWithScaling.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutput3.xml");
        }

        [Fact]
        public void XmlWriterPlainTextWithScalingWithNewlinesTest()
        {
            FileOutputPlainTextWithScalingWithNewlines.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutput4.xml");
        }

        [Fact]
        public void XmlWriterColoredTextWithScalingTest()
        {
            FileOutputColoredTextWithScaling.CreateXml();
            CompareFile(DefaultCreatedFile, "XmlOutput6.xml");
        }
    }
}