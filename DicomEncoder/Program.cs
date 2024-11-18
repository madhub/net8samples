// See https://aka.ms/new-console-template for more information

using FellowOakDicom;
using FellowOakDicom.Imaging;
using FellowOakDicom.Imaging.Codec;
using FellowOakDicom.Imaging.NativeCodec;
using FellowOakDicom.IO.Buffer;
using ImageMagick;

new DicomSetupBuilder()
  .RegisterServices(s => s.AddFellowOakDicom().AddTranscoderManager<FellowOakDicom.Imaging.NativeCodec.NativeTranscoderManager>())
  .SkipValidation()
  .Build();

//var path = @"C:\dev\gitrepos\net8samples\DicomEncoder\bin\Debug\net8.0\7f925f22-4e2e-45a9-a70b-89ceac6d3d33.dcm";
//var input = DicomFile.Open(path);
//var inputDicomPixelData = DicomPixelData.Create(input.Dataset, false);
//var frameData = inputDicomPixelData.GetFrame(0);

//var outFileName = $"{Guid.NewGuid().ToString()}.jpg";
//File.WriteAllBytes(outFileName, frameData.Data);
//using var imageFromFile = new MagickImage(outFileName);
Function();
void Function()
{



    var path = @"C:\dev\dicom\images\democases\democases\case1\case1_008.dcm";
    var input = DicomFile.Open(path);

    var inputDicomPixelData = DicomPixelData.Create(input.Dataset, false);
    var inputPixelDataByteBuffer = inputDicomPixelData.GetFrame(0);



    var nativetranscoder = new NativeTranscoderManager();
    var codec = nativetranscoder.GetCodec(DicomTransferSyntax.JPEGProcess1);
    Console.WriteLine(codec.TransferSyntax.UID);
    Console.WriteLine(DicomTransferSyntax.JPEGProcess1.UID);



    var inputDicomDataSet = new DicomDataset(input.Dataset.InternalTransferSyntax);
    inputDicomDataSet.AddOrUpdate(DicomTag.Rows, inputDicomPixelData.Height);
    inputDicomDataSet.AddOrUpdate(DicomTag.Columns, inputDicomPixelData.Width);
    inputDicomDataSet.AddOrUpdate(DicomTag.BitsAllocated, inputDicomPixelData.BitsAllocated);
    inputDicomDataSet.AddOrUpdate(DicomTag.BitsStored, inputDicomPixelData.BitsStored);
    inputDicomDataSet.AddOrUpdate(DicomTag.HighBit, inputDicomPixelData.HighBit);
    inputDicomDataSet.AddOrUpdate(DicomTag.PixelRepresentation, (ushort)inputDicomPixelData.PixelRepresentation);
    inputDicomDataSet.AddOrUpdate(DicomTag.PhotometricInterpretation, inputDicomPixelData.PhotometricInterpretation.Value);
    inputDicomDataSet.AddOrUpdate(DicomTag.SamplesPerPixel, inputDicomPixelData.SamplesPerPixel);

    byte[] inputBytes = inputPixelDataByteBuffer.Data; // new byte[] { 0, 0, 0 };// input pixel bytes
    var memoryBB = new MemoryByteBuffer(inputBytes);
    var pixelData = DicomPixelData.Create(inputDicomDataSet, true);
    pixelData.AddFrame(memoryBB);

    // transcode to request transfer syntax 
    var outputDataSet = inputDicomDataSet.Clone(DicomTransferSyntax.JPEGLSLossless);
    var outputPixelData = DicomPixelData.Create(outputDataSet, false);
    var outByteBuffer = outputPixelData.GetFrame(0);

    var outFileName = $"{Guid.NewGuid().ToString()}.jpg";
    File.WriteAllBytes(outFileName, outByteBuffer.Data);
    using var imageFromFile = new MagickImage(outFileName);

    Console.WriteLine(imageFromFile);
}






