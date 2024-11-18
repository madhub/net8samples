// See https://aka.ms/new-console-template for more information
using ImageMagick;
using System.Text.Json;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor;
using static System.Net.Mime.MediaTypeNames;

var gps = ImageMetadataReader.ReadMetadata(@"C:\madhu-docs\photos\one.JPG")
                             .OfType<GpsDirectory>()
                             .FirstOrDefault();

var location = gps.GetGeoLocation();

Console.WriteLine("Image at {0},{1}", location.Latitude, location.Longitude);


var pathToBackgroundImage = @"C:\madhu-docs\photos\one.JPG";
var pathToNewImage = Path.Combine(@"C:\madhu-docs\photos", "2FD-WithAddedText.jpg");
var locationString = $"Lat/Long={location.Latitude},{location.Longitude} ";
var textToWrite = locationString+"Krishna Rajendra Rd, Parvathipuram, Vishweshwarapura,  Basavanagudi, Bengaluru,Karnataka 560004";

// These settings will create a new caption
// which automatically resizes the text to best
// fit within the box.

var settings = new MagickReadSettings
{
    Font = "Calibri",
    TextGravity = Gravity.Northwest,
    BackgroundColor = MagickColors.WhiteSmoke,
    Height = 100, // height of text box
    Width = 250 // width of text box
    
///    ,StrokeColor = MagickColors.White,
    
};


using var image = new MagickImage(pathToBackgroundImage);
using var caption = new MagickImage($"caption:{textToWrite}", settings);

// Add the caption layer on top of the background image
// at position 590,450
int x = image.Width - settings.Width.Value;
int y = image.Height - settings.Height.Value;
image.Composite(caption, x, y, CompositeOperator.Over);

image.Write(pathToNewImage);

//// Load the image
//using (var image = new MagickImage(@"C:\madhu-docs\photos\one.JPG"))
//{

//    // Look up location details (optional)
//    // You'll need to implement this based on your preferred API or service
//    var locationDetails = "Bangalore"; /// GetLocationDetails(latitude., longitude);

//    //Compose watermark text
//   var watermarkText = $"Taken at: {locationDetails}";

//    image.Draw(new Drawables()
//        .FillColor(new MagickColor("black")) // -fill grey
//        .Gravity(Gravity.Southeast) // -gravity NorthWest
//        //.Rotation(45) // -draw "rotate 45"
//        .Text(10, 10, "Copyright").StrokeWidth(2)
//        ); // -draw "text 10,10 'Copyright'"
//    // Add watermark to the image
//    image.Draw(
//        new Drawables()
//            .Text(watermarkText, new MagickFont("Arial", 20))
//            .FillColor(new MagickColor("black", 50))
//            .Gravity(Gravity.Southeast)
//            .Stroke(new MagickColor("white", 1))
//    .StrokeWidth(2),
//        new MagickGeometry(10, 10)); // Adjust position and padding as needed

//    // Save the watermarked image
//    image.Write("watermarked_image.jpg");
//    //// Check if GPS data exists
//    //if (image.HasProperty("GPSLatitude") && image.HasProperty("GPSLongitude"))
//    //{
//    //    // Extract coordinates
//    //    var latitude = image.GetProperty("GPSLatitude").ToDecimal();
//    //    var longitude = image.GetProperty("GPSLongitude").ToDecimal();

//    //    // Look up location details (optional)
//    //    // You'll need to implement this based on your preferred API or service
//    //    var locationDetails = GetLocationDetails(latitude, longitude);

//    //    // Compose watermark text
//    //    var watermarkText = $"Taken at: {locationDetails}";

//    //    // Add watermark to the image
//    //    image.DrawText(
//    //        new Drawables()
//    //            .Text(watermarkText, new MagickFont("Arial", 20))
//    //            .FillColor(new MagickColor("black", 50))
//    //            .Gravity(Gravity.Southeast)
//    //            .Stroke(new MagickColor("white", 1))
//    //    .StrokeWidth(2),
//    //        new MagickGeometry(10, 10)); // Adjust position and padding as needed

//    //    // Save the watermarked image
//    //    image.Write("watermarked_image.jpg");
//    //}
//    //else
//    //{
//    //    Console.WriteLine("GPS data not found in the image.");
//    //}

//}

// string GetLocationDetails(double latitude, double longitude)
//{
//    using (var client = new HttpClient())
//    {
//        var apiKey = "YOUR_GOOGLE_MAPS_API_KEY"; // Replace with your API key
//        var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={apiKey}";

//        var response = client.GetStringAsync(url).Result;
//        var geocodeResult = JsonSerializer.Deserialize<Dictionary<string, object>>(response);

//        // Extract relevant location details from the API response
//        //var formattedAddress = geocodeResult["results"][0]["formatted_address"].ToString();

//        return string.Empty;
//    }
//}

