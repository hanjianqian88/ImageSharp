﻿// <copyright file="EncodeJpeg.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Benchmarks.Codecs.Jpeg
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    using BenchmarkDotNet.Attributes;

    using SixLabors.ImageSharp.Formats.Jpeg.LibJpegTurbo;

    using CoreImage = SixLabors.ImageSharp.Image;

    public class EncodeJpeg : BenchmarkBase
    {
        // System.Drawing needs this.
        private Stream bmpStream;
        //private Image bmpDrawing;
        private Image<Rgba32> bmpCore;

        [GlobalSetup]
        public void ReadImages()
        {
            if (this.bmpStream == null)
            {
                this.bmpStream = File.OpenRead("/Users/pknopf/git/ImageSharp/tests/Images/Input/Bmp/Car.bmp");
                this.bmpCore = CoreImage.Load<Rgba32>(this.bmpStream);
                this.bmpStream.Position = 0;
                //this.bmpDrawing = Image.FromStream(this.bmpStream);
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.bmpStream.Dispose();
            this.bmpCore.Dispose();
            //this.bmpDrawing.Dispose();
        }

        [Benchmark(Description = "LibJpegTurbo")]
        public void LibJpegTurbo()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this.bmpCore.Save(memoryStream, new LibJpegTurboEncoder());
            }
        }
        
        // [Benchmark(Baseline = true, Description = "System.Drawing Jpeg")]
        // public void JpegSystemDrawing()
        // {
        //     using (MemoryStream memoryStream = new MemoryStream())
        //     {
        //         this.bmpDrawing.Save(memoryStream, ImageFormat.Jpeg);
        //     }
        // }

        [Benchmark(Description = "ImageSharp Jpeg")]
        public void JpegCore()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                this.bmpCore.SaveAsJpeg(memoryStream);
            }
        }
    }
}
